using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Sales;

namespace Toss.Infrastructure.Data.Configurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.Property(s => s.SaleNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.PaymentReference)
            .HasMaxLength(200);

        builder.Property(s => s.Notes)
            .HasMaxLength(1000);

        builder.Property(s => s.Subtotal)
            .HasPrecision(18, 2);

        builder.Property(s => s.TaxAmount)
            .HasPrecision(18, 2);

        builder.Property(s => s.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(s => s.Total)
            .HasPrecision(18, 2);

        builder.HasOne(s => s.Shop)
            .WithMany()
            .HasForeignKey(s => s.ShopId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Customer)
            .WithMany(c => c.Sales)
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(s => s.Items)
            .WithOne(i => i.Sale)
            .HasForeignKey(i => i.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Documents)
            .WithOne(d => d.Sale)
            .HasForeignKey(d => d.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => s.SaleNumber)
            .IsUnique();

        builder.HasIndex(s => s.SaleDate);
        builder.HasIndex(s => new { s.ShopId, s.SaleDate });
        builder.HasIndex(s => s.Status);
    }
}

