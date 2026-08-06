using FluentResults;
using MediatR;

namespace Inventory.Application.Commands.ReserveStock;

public sealed record ReserveStockCommand(Guid ProductId, int Quantity) : IRequest<Result<ReserveStockResponse>>;
