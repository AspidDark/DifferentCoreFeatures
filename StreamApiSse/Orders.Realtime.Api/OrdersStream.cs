using System.Net.ServerSentEvents;
using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc;

namespace Orders.Realtime.Api;

public static class OrdersStream
{
    public static void MapOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("orders/realtime", (
            ChannelReader<OrderPlacement> channelReader,
            CancellationToken cancellationToken) =>
        {
            return Results.ServerSentEvents(
                channelReader.ReadAllAsync(cancellationToken),
                "orders");
        });

        app.MapGet("orders/realtime/with-events", (
            ChannelReader<OrderPlacement> channelReader,
            OrderEventBuffer eventBuffer,
            [FromHeader(Name = "Last-Event-ID")] string? lastEventId,
            CancellationToken cancellationToken) =>
        {
            async IAsyncEnumerable<SseItem<OrderPlacement>> StreamEvents()
            {
                if (!string.IsNullOrWhiteSpace(lastEventId))
                {
                    var missedEvents = eventBuffer.GetEventsAfter(lastEventId);
                    foreach (var missedEvent in missedEvents)
                    {
                        yield return missedEvent;
                    }
                }

                await foreach (var order in channelReader.ReadAllAsync(cancellationToken))
                {
                    var sseItem = eventBuffer.Add(order);
                    yield return sseItem;
                }
            }

            return TypedResults.ServerSentEvents(
                StreamEvents(),
                "orders");
        });
    }
}