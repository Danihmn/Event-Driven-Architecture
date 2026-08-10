using Inventory.Application.Commands.GetAllProducts;
using MediatR;

namespace Inventory.Api.Endpoints;

public static class ProductsEndpoints
{
    public static void MapProductsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("products/");

        group.MapGet("", async (ISender sender, CancellationToken ct, int skip, int take) =>
        {
            var result = await sender.Send(new GetAllProductsCommand(skip, take), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(new
                {
                    result.Errors,
                });
        }).WithName("GetAllProducts").WithDescription("Gets all products");
    }
}