import http from "k6/http";
import { check } from "k6";
// import { randomItem } from 'k6/experimental/collections'; // Removed import

// Base URL of your application
const BASE_URL = "http://localhost:5000"; // Adjust if your app runs on a different port/host

const NUM_SETUP_KEYS = 100; // Number of keys to pre-populate for querying

// --- Setup Function --- Runs once before the test
export function setup() {
  console.log(`INFO: Setting up ${NUM_SETUP_KEYS} keys for query test...`);
  const keys = [];
  const setupParams = {
    headers: { "Content-Type": "application/json" },
    // Add a timeout specific to setup? k6 default is 60s
    // timeout: '120s',
  };

  for (let i = 0; i < NUM_SETUP_KEYS; i++) {
    const key = `query-test-key-${i}`;
    const value = { data: `Setup data for ${key}`, index: i };
    const payload = JSON.stringify({ Key: key, Value: value });

    // Insert into Postgres
    const pgRes = http.post(`${BASE_URL}/postgres/cache`, payload, setupParams);
    if (pgRes.status !== 200) {
      console.error(
        `ERROR: Setup failed for Postgres key ${key}. Status: ${pgRes.status}, Body: ${pgRes.body}`
      );
      // Optionally fail the test if setup is critical
      // fail(`Setup failed for Postgres key ${key}`);
      continue; // Skip adding this key if insert failed
    }

    // Insert into Redis
    const redisRes = http.post(`${BASE_URL}/redis/cache`, payload, setupParams);
    if (redisRes.status !== 200) {
      console.error(
        `ERROR: Setup failed for Redis key ${key}. Status: ${redisRes.status}, Body: ${redisRes.body}`
      );
      // Optionally fail the test if setup is critical
      // fail(`Setup failed for Redis key ${key}`);
      continue; // Skip adding this key if insert failed
    }

    keys.push(key); // Add key to the list only if both inserts succeeded
    if ((i + 1) % 10 === 0) {
      console.log(
        `INFO: Setup progress... ${i + 1}/${NUM_SETUP_KEYS} keys inserted.`
      );
    }
  }

  if (keys.length !== NUM_SETUP_KEYS) {
    console.warn(
      `WARN: Setup only successfully inserted ${keys.length}/${NUM_SETUP_KEYS} keys.`
    );
  }
  if (keys.length === 0) {
    fail("Setup failed: No keys were successfully inserted.");
  }

  console.log(
    `INFO: Setup complete. ${keys.length} keys available for querying.`
  );
  return { keys }; // Return the list of keys
}

// --- Configuration for the main test stage ---
export const options = {
  vus: 50,
  iterations: 20000, // 50 VUs * 2,000 iterations = 100,000 total queries
  thresholds: {
    "http_req_duration{endpoint:postgres_get}": ["p(95)<300"],
    "http_req_failed{endpoint:postgres_get}": ["rate<0.01"],
    "checks{endpoint:postgres_get}": ["rate>0.99"],
    "http_req_duration{endpoint:redis_get}": ["p(95)<200"],
    "http_req_failed{endpoint:redis_get}": ["rate<0.01"],
    "checks{endpoint:redis_get}": ["rate>0.99"],
  },
  // If setup fails, don't run the main iterations
  setupTimeout: "120s", // Allow more time for setup if needed
};

// --- Default Function --- Runs concurrently by VUs after setup
export default function (data) {
  // Receives data returned from setup()
  // Randomly select a key from the list created during setup using Math.random()
  const randomIndex = Math.floor(Math.random() * data.keys.length);
  const key = data.keys[randomIndex];

  const params = {
    headers: {
      Accept: "application/json",
    },
  };

  // --- Test Postgres Get ---
  const postgresRes = http.get(`${BASE_URL}/postgres/cache/${key}`, {
    ...params,
    tags: { endpoint: "postgres_get" },
  });
  check(
    postgresRes,
    {
      "Postgres: get status is 200": (r) => r.status === 200,
      "Postgres: get response is valid JSON": (r) => {
        try {
          const jsonData = r.json(); // Check if the response body is valid JSON
          return jsonData !== null && typeof jsonData === "object"; // Basic validation
        } catch (e) {
          console.error(`Postgres invalid JSON for key ${key}: ${r.body}`);
          return false;
        }
      },
    },
    { endpoint: "postgres_get" }
  );

  // --- Test Redis Get ---
  const redisRes = http.get(`${BASE_URL}/redis/cache/${key}`, {
    ...params,
    tags: { endpoint: "redis_get" },
  });
  check(
    redisRes,
    {
      "Redis: get status is 200": (r) => r.status === 200,
      "Redis: get response is valid JSON": (r) => {
        try {
          const jsonData = r.json(); // Check if the response body is valid JSON
          return jsonData !== null && typeof jsonData === "object"; // Basic validation
        } catch (e) {
          console.error(`Redis invalid JSON for key ${key}: ${r.body}`);
          return false;
        }
      },
    },
    { endpoint: "redis_get" }
  );
}

// --- Teardown Function --- Runs once after the test (optional)
export function teardown(data) {
  console.log(
    `INFO: Query test finished. Tested with ${data.keys.length} keys.`
  );
  // Optional: Clean up keys inserted during setup
  // console.log('INFO: Starting teardown cleanup...');
  // for (const key of data.keys) {
  //   http.del(`${BASE_URL}/postgres/cache/${key}`); // Assuming you add a DELETE endpoint
  //   http.del(`${BASE_URL}/redis/cache/${key}`);
  // }
  // console.log('INFO: Teardown cleanup finished.');
}
