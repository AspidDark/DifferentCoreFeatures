using JasperFx.Core;
using Microsoft.Extensions.Logging;

namespace WolverineDemo;

public class CreateCustomerHandler
{
    private readonly ILogger<CreateCustomerHandler> _logger;
    
    public CreateCustomerHandler(ILogger<CreateCustomerHandler> logger)
    {
        _logger = logger;
    }

    public CustomerCreated Handle(CreateCustomer createCustomer)
    {
        _logger.LogInformation(createCustomer.ToString());
        return new CustomerCreated(createCustomer.Id, createCustomer.FullName);
    }
}
