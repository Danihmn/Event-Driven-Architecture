using Contracts;

namespace Order.Events;

public record InventoryReservedEvent(
    Guid OrderId,
    Guid ProductId,
    int Quantity
) : Event;