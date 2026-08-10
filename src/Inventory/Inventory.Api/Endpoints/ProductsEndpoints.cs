using Inventory.Application.Commands.GetAllProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Endpoints;

public static class ProductsEndpoints
{
    public static void MapProductsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("products/");

        group.MapGet("", async
            ([AsParameters] GetAllProductsCommand request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(request, ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(new
                {
                    result.Errors,
                });
        }).WithName("GetAllProducts").WithDescription("Gets all products");
    }
}