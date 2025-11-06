using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Sales;

namespace Toss.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for the Sale entity.
/// Configures POS sale transactions with proper indexes and relationships.
/// </summary>
public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        // Required fields
        builder.Property(s => s.SaleNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.PaymentReference)
            .HasMaxLength(200);

        builder.Property(s => s.Notes)
            .HasMaxLength(1000);

        // Monetary values with 2 decimal places
        builder.Property(s => s.Subtotal)
            .HasPrecision(18, 2);

        builder.Property(s => s.TaxAmount)
            .HasPrecision(18, 2);

        builder.Property(s => s.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(s => s.Total)
            .HasPrecision(18, 2);

        // Relationships
        builder.HasOne(s => s.Shop)
            .WithMany()
            .HasForeignKey(s => s.ShopId)
            .OnDelete(DeleteBehavior.Cascade); // Delete sales if shop is deleted

        builder.HasOne(s => s.Customer)
            .WithMany(c => c.Sales)
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.SetNull); // Keep sale if customer is deleted (anonymous sale)

        builder.HasMany(s => s.Items)
            .WithOne(i => i.Sale)
            .HasForeignKey(i => i.SaleId)
            .OnDelete(DeleteBehavior.Cascade); // Delete items with sale

        builder.HasMany(s => s.Documents)
            .WithOne(d => d.Sale)
            .HasForeignKey(d => d.SaleId)
            .OnDelete(DeleteBehavior.Cascade); // Delete documents with sale

        // Indexes for performance
        builder.HasIndex(s => s.SaleNumber)
            .IsUnique(); // Enforce unique sale numbers

        builder.HasIndex(s => s.SaleDate); // For date range queries
        builder.HasIndex(s => new { s.ShopId, s.SaleDate }); // Composite for shop sales reports
        builder.HasIndex(s => s.Status); // For filtering by status
    }
}

