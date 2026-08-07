using Contracts;
using Contracts.Infra.Publish;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using Order.Domain.Repository;
using Shared.Implementations.Event;

namespace Order.Application.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler(
    IOrderRepository repository,
    IEventPublisher publisher,
    ILogger<CreateOrderCommandHandler> logger)
    : IRequestHandler<CreateOrderCommand, Result<CreateOrderResponse>>
{
    public async Task<Result<CreateOrderResponse>> Handle(CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting to create order for product {ProductId}", request.ProductId);

        var orderResult = Domain.Entities.Order.Create(request.ProductId, request.Quantity);

        if (orderResult.IsFailed)
        {
            logger.LogWarning("Failed to create order for product {ProductId}", request.ProductId);
            return Result.Fail<CreateOrderResponse>(orderResult.Errors);
        }

        var order = orderResult.Value;

        await repository.CreateAsync(order, cancellationToken);

        await publisher.PublishAsync(Topics.OrderCreated,
            new OrderCreatedEvent(order.Id, order.ProductId, order.Quantity), cancellationToken);

        logger.LogInformation("Created order {OrderId} for product {ProductId}", order.Id, order.ProductId);

        return Result.Ok(new CreateOrderResponse(
            Id: order.Id,
            ProductId: order.ProductId,
            Quantity: order.Quantity,
            Status: order.Status.ToString(),
            CreatedAt: order.CreatedAt));
    }
}