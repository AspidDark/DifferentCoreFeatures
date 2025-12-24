namespace Orders.Realtime.Api;

public record OrderPlacement(
    string OrderId,
    string CustomerName,
    decimal Amount,
    DateTime Timestamp
);