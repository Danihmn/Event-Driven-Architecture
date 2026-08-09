using FluentResults;
using MediatR;

namespace Order.Application.Commands.GetAllOrders;

public sealed record GetAllOrdersCommand(int Skip = 0, int Take = 50)
    : IRequest<Result<IEnumerable<GetAllOrdersResponse>>>;