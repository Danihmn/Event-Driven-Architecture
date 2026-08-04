using Microsoft.EntityFrameworkCore;
using Order.Domain.Repository;
using Order.Infrastructure.Data;

namespace Order.Infrastructure.Repository;

public class OrderRepository(OrderDbContext context) : IOrderRepository
{
    public async Task<IEnumerable<Domain.Entities.Order>> GetAllAsync(int skip, int take,
        CancellationToken cancellationToken = default)
        => await context.Orders.AsNoTracking().Skip(skip).Take(take).ToListAsync(cancellationToken);

    public async Task<Domain.Entities.Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Orders.FindAsync([id], cancellationToken);

    public async Task CreateAsync(Domain.Entities.Order entity, CancellationToken cancellationToken = default)
    {
        context.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Domain.Entities.Order entity, CancellationToken cancellationToken = default)
    {
        context.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Domain.Entities.Order entity, CancellationToken cancellationToken = default)
    {
        context.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }
}