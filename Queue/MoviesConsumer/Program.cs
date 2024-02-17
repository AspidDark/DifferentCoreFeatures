using Wolverine;
using Wolverine.AmazonSqs;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseWolverine(x =>
{
    x.ListenToSqsQueue("movies-queue").UseForReplies();
    
    x.UseAmazonSqsTransport().AutoProvision();
});

var app = builder.Build();

app.Run();
