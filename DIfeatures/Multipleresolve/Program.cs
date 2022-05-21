using DIFeatures;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

//+Imp1
builder.Services.AddTransient<IHelloer, HelloerA>();
builder.Services.AddTransient<IHelloer, HelloerB>();
//-Imp1



//+Imp2
builder.Services.AddScoped<Service1>();
builder.Services.AddScoped<Service2>();

builder.Services.AddScoped<Func<int, IContractService>>(serviceProvider => key =>
{
    return key switch
    {
        1 => serviceProvider.GetService<Service1>(),
        2 => serviceProvider.GetService<Service2>(),
        _ => throw new Exception("Not valid key"),
    };
});
//-Imp2



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//https://stackoverflow.com/questions/50522111/how-to-register-two-implementations-then-get-one-in-net-core-dependency-injecti
app.Run();

//https://edi.wang/post/2018/12/28/dependency-injection-with-multiple-implementations-in-aspnet-core
//https://www.c-sharpcorner.com/article/multiple-interface-implementations-in-asp-net-core/
