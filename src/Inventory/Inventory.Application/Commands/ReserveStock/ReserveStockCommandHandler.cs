using Contracts;
using Contracts.Event;
using Contracts.Infra.Publish;
using FluentResults;
using Inventory.Domain.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

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

        if (product is null)
        {
            await SendFailedReserveStock(request, cancellationToken);

            logger.LogWarning("Cannot reserve stock for product {ProductId} because it was not found",
                request.ProductId);
            return Result.Fail<ReserveStockResponse>("Product not found");
        }

        var reserveResult = product.Reserve(request.Quantity);

        if (reserveResult.IsFailed)
        {
            await SendFailedReserveStock(request, cancellationToken);

            logger.LogWarning("Failed to reserve stock for product {ProductId}", request.ProductId);
            return Result.Fail<ReserveStockResponse>(reserveResult.Errors);
        }

        await repository.UpdateAsync(product, cancellationToken);

        await publisher.PublishAsync(Topics.StockReserved,
            new InventoryReservedEvent(request.OrderId, request.ProductId, request.Quantity), cancellationToken);

        logger.LogInformation("Reserved {Quantity} units of product {ProductId}", request.Quantity, product.Id);

        return Result.Ok(new ReserveStockResponse(
            ProductId: product.Id,
            QuantityAvailable: product.QuantityAvailable,
            QuantityReserved: request.Quantity));
    }

    private async Task SendFailedReserveStock(ReserveStockCommand request, CancellationToken cancellationToken)
        => await publisher.PublishAsync(Topics.StockReservationFailed,
            new InventoryReserveFailEvent(request.OrderId, request.ProductId, request.Quantity), cancellationToken);
}