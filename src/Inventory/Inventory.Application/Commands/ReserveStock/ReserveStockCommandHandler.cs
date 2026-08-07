using Contracts;
using Contracts.Infra.Publish;
using FluentResults;
using Inventory.Domain.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Implementations.Event;

namespace Inventory.Application.Commands.ReserveStock;

public sealed class ReserveStockCommandHandler(
    IProductRepository repository,
    IEventPublisher publisher,
    ILogger<ReserveStockCommandHandler> logger)
    : IRequestHandler<ReserveStockCommand, Result<ReserveStockResponse>>
{
    public async Task<Result<ReserveStockResponse>> Handle(ReserveStockCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting to reserve stock for product {ProductId}", request.ProductId);

        var product = await repository.GetByIdAsync(request.ProductId, cancellationToken);

        var reserveResult = product is null
            ? Result.Fail("Product not found")
            : product.Reserve(request.Quantity);

        if (reserveResult.IsFailed)
        {
            await publisher.PublishAsync(Topics.StockReservationFailed,
                new InventoryReserveFailEvent(request.OrderId, request.ProductId, request.Quantity), cancellationToken);

            logger.LogWarning("Failed to reserve stock for product {ProductId}", request.ProductId);
            return Result.Fail<ReserveStockResponse>(reserveResult.Errors);
        }

        await repository.UpdateAsync(product!, cancellationToken);

        await publisher.PublishAsync(Topics.StockReserved,
            new InventoryReservedEvent(request.OrderId, request.ProductId, request.Quantity), cancellationToken);

        logger.LogInformation("Reserved {Quantity} units of product {ProductId}", request.Quantity, product!.Id);

        return Result.Ok(new ReserveStockResponse(product.Id, product.QuantityAvailable, request.Quantity));
    }
}