using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Catalog;

namespace Toss.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for the StockLevel entity.
/// Tracks inventory levels per shop/product combination with unique constraints.
/// </summary>
public class StockLevelConfiguration : IEntityTypeConfiguration<StockLevel>
{
    public void Configure(EntityTypeBuilder<StockLevel> builder)
    {
        // Monetary tracking for cost accounting
        builder.Property(s => s.AverageCost)
            .HasPrecision(18, 2);

        // Relationships
        builder.HasOne(s => s.Shop)
            .WithMany()
            .HasForeignKey(s => s.ShopId)
            .OnDelete(DeleteBehavior.Cascade); // Delete stock levels if shop is deleted

        builder.HasOne(s => s.Product)
            .WithMany(p => p.StockLevels)
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Cascade); // Delete stock levels if product is deleted

        // Indexes and constraints
        builder.HasIndex(s => new { s.ShopId, s.ProductId })
            .IsUnique(); // Enforce one stock level per shop/product combination

        builder.HasIndex(s => s.ProductId); // For product-wide stock queries
    }
}

