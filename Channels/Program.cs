using Channels;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



//2 publishers  1 reader
var channel = Channel.CreateUnbounded<ChannelData>();
builder.Services.AddSingleton(channel);
builder.Services.AddSingleton<ChannelReader<ChannelData>>(sp => channel.Reader);
builder.Services.AddSingleton<ChannelWriter<ChannelData>>(sp => channel.Writer);
//builder.Services.AddHostedService<BackGroundSingle>();
//builder.Services.AddHostedService<BackGroundSingleWriter>();


//1 publiser  2 readers differnt messages processing
builder.Services.AddSingleton<Channel<ChannelData3>>(
    _ => Channel.CreateUnbounded<ChannelData3>(new UnboundedChannelOptions 
    {
        SingleReader = false,
        SingleWriter = false,
        
    }));
builder.Services.AddHostedService<BackGroundManyReader3>();
builder.Services.AddHostedService<BackGroundManyReader4>();


builder.Services.AddSingleton<Channel<ChannelData3>>(
    _ => Channel.CreateBounded<ChannelData3>(new BoundedChannelOptions(10)
    {
        SingleReader = false,
        SingleWriter = false,
    }));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
