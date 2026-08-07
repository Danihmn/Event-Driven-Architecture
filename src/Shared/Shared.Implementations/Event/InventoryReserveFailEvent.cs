namespace Shared.Implementations.Event;

public record InventoryReserveFailEvent(
    Guid OrderId,
    Guid ProductId,
    int Quantity) : Contracts.Event;