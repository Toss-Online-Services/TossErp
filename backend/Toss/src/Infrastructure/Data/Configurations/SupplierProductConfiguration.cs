using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Suppliers;

namespace Toss.Infrastructure.Data.Configurations;

public class SupplierProductConfiguration : IEntityTypeConfiguration<SupplierProduct>
{
    public void Configure(EntityTypeBuilder<SupplierProduct> builder)
    {
        builder.Property(sp => sp.SupplierSKU)
            .HasMaxLength(100);

        builder.Property(sp => sp.BasePrice)
            .HasPrecision(18, 2);

        builder.Property(sp => sp.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.HasOne(sp => sp.Supplier)
            .WithMany(s => s.SupplierProducts)
            .HasForeignKey(sp => sp.SupplierId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sp => sp.Product)
            .WithMany()
            .HasForeignKey(sp => sp.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(sp => sp.PricingTiers)
            .WithOne(pt => pt.SupplierProduct)
            .HasForeignKey(pt => pt.SupplierProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(sp => new { sp.SupplierId, sp.ProductId })
            .IsUnique();

        builder.HasIndex(sp => sp.ProductId);
    }
}

