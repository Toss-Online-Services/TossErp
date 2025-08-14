using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Stock.Domain.Entities;

namespace TossErp.Stock.Infrastructure.Data.Configurations;

public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.ToTable("StockMovements");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.ItemId)
            .IsRequired();

        builder.Property(x => x.WarehouseId)
            .IsRequired();

        builder.Property(x => x.BinId);

        builder.Property(x => x.MovementType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.UnitCost)
            .HasPrecision(18, 4);

        builder.Property(x => x.ReferenceNumber)
            .HasMaxLength(100);

        builder.Property(x => x.ReferenceType)
            .HasMaxLength(50);

        builder.Property(x => x.MovementDate)
            .IsRequired();

        builder.Property(x => x.BatchId);

        builder.Property(x => x.Reason)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
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

        builder.HasOne(x => x.Batch)
            .WithMany()
            .HasForeignKey(x => x.BatchId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure indexes for performance
        builder.HasIndex(x => x.TenantId)
            .HasDatabaseName("IX_StockMovements_TenantId");

        builder.HasIndex(x => x.ItemId)
            .HasDatabaseName("IX_StockMovements_ItemId");

        builder.HasIndex(x => x.WarehouseId)
            .HasDatabaseName("IX_StockMovements_WarehouseId");

        builder.HasIndex(x => x.MovementType)
            .HasDatabaseName("IX_StockMovements_MovementType");

        builder.HasIndex(x => x.MovementDate)
            .HasDatabaseName("IX_StockMovements_MovementDate");

        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("IX_StockMovements_CreatedAt");

        builder.HasIndex(x => new { x.ItemId, x.WarehouseId, x.MovementDate })
            .HasDatabaseName("IX_StockMovements_ItemId_WarehouseId_MovementDate");

        builder.HasIndex(x => new { x.ReferenceType, x.ReferenceNumber })
            .HasDatabaseName("IX_StockMovements_ReferenceType_ReferenceNumber");

        builder.HasIndex(x => x.BatchId)
            .HasDatabaseName("IX_StockMovements_BatchId");

        // Configure audit fields
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(100)
            .IsRequired();
    }
}
