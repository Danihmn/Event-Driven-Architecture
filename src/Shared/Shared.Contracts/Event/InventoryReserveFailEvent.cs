namespace Contracts.Event;

public record InventoryReserveFailEvent(
    Guid OrderId,
    Guid ProductId,
    int Quantity) : Abstractions.Event;