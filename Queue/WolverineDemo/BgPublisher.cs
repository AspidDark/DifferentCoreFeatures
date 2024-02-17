using Microsoft.Extensions.Hosting;
using Wolverine;

namespace WolverineDemo;

public class BgPublisher : BackgroundService
{
    private readonly IMessageBus _messageBus;

    public BgPublisher(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _messageBus.SendAsync(new CreateCustomer(Guid.NewGuid(), "Nick Chapsas"));
            await Task.Delay(2000, stoppingToken);
        }
    }
}
