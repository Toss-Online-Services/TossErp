using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Stock.Domain.Aggregates.WarehouseAggregate.Entities;
using TossErp.Stock.Domain.ValueObjects;

namespace TossErp.Stock.Infrastructure.Data.Configurations;

public class BinConfiguration : IEntityTypeConfiguration<Bin>
{
    public void Configure(EntityTypeBuilder<Bin> builder)
    {
        builder.ToTable("Bins");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        // Properties
        builder.Property(x => x.BinName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.BinType).HasMaxLength(50);
        builder.Property(x => x.Capacity).HasPrecision(18, 4);
        builder.Property(x => x.CapacityUOM).HasMaxLength(20);

        // Value Objects
        builder.Property(x => x.BinCode)
            .HasConversion(
                v => v.Value,
                v => new BinCode(v))
            .HasMaxLength(50)
            .IsRequired();

        // Indexes
        builder.HasIndex(x => x.BinCode).IsUnique();
        builder.HasIndex(x => x.BinName);
        builder.HasIndex(x => x.IsActive);
    }
} 
