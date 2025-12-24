using System.Threading.Channels;

namespace Orders.Realtime.Api;

public class OrderProducerService(
    ChannelWriter<OrderPlacement> channelWriter,
    ILogger<OrderProducerService> logger)
    : BackgroundService
{
    private static readonly string[] CustomerNames =
    [
        "Alice Johnson", "Bob Smith", "Charlie Brown", "Diana Prince",
        "Eve Wilson", "Frank Miller", "Grace Lee", "Henry Davis"
    ];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var random = new Random();
        var orderCounter = 1;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var order = new OrderPlacement(
                    OrderId: $"ORD-{orderCounter++:D6}",
                    CustomerName: CustomerNames[random.Next(CustomerNames.Length)],
                    Amount: Math.Round((decimal)(random.NextDouble() * 1000 + 10), 2),
                    Timestamp: DateTime.UtcNow
                );

                await channelWriter.WriteAsync(order, stoppingToken);

                logger.LogInformation(
                    "Produced order: {OrderId} for {CustomerName}", order.OrderId, order.CustomerName);

                // Wait 3-5 seconds before producing next order
                await Task.Delay(TimeSpan.FromSeconds(random.Next(3, 5)), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error producing order");
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }

        channelWriter.Complete();
    }
}
