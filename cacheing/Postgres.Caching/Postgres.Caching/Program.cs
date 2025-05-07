using Postgres.Caching;
using System.Text.Json;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddRedisClient("redis");
builder.AddNpgsqlDataSource("caching-db");

builder.Services.AddSingleton<ICacheService, PostgresCacheService>();

var app = builder.Build();

app.MapPost("/postgres/cache", async ([FromBody] CacheItem cacheItem, NpgsqlDataSource dataSource) =>
{
    using var connection = dataSource.OpenConnection();

    await connection.ExecuteAsync(
        """
        INSERT INTO cache(key, value)
        VALUES (@Key, @Value::jsonb)
        ON CONFLICT (key) DO UPDATE
        SET value = excluded.value;
        """,
        new { cacheItem.Key, Value = cacheItem.Value.ToString() });

    return Results.Ok();
});

app.MapGet("/postgres/cache/{key}", async (string key, NpgsqlDataSource dataSource) =>
{
    using var connection = dataSource.OpenConnection();

    var value = await connection.QuerySingleOrDefaultAsync<string>(
        "SELECT value FROM cache WHERE key = @Key",
        new { Key = key });

    return value is not null ?
        Results.Ok(JsonDocument.Parse(value).RootElement) :
        Results.NotFound();
});

app.MapPost("/redis/cache", async ([FromBody] CacheItem item, IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();

    await db.StringSetAsync(item.Key, JsonSerializer.Serialize(item.Value));

    return Results.Ok();
});

app.MapGet("/redis/cache/{key}", async (string key, IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();

    var value = await db.StringGetAsync(key);

    return value.HasValue ?
        Results.Ok(JsonDocument.Parse(value.ToString()).RootElement) :
        Results.NotFound();
});

await ApplyDatabaseMigrations(app);

app.MapDefaultEndpoints();

app.UseHttpsRedirection();

app.Run();

static async Task ApplyDatabaseMigrations(WebApplication app)
{
    var dataSource = app.Services.GetRequiredService<NpgsqlDataSource>();
    using var connection = dataSource.OpenConnection();

    await connection.ExecuteAsync(
        """
        CREATE UNLOGGED TABLE IF NOT EXISTS cache (
        	id SERIAL PRIMARY KEY,
        	key TEXT UNIQUE NOT NULL,
        	value JSONB,
        	created_at_utc TIMESTAMP DEFAULT CURRENT_TIMESTAMP);
        
        CREATE INDEX IF NOT EXISTS idx_cache_key ON cache (key) INCLUDE (value);
        """);
}

// Add the CacheItem record definition
public record CacheItem(string Key, JsonElement Value);
