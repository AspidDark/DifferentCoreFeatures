var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("postgres").WithPgAdmin().AddDatabase("caching-db");

var cache = builder.AddRedis("redis").WithRedisInsight();

builder.AddProject<Projects.Postgres_Caching>("postgres-caching")
    .WithHttpEndpoint(5000, name: "public-http")
    .WithReference(db).WaitFor(db)
    .WithReference(cache).WaitFor(cache);

builder.Build().Run();
