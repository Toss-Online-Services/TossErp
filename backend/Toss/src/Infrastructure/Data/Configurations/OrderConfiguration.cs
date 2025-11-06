using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Orders;

namespace Toss.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for the Order entity.
/// Configures order properties, relationships with addresses and items, and cascade behaviors.
/// </summary>
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // Primary key
        builder.HasKey(x => x.Id);

        // GUID for external tracking
        builder.Property(x => x.OrderGuid);

        // Customer IP for fraud detection and analytics
        builder.Property(x => x.CustomerIp)
            .HasMaxLength(45); // IPv6 max length

        // Monetary values with 4 decimal places for precision
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

        // Payment and shipping details
        builder.Property(x => x.PaymentMethodSystemName)
            .HasMaxLength(400);

        builder.Property(x => x.ShippingMethod)
            .HasMaxLength(400);

        builder.Property(x => x.CustomerCurrencyCode)
            .HasMaxLength(5); // ISO 4217 currency code

        // Address relationships - Restrict to prevent accidental deletion
        builder.HasOne(x => x.BillingAddress)
            .WithMany()
            .HasForeignKey(x => x.BillingAddressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ShippingAddress)
            .WithMany()
            .HasForeignKey(x => x.ShippingAddressId)
            .OnDelete(DeleteBehavior.Restrict);

        // Order items - Cascade delete when order is deleted
        builder.HasMany(x => x.OrderItems)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Order notes - Cascade delete when order is deleted
        builder.HasMany(x => x.OrderNotes)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

