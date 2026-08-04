namespace Events;

public record Event(
    Guid EventId,
    Guid OrderId,
    Guid ProductId,
    int Quantity,
    DateTime OcurredAt);