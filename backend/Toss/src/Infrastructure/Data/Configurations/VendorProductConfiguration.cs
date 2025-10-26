using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Vendors;

namespace Toss.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration for VendorProduct (replaces SupplierProduct)
/// </summary>
public class VendorProductConfiguration : IEntityTypeConfiguration<VendorProduct>
{
    public void Configure(EntityTypeBuilder<VendorProduct> builder)
    {
        builder.HasKey(vp => vp.Id);

        builder.Property(vp => vp.VendorSKU)
            .HasMaxLength(100);

        builder.Property(vp => vp.BasePrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(vp => vp.LeadTimeDays)
            .IsRequired();

        builder.Property(vp => vp.MinOrderQuantity)
            .IsRequired();

        builder.HasOne(vp => vp.Vendor)
            .WithMany(v => v.VendorProducts)
            .HasForeignKey(vp => vp.VendorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(vp => vp.Product)
            .WithMany()
            .HasForeignKey(vp => vp.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(vp => new { vp.VendorId, vp.ProductId })
            .IsUnique();

        builder.HasIndex(vp => vp.ProductId);
        builder.HasIndex(vp => vp.IsActive);
    }
}

