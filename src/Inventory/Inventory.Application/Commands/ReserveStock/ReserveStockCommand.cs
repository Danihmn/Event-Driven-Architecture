using FluentResults;
using MediatR;

namespace Inventory.Application.Commands.ReserveStock;

public sealed record ReserveStockCommand(Guid ProductId, Guid OrderId, int Quantity)
    : IRequest<Result<ReserveStockResponse>>;