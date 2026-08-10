using Inventory.Domain.Entities;
using Inventory.Domain.Repository;
using Inventory.Infrastructure.Data;
using Inventory.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repository;

public class ProductRepository(InventoryDbContext context) : IProductRepository
{
    public async Task<IEnumerable<Product>?> GetAllAsync(int skip, int take,
        CancellationToken cancellationToken = default)
        => await context.Products.AsNoTracking().Skip(skip).Take(take).ToListAsync(cancellationToken);

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Products.FindAsync([id], cancellationToken);

    public async Task CreateAsync(Product entity, CancellationToken cancellationToken = default)
    {
        context.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Product entity, CancellationToken cancellationToken = default)
    {
        context.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Product entity, CancellationToken cancellationToken = default)
    {
        context.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }
}