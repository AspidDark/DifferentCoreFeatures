using DIFeatures;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DIFeatures
{
    var builder = WebApplication.CreateBuilder(args);

    //Оба сервиса зарегистрированы
    builder.Services.AddSingleton<ICoolService, CoolServiceOne>();
builder.Services.AddSingleton<ICoolService, CoolServiceTwo>();
//Оба сервиса зарегистрированы но при резолве виден будет последний но если резолвить как
//IServiceCollection<ICoolService>  то достать можно оба

var descriptor = new ServiceDescriptor(typeof(ICoolService), typeof(CoolServiceOne), ServiceLifetime.Singleton);
    //Еще один способ регать зависимости в рантайме!!!


    builder.Services.TryAddSingleton<ICoolService, CoolServiceOne>();
builder.Services.TryAddSingleton<ICoolService, CoolServiceTwo>();
//Зареган 1 сервис только первый! Смотрит по интерфейсу для которого регают

//Оба сервиса зарегистрированы
builder.Services.AddSingleton<ICoolService, CoolServiceOne>();
builder.Services.AddSingleton<ICoolService, CoolServiceOne>();
//Оба сервиса зарегистрированы но 2 одинаковых сервиса!!!!!!!! и оба можно вытащить

var descriptorOne = new ServiceDescriptor(typeof(ICoolService), typeof(CoolServiceOne), ServiceLifetime.Singleton);
    var descriptorTwo = new ServiceDescriptor(typeof(ICoolService), typeof(CoolServiceTwo), ServiceLifetime.Singleton);
    builder.Services.TryAddEnumerable(new[] { descriptorOne, descriptorTwo
});
//Добавлет несколько зависимостей!


builder.Services.TryAddEnumerable(descriptorOne);
builder.Services.TryAddEnumerable(descriptorTwo);
builder.Services.TryAddEnumerable(descriptorTwo);
//Третий не зарегистрируется т к в нем сверка идет И по интерфейсу И пореализации


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

app.Run();




public interface ICoolService
{
}

public class CoolServiceOne : ICoolService
{

}

public class CoolServiceTwo : ICoolService
{

}
}
