using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Order.Infrastructure.Data.Mapping;

public class OrderMapping : IEntityTypeConfiguration<Domain.Entities.Order>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(o => o.Id).HasName("pk_order");

        builder.Property(o => o.Quantity).IsRequired();
        builder.Property(o => o.Status).HasConversion<string>();
        builder.Property(o => o.CreatedAt).HasDefaultValue(DateTime.UtcNow);
    }
}