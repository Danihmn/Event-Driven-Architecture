using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using Order.Domain.Repository;

namespace Order.Application.Commands.GetAllOrders;

public class GetAllOrdersCommandHandler(IOrderRepository repository, ILogger<GetAllOrdersCommandHandler> logger)
    : IRequestHandler<GetAllOrdersCommand, Result<IEnumerable<GetAllOrdersResponse>>>
{
    public async Task<Result<IEnumerable<GetAllOrdersResponse>>> Handle(GetAllOrdersCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting to get all orders");

        var orders = await repository.GetAllAsync(request.Skip, request.Take, cancellationToken);

        var responses = (orders ?? []).Select(order => new GetAllOrdersResponse
            (order.Id, order.ProductId, order.Quantity, order.Status.ToString(), order.CreatedAt));

        return Result.Ok(responses);
    }
}