namespace Shared.Implementations.Event;

public record OrderCreatedEvent(
    Guid OrderId,
    Guid ProductId,
    int Quantity) : Contracts.Event;