using Contracts;
using Contracts.Event;

namespace Order.Events;

public record OrderCreatedEvent(
    Guid OrderId,
    Guid ProductId,
    int Quantity) : Event;