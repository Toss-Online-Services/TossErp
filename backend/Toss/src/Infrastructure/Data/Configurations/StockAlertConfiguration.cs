using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Catalog;

namespace Toss.Infrastructure.Data.Configurations;

public class StockAlertConfiguration : IEntityTypeConfiguration<StockAlert>
{
    public void Configure(EntityTypeBuilder<StockAlert> builder)
    {
        builder.HasOne(a => a.Shop)
            .WithMany()
            .HasForeignKey(a => a.ShopId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Product)
            .WithMany()
            .HasForeignKey(a => a.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => new { a.ShopId, a.IsAcknowledged });
        builder.HasIndex(a => a.ProductId);
    }
}

