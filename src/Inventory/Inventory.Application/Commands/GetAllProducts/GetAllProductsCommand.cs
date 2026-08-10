using FluentResults;
using MediatR;

namespace Inventory.Application.Commands.GetAllProducts;

public sealed record GetAllProductsCommand(int Skip, int Take)
    : IRequest<Result<IEnumerable<GetAllProductsResponse>>>;