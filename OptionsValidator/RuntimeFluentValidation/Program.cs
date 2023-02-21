using FluentValidation;
using RuntimeFluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var config = builder.Configuration;

builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);

//builder.Services.AddSingleton<IValidator<Options>, ExampleValidation>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOptions<Options>()
    .Bind(config.GetSection(Options.sectionName))
    //.Validate(x =>
    //{
    //    return x.SomeValue != string.Empty;
    //})
    .ValidateFluently()
    .ValidateOnStart();


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
