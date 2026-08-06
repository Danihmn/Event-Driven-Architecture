using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Order.Infrastructure.Data.Mapping;

public class OrderMapping : IEntityTypeConfiguration<Domain.Entities.Order>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(o => o.Id).HasName("pk_orders");

        builder.Property(o => o.Id).HasColumnName("id");
        builder.Property(o => o.ProductId).HasColumnName("product_id").IsRequired();
        builder.Property(o => o.Quantity).HasColumnName("quantity").IsRequired();
        builder.Property(o => o.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(o => o.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
    }
}