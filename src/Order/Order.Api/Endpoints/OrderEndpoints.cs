using MediatR;
using Order.Application.Commands.CreateOrder;

namespace Order.Api.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("orders/");

        group.MapPost("order", async (CreateOrderCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);

            if (result.IsSuccess)
                return Results.Created();

            return Results.BadRequest(new
            {
                Errors = result.Errors
            });
        });
    }
}