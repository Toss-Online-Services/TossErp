using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Orders;

namespace Toss.Infrastructure.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderItemGuid);

        builder.Property(x => x.UnitPriceExclTax)
            .HasPrecision(18, 4);

        builder.Property(x => x.UnitPriceInclTax)
            .HasPrecision(18, 4);

        builder.Property(x => x.PriceExclTax)
            .HasPrecision(18, 4);

        builder.Property(x => x.PriceInclTax)
            .HasPrecision(18, 4);

        builder.Property(x => x.DiscountAmountExclTax)
            .HasPrecision(18, 4);

        builder.Property(x => x.DiscountAmountInclTax)
            .HasPrecision(18, 4);

        builder.HasOne(x => x.Order)
            .WithMany(x => x.OrderItems)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

