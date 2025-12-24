using System.Threading.Channels;
using Orders.Realtime.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// Create and register the Channel for order streaming
var channel = Channel.CreateUnbounded<OrderPlacement>();
builder.Services.AddSingleton(channel);
builder.Services.AddSingleton(channel.Reader);
builder.Services.AddSingleton(channel.Writer);

// Register the event buffer
builder.Services.AddSingleton<OrderEventBuffer>();

// Register the background worker
builder.Services.AddHostedService<OrderProducerService>();

builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors(p => p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseHttpsRedirection();

// Map the SSE endpoints
app.MapOrdersEndpoints();

app.Run();
