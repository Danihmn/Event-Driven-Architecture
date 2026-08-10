using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Order.Infrastructure.Data.Context;

public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
{
    public DbSet<Domain.Entities.Order> Orders { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder
                .UseNpgsql("Name=ConnectionStrings:orderdb")
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
}