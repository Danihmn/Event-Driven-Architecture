using Microsoft.EntityFrameworkCore;

namespace Order.Infrastructure.Data;

public class OrderDbContext : DbContext
{
    public DbSet<Domain.Entities.Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
}