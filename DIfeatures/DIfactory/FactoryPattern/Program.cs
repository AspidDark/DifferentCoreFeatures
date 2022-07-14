using FactoryPattern.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using FactoryPattern.Samples;
using FactoryPattern.Factories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
//builder.Services.AddTransient<ISample1, Sample1>();
//builder.Services.AddSingleton<Func<ISample1>>(x => () => x.GetService<ISample1>()!);
builder.Services.AddAbstractFactory<ISample1, Sample1>();
builder.Services.AddAbstractFactory<ISample2, Sample2>();
builder.Services.AddGenericClassWithDataFactory();
builder.Services.AddVehicleFactory();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
