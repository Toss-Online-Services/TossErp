using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Orders;

namespace Toss.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderGuid);

        builder.Property(x => x.CustomerIp)
            .HasMaxLength(45);

        builder.Property(x => x.OrderSubtotalExclTax)
            .HasPrecision(18, 4);

        builder.Property(x => x.OrderSubtotalInclTax)
            .HasPrecision(18, 4);

        builder.Property(x => x.OrderTax)
            .HasPrecision(18, 4);

        builder.Property(x => x.OrderTotal)
            .HasPrecision(18, 4);

        builder.Property(x => x.RefundedAmount)
            .HasPrecision(18, 4);

        builder.Property(x => x.PaymentMethodSystemName)
            .HasMaxLength(400);

        builder.Property(x => x.ShippingMethod)
            .HasMaxLength(400);

        builder.Property(x => x.CustomerCurrencyCode)
            .HasMaxLength(5);

        builder.HasOne(x => x.BillingAddress)
            .WithMany()
            .HasForeignKey(x => x.BillingAddressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ShippingAddress)
            .WithMany()
            .HasForeignKey(x => x.ShippingAddressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.OrderItems)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.OrderNotes)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

