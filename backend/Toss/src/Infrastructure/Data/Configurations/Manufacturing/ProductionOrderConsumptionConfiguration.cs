using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Manufacturing;

namespace Toss.Infrastructure.Data.Configurations.Manufacturing;

public class ProductionOrderConsumptionConfiguration : IEntityTypeConfiguration<ProductionOrderConsumption>
{
    public void Configure(EntityTypeBuilder<ProductionOrderConsumption> builder)
    {
        builder.Property(c => c.Quantity)
            .HasPrecision(18, 4);

        // Relationships
        builder.HasOne(c => c.Business)
            .WithMany()
            .HasForeignKey(c => c.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.ProductionOrder)
            .WithMany(o => o.Consumed)
            .HasForeignKey(c => c.ProductionOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.ComponentProduct)
            .WithMany()
            .HasForeignKey(c => c.ComponentProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Shop)
            .WithMany()
            .HasForeignKey(c => c.ShopId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(c => c.BusinessId);
        builder.HasIndex(c => c.ProductionOrderId);
        builder.HasIndex(c => c.ComponentProductId);
        builder.HasIndex(c => c.ShopId);
    }
}

