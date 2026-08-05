using FluentResults;
using Inventory.Domain.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Inventory.Application.Commands.ReserveStock;

public sealed class ReserveStockCommandHandler(
    IProductRepository repository,
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
            logger.LogWarning("Cannot reserve stock for product {ProductId} because it was not found",
                request.ProductId);
            return Result.Fail<ReserveStockResponse>("Product not found");
        }

        var reserveResult = product.Reserve(request.Quantity);

        if (reserveResult.IsFailed)
        {
            logger.LogWarning("Failed to reserve stock for product {ProductId}", request.ProductId);
            return Result.Fail<ReserveStockResponse>(reserveResult.Errors);
        }

        await repository.UpdateAsync(product, cancellationToken);

        logger.LogInformation("Reserved {Quantity} units of product {ProductId}", request.Quantity, product.Id);

        return Result.Ok(new ReserveStockResponse(
            ProductId: product.Id,
            QuantityAvailable: product.QuantityAvailable,
            QuantityReserved: request.Quantity));
    }
}