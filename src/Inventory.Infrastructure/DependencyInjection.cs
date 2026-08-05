using Inventory.Domain.Repository;
using Inventory.Infrastructure.Data;
using Inventory.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<InventoryDbContext>();
        services.AddTransient<IProductRepository, ProductRepository>();

        return services;
    }
}