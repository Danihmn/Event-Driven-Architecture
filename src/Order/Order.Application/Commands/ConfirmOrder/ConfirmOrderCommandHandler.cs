using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using Order.Application.Commands.CancelOrder;
using Order.Domain.Repository;

namespace Order.Application.Commands.ConfirmOrder;

public class ConfirmOrderCommandHandler(IOrderRepository repository, ILogger<CancelOrderCommandHandler> logger)
    : IRequestHandler<ConfirmOrderCommand, Result<ConfirmOrderResponse>>
{
    public async Task<Result<ConfirmOrderResponse>> Handle(ConfirmOrderCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting to confirm order {OrderId}", request.OrderId);

        var order = await repository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            logger.LogWarning("Cannot confirm order {OrderId} because it was not found", request.OrderId);
            return Result.Fail<ConfirmOrderResponse>("Order not found");
        }

        try
        {
            order.Confirm();
        }
        catch (InvalidOperationException)
        {
            return Result.Fail<ConfirmOrderResponse>("Could not confirm order, it was not pending");
        }

        await repository.UpdateAsync(order, cancellationToken);

        logger.LogInformation("Starting to cancel order {OrderId}", request.OrderId);

        return Result.Ok(new ConfirmOrderResponse(
            Id: order.Id,
            ProductId: order.ProductId,
            Quantity: order.Quantity,
            Status: order.Status.ToString(),
            CreatedAt: order.CreatedAt));
    }
}