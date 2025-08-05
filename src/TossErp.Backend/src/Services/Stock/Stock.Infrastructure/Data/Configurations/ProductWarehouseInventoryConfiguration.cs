using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.ValueObjects;

namespace TossErp.Stock.Infrastructure.Data.Configurations;

public class ProductWarehouseInventoryConfiguration : IEntityTypeConfiguration<ProductWarehouseInventory>
{
    public void Configure(EntityTypeBuilder<ProductWarehouseInventory> builder)
    {
        builder.ToTable("ProductWarehouseInventories");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.ItemCode)
            .HasConversion(
                v => v.Value,
                v => new ItemCode(v))
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.WarehouseCode)
            .HasConversion(
                v => v.Value,
                v => new WarehouseCode(v))
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Quantity).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.ReservedQuantity).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();

        builder.HasIndex(x => x.ItemCode);
        builder.HasIndex(x => x.WarehouseCode);
        builder.HasIndex(x => new { x.ItemCode, x.WarehouseCode }).IsUnique();
    }
} 
