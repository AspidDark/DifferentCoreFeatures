using GrpcTry.Service;


//!!! Click on every project!!!! add this!!!
//< ItemGroup >

//        < Protobuf Include = "Protos\greet.proto" GrpcServices = "Server" />

//    </ ItemGroup >

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapGrpcService<GreeterService>();

app.UseAuthorization();

app.MapControllers();

app.Run();
