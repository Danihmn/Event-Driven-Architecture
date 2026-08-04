using FluentResults;
using MediatR;

namespace Order.Application.Commands.CreateOrder;

public sealed record CreateOrderCommand(Guid ProductId, int Quantity) : IRequest<Result<CreateOrderResponse>>;