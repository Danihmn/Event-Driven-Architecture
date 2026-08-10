using FluentResults;
using Inventory.Domain.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Inventory.Application.Commands.GetAllProducts;

public class
    GetAllProductsCommandHandler(IProductRepository repository, ILogger<GetAllProductsCommandHandler> logger)
    : IRequestHandler<GetAllProductsCommand, Result<IEnumerable<GetAllProductsResponse>>>
{
    public async Task<Result<IEnumerable<GetAllProductsResponse>>> Handle(
        GetAllProductsCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting to get all products");

        var products =
            await repository.GetAllAsync(request.Skip, request.Take, cancellationToken);

        var responses = (products ?? []).Select(product =>
            new GetAllProductsResponse(
                product.Id, product.Name, product.Price, product.QuantityAvailable, product.CreatedAt));

        return Result.Ok(responses);
    }
}