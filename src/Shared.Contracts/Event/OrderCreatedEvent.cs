namespace Contracts.Event;

public record OrderCreatedEvent(
    Guid OrderId,
    Guid ProductId,
    int Quantity) : Abstractions.Event;