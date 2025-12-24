using System.Collections.Concurrent;
using System.Net.ServerSentEvents;

namespace Orders.Realtime.Api;

public class OrderEventBuffer(int maxBufferSize = 100)
{
    private readonly ConcurrentQueue<SseItem<OrderPlacement>> _buffer = new();
    private long _nextEventId = 1;

    public SseItem<OrderPlacement> Add(OrderPlacement order)
    {
        var eventId = Interlocked.Increment(ref _nextEventId) - 1;
        var sseItem = new SseItem<OrderPlacement>(order)
        {
            EventId = eventId.ToString()
        };

        _buffer.Enqueue(sseItem);

        // Maintain buffer size by removing the oldest items
        while (_buffer.Count > maxBufferSize)
        {
            _buffer.TryDequeue(out _);
        }

        return sseItem;
    }

    public IEnumerable<SseItem<OrderPlacement>> GetEventsAfter(string? lastEventId)
    {
        if (string.IsNullOrEmpty(lastEventId))
        {
            return [];
        }

        if (!long.TryParse(lastEventId, out var lastId))
        {
            return [];
        }

        return _buffer
            .Where(item => long.TryParse(item.EventId, out var itemId) && itemId > lastId)
            .OrderBy(item => long.Parse(item.EventId!));
    }

    public long GetCurrentEventId() => _nextEventId - 1;
}
