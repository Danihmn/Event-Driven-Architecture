using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using Order.Domain.Repository;

namespace Order.Application.Commands.CancelOrder;

public class CancelOrderCommandHandler(IOrderRepository repository, ILogger<CancelOrderCommandHandler> logger)
    : IRequestHandler<CancelOrderCommand, Result<CancelOrderResponse>>
{
    public async Task<Result<CancelOrderResponse>> Handle(CancelOrderCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting to cancel order {OrderId}", request.OrderId);

        var order = await repository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            logger.LogWarning("Cannot cancel order {OrderId} because it was not found", request.OrderId);
            return Result.Fail<CancelOrderResponse>("Order not found");
        }

        try
        {
            order.Cancel();
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<CancelOrderResponse>("Could not cancel order, it was already cancelled");
        }

        await repository.UpdateAsync(order, cancellationToken);

        logger.LogInformation("Starting to cancel order {OrderId}", request.OrderId);

        return Result.Ok(new CancelOrderResponse(
            Id: order.Id,
            ProductId: order.ProductId,
            Quantity: order.Quantity,
            Status: order.Status.ToString(),
            CreatedAt: order.CreatedAt));
    }
}