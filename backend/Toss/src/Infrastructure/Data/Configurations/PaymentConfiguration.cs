using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Payments;

namespace Toss.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for the Payment entity.
/// Configures mobile money payments (MTN, Airtel, M-Pesa) with status tracking and audit trail.
/// </summary>
public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        // Unique payment identifier
        builder.Property(p => p.PaymentReference)
            .HasMaxLength(200)
            .IsRequired();

        // Monetary value with 2 decimal places
        builder.Property(p => p.Amount)
            .HasPrecision(18, 2);

        // ISO 4217 currency code (e.g., UGX, KES, TZS)
        builder.Property(p => p.Currency)
            .HasMaxLength(3)
            .IsRequired();

        // Source entity type (e.g., "Sale", "Order", "Invoice")
        builder.Property(p => p.SourceType)
            .HasMaxLength(50);

        // Customer information
        builder.Property(p => p.PayerName)
            .HasMaxLength(200);

        builder.Property(p => p.PayerPhone)
            .HasMaxLength(20);

        builder.Property(p => p.PayerEmail)
            .HasMaxLength(256);

        // Payment gateway tracking
        builder.Property(p => p.GatewayReference)
            .HasMaxLength(200);

        builder.Property(p => p.FailureReason)
            .HasMaxLength(500);

        builder.Property(p => p.Notes)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(p => p.Shop)
            .WithMany()
            .HasForeignKey(p => p.ShopId)
            .OnDelete(DeleteBehavior.Cascade); // Delete payments if shop is deleted

        builder.HasOne(p => p.Customer)
            .WithMany()
            .HasForeignKey(p => p.CustomerId)
            .OnDelete(DeleteBehavior.SetNull); // Keep payment if customer is deleted

        builder.HasOne(p => p.PayLink)
            .WithMany(pl => pl.Payments)
            .HasForeignKey(p => p.PayLinkId)
            .OnDelete(DeleteBehavior.SetNull); // Keep payment if paylink is deleted

        // Indexes for performance and constraints
        builder.HasIndex(p => p.PaymentReference)
            .IsUnique(); // Enforce unique payment references

        builder.HasIndex(p => p.Status); // For filtering by status
        builder.HasIndex(p => p.InitiatedAt); // For date-based queries
        builder.HasIndex(p => new { p.SourceType, p.SourceId }); // Composite for polymorphic lookups
    }
}

