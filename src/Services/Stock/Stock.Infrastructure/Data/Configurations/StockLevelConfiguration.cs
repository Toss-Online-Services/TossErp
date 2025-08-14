using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Stock.Domain.Entities;

namespace TossErp.Stock.Infrastructure.Data.Configurations;

public class StockLevelConfiguration : IEntityTypeConfiguration<StockLevel>
{
    public void Configure(EntityTypeBuilder<StockLevel> builder)
    {
        builder.ToTable("StockLevels");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.ItemId)
            .IsRequired();

        builder.Property(x => x.WarehouseId)
            .IsRequired();

        builder.Property(x => x.BinId);

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.ReservedQuantity)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.UnitCost)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.LastMovementDate)
            .IsRequired();

        builder.Property(x => x.LastUpdated)
            .IsRequired();

        // Configure relationships
        builder.HasOne(x => x.Item)
            .WithMany()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Bin)
            .WithMany()
            .HasForeignKey(x => x.BinId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure indexes for performance
        builder.HasIndex(x => new { x.ItemId, x.WarehouseId })
            .IsUnique()
            .HasDatabaseName("IX_StockLevels_ItemId_WarehouseId");

        builder.HasIndex(x => x.ItemId)
            .HasDatabaseName("IX_StockLevels_ItemId");

        builder.HasIndex(x => x.WarehouseId)
            .HasDatabaseName("IX_StockLevels_WarehouseId");

        builder.HasIndex(x => x.LastUpdated)
            .HasDatabaseName("IX_StockLevels_LastUpdated");
    }
}
