using MediatR;
using Order.Application.Commands.CreateOrder;
using Order.Application.Commands.GetAllOrders;

namespace Order.Api.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("orders/");

        group.MapGet("", async
            ([AsParameters] GetAllOrdersCommand request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(request, ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(new
                {
                    result.Errors,
                });
        }).WithName("GetAllOrders").WithDescription("Returns all orders");

        group.MapPost("create", async (CreateOrderCommand request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(request, ct);
            return result.IsSuccess
                ? Results.Created()
                : Results.BadRequest(new
                {
                    result.Errors,
                });
        }).WithName("CreateOrder").WithDescription("Creates a new order");
    }
}