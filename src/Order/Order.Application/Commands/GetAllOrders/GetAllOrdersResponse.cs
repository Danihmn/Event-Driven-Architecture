namespace Order.Application.Commands.GetAllOrders;

public record GetAllOrdersResponse(Guid Id, Guid ProductId, int Quantity, string Status, DateTime CreatedAt);