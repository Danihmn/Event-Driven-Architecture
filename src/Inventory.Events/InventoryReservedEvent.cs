using Contracts;
using Contracts.Event;

namespace Inventory.Events;

public record InventoryReservedEvent(
    Guid OrderId,
    Guid ProductId,
    int Quantity) : Event;