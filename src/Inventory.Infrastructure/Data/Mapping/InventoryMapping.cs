using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Data.Mapping;

public class InventoryMapping : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(p => p.Id).HasName("pk_products");

        builder.Property(p => p.Name).HasMaxLength(50).IsRequired();
        builder.Property(p => p.Price).HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(p => p.QuantityAvailable).HasColumnName("quantity_available").IsRequired();
        builder.Property(p => p.CreatedAt).HasDefaultValue(DateTime.UtcNow);
    }
}