using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Suppliers;

namespace Toss.Infrastructure.Data.Configurations;

public class SupplierPricingConfiguration : IEntityTypeConfiguration<SupplierPricing>
{
    public void Configure(EntityTypeBuilder<SupplierPricing> builder)
    {
        builder.Property(p => p.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(p => p.DiscountPercentage)
            .HasPrecision(5, 2);

        builder.HasIndex(p => p.SupplierProductId);
        builder.HasIndex(p => new { p.ValidFrom, p.ValidTo });
    }
}

