using FluentResults;
using MediatR;

namespace Order.Application.Commands.GetAllOrders;

public sealed record GetAllOrdersCommand(int Skip, int Take)
    : IRequest<Result<IEnumerable<GetAllOrdersResponse>>>;