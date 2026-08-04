namespace Order.Application.Commands.CreateOrder;

public sealed record CreateOrderResponse(Guid Id, Guid ProductId, int Quantity, string Status, DateTime CreatedAt);
