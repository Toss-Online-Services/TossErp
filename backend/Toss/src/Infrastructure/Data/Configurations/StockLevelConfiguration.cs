using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Catalog;

namespace Toss.Infrastructure.Data.Configurations;

public class StockLevelConfiguration : IEntityTypeConfiguration<StockLevel>
{
    public void Configure(EntityTypeBuilder<StockLevel> builder)
    {
        builder.Property(s => s.AverageCost)
            .HasPrecision(18, 2);

        builder.HasOne(s => s.Shop)
            .WithMany()
            .HasForeignKey(s => s.ShopId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Product)
            .WithMany(p => p.StockLevels)
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => new { s.ShopId, s.ProductId })
            .IsUnique();

        builder.HasIndex(s => s.ProductId);
    }
}

