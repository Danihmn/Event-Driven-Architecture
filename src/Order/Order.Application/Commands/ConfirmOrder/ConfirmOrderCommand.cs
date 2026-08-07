using FluentResults;
using MediatR;

namespace Order.Application.Commands.ConfirmOrder;

public sealed record ConfirmOrderCommand(Guid OrderId) : IRequest<Result<ConfirmOrderResponse>>;