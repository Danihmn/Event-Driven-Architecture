using FluentResults;
using MediatR;

namespace Inventory.Application.Commands.GetAllProducts;

public sealed record GetAllProductsCommand(int Skip = 0, int Take = 50)
    : IRequest<Result<IEnumerable<GetAllProductsResponse>>>;