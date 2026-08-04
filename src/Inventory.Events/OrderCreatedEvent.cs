using Contracts;

namespace Inventory.Events;

public record OrderCreatedEvent(Guid OrderId, Guid ProductId, int Quantity) : Event;