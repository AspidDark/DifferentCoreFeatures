using JasperFx.Core;
using Microsoft.Extensions.Logging;

namespace WolverineDemo;

public class CustomerCreatedHandler
{
    private readonly ILogger<CustomerCreatedHandler> _logger;
    
    public CustomerCreatedHandler(ILogger<CustomerCreatedHandler> logger)
    {
        _logger = logger;
    }

    public void Handle(CustomerCreated customerCreated)
    {
        _logger.LogInformation(customerCreated.ToString());
    }
}
