using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Vendors;

namespace Toss.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration for VendorPricing (replaces SupplierPricing)
/// </summary>
public class VendorPricingConfiguration : IEntityTypeConfiguration<VendorPricing>
{
    public void Configure(EntityTypeBuilder<VendorPricing> builder)
    {
        builder.HasKey(vp => vp.Id);

        builder.Property(vp => vp.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(vp => vp.DiscountPercentage)
            .HasPrecision(5, 2);

        builder.HasOne(vp => vp.VendorProduct)
            .WithMany(v => v.PricingTiers)
            .HasForeignKey(vp => vp.VendorProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(vp => vp.VendorProductId);
        builder.HasIndex(vp => vp.ValidFrom);
        builder.HasIndex(vp => new { vp.VendorProductId, vp.ValidFrom });
        builder.HasIndex(vp => vp.IsActive);
    }
}

