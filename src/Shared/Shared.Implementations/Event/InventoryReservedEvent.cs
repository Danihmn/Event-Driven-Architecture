namespace Contracts.Event;

public record InventoryReservedEvent(
    Guid OrderId,
    Guid ProductId,
    int Quantity) : Abstractions.Event;