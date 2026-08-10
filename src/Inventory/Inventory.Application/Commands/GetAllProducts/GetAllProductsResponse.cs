namespace Inventory.Application.Commands.GetAllProducts;

public record GetAllProductsResponse(
    Guid Id,
    string Name,
    decimal Price,
    int QuantityAvailable,
    DateTime CreatedAt);