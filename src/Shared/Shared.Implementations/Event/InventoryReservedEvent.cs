namespace Shared.Implementations.Event;

public record InventoryReservedEvent(
    Guid OrderId,
    Guid ProductId,
    int Quantity) : Contracts.Event;