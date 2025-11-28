using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Manufacturing;
using Toss.Domain.Enums;

namespace Toss.Infrastructure.Data.Configurations.Manufacturing;

public class ProductionOrderConfiguration : IEntityTypeConfiguration<ProductionOrder>
{
    public void Configure(EntityTypeBuilder<ProductionOrder> builder)
    {
        builder.Property(o => o.Notes)
            .HasMaxLength(1000);

        builder.Property(o => o.Status)
            .HasConversion<int>();

        // Relationships
        builder.HasOne(o => o.Business)
            .WithMany()
            .HasForeignKey(o => o.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.Product)
            .WithMany()
            .HasForeignKey(o => o.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Shop)
            .WithMany()
            .HasForeignKey(o => o.ShopId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Consumed)
            .WithOne(c => c.ProductionOrder)
            .HasForeignKey(c => c.ProductionOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.Produced)
            .WithOne(p => p.ProductionOrder)
            .HasForeignKey(p => p.ProductionOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(o => o.BusinessId);
        builder.HasIndex(o => new { o.BusinessId, o.ProductId });
        builder.HasIndex(o => o.ShopId);
        builder.HasIndex(o => o.Status);
    }
}

