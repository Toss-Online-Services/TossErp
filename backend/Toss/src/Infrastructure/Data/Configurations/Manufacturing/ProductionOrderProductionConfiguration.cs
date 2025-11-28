using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Manufacturing;

namespace Toss.Infrastructure.Data.Configurations.Manufacturing;

public class ProductionOrderProductionConfiguration : IEntityTypeConfiguration<ProductionOrderProduction>
{
    public void Configure(EntityTypeBuilder<ProductionOrderProduction> builder)
    {
        // Relationships
        builder.HasOne(p => p.Business)
            .WithMany()
            .HasForeignKey(p => p.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.ProductionOrder)
            .WithMany(o => o.Produced)
            .HasForeignKey(p => p.ProductionOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Product)
            .WithMany()
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Shop)
            .WithMany()
            .HasForeignKey(p => p.ShopId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(p => p.BusinessId);
        builder.HasIndex(p => p.ProductionOrderId);
        builder.HasIndex(p => p.ProductId);
        builder.HasIndex(p => p.ShopId);
    }
}

