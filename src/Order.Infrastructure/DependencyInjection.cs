using Microsoft.Extensions.DependencyInjection;
using Order.Domain.Repository;
using Order.Infrastructure.Repository;

namespace Order.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<IOrderRepository, OrderRepository>();
        return services;
    }
}