namespace Order.Application.Commands.CancelOrder;

public sealed record CancelOrderResponse(Guid Id, Guid ProductId, int Quantity, string Status, DateTime CreatedAt);