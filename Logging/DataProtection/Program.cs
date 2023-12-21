using System.Text.Json;
using DataProtection;
using Microsoft.Extensions.Compliance.Classification;
using Microsoft.Extensions.Compliance.Redaction;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole(options => options.JsonWriterOptions = new JsonWriterOptions
{
    Indented = true
});

builder.Logging.EnableRedaction();

builder.Services.AddRedaction(x =>
{
    //x.SetRedactor<ErasingRedactor>(new DataClassificationSet(DataTaxonomy.SensitiveData));
    x.SetRedactor<StarRedactor>(new DataClassificationSet(DataTaxonomy.SensitiveData));
    
#pragma warning disable EXTEXP0002
    x.SetHmacRedactor(options =>
    {
        options.Key = Convert.ToBase64String("SecretKeyDontHardcodeInsteadStoreAndLoadSecurely"u8);
        options.KeyId = 69;
    }, new DataClassificationSet(DataTaxonomy.PiiData));
});

var app = builder.Build();

app.MapPost("/customers", (Customer customer, ILogger<Program> logger) =>
{
    //_customerService.Create(customer); Assume customer created
    
    //logger.LogInformation("Customer created {Customer}", customer);
    logger.LogCustomerCreated(customer);
    return customer;
});

app.Run();

public record Customer(
    [SensitiveData]string Name, 
    [PiiData]string Email, 
    DateOnly DateOfBirth)
{
    public Guid Id { get; } = Guid.NewGuid();
};


