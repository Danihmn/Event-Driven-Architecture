namespace Order.Application.Commands.ConfirmOrder;

public sealed record ConfirmOrderResponse(
    Guid Id,
    Guid ProductId,
    int Quantity,
    string Status,
    DateTime CreatedAt);