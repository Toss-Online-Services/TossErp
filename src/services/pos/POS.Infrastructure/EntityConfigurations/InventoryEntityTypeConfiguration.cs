using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.InventoryAggregate;

namespace POS.Infrastructure.EntityConfigurations;

public class InventoryEntityTypeConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.ToTable("Inventories", POSContext.DEFAULT_SCHEMA);

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .UseHiLo("inventoryseq", POSContext.DEFAULT_SCHEMA);

        builder.Property(i => i.StoreId)
            .IsRequired();

        builder.Property(i => i.ProductId)
            .IsRequired();

        builder.Property(i => i.Quantity)
            .IsRequired();

        builder.Property(i => i.Reason)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(i => i.CurrentStock)
            .IsRequired();

        builder.Property(i => i.MinimumStock)
            .IsRequired();

        builder.Property(i => i.MaximumStock)
            .IsRequired();

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.Property(i => i.LastModifiedAt);

        builder.Property(i => i.LotNumber)
            .HasMaxLength(50);

        builder.Property(i => i.ExpiryDate);

        builder.Property(i => i.SerialNumber)
            .HasMaxLength(50);

        builder.Property(i => i.UnitOfMeasure)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(i => i.UnitCost)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(i => i.Location)
            .HasMaxLength(100);

        builder.Property(i => i.BinNumber)
            .HasMaxLength(50);

        builder.Property(i => i.IsActive)
            .IsRequired();

        // Configure collections
        builder.HasMany(i => i.Movements)
            .WithOne()
            .HasForeignKey("InventoryId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(i => i.Reservations)
            .WithOne()
            .HasForeignKey("InventoryId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(i => i.Adjustments)
            .WithOne()
            .HasForeignKey("InventoryId")
            .OnDelete(DeleteBehavior.Cascade);

        // Configure indexes
        builder.HasIndex(i => new { i.ProductId, i.StoreId })
            .IsUnique();

        builder.HasIndex(i => i.IsActive);

        builder.HasIndex(i => i.ExpiryDate);

        builder.HasIndex(i => i.Location);

        builder.HasIndex(i => i.LotNumber);

        builder.HasIndex(i => i.SerialNumber);

        builder.HasIndex(i => i.UnitOfMeasure);

        builder.HasIndex(i => i.CreatedAt);
    }
} 
