namespace Inventory.Application.Commands.ReserveStock;

public sealed record ReserveStockResponse(Guid ProductId, int QuantityAvailable, int QuantityReserved);
