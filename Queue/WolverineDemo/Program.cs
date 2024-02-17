using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wolverine;
using WolverineDemo;

var builder = Host.CreateDefaultBuilder();

builder.UseWolverine();

builder.ConfigureServices(x =>
{
    x.AddHostedService<BgPublisher>();
});

var app = builder.Build();

app.Run();

public record CreateCustomer(Guid Id, string FullName);

public record CustomerCreated(Guid Id, string FullName);
