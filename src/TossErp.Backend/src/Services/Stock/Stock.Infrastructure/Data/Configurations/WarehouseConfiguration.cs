using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Stock.Domain.Aggregates.WarehouseAggregate;
using TossErp.Stock.Domain.ValueObjects;

namespace TossErp.Stock.Infrastructure.Data.Configurations;

public class WarehouseConfiguration : IEntityTypeConfiguration<WarehouseAggregate>
{
    public void Configure(EntityTypeBuilder<WarehouseAggregate> builder)
    {
        builder.ToTable("Warehouses");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        // Value Object: Code
        builder.Property(x => x.Code)
            .HasConversion(
                v => v.Value,
                v => new TossErp.Stock.Domain.ValueObjects.WarehouseCode(v))
            .HasMaxLength(50)
            .IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();

        // Properties
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.WarehouseType).HasMaxLength(100);
        builder.Property(x => x.Company).HasMaxLength(100).IsRequired();
        builder.Property(x => x.City).HasMaxLength(100);
        builder.Property(x => x.State).HasMaxLength(100);
        builder.Property(x => x.Country).HasMaxLength(100);
        builder.Property(x => x.Pin).HasMaxLength(20);
        builder.Property(x => x.EmailId).HasMaxLength(100);
        builder.Property(x => x.PhoneNo).HasMaxLength(20);
        builder.Property(x => x.MobileNo).HasMaxLength(20);
        builder.Property(x => x.IsGroup).IsRequired();
        builder.Property(x => x.IsDisabled).IsRequired();
        builder.Property(x => x.Lft);
        builder.Property(x => x.Rgt);

        // Remove navigation mapping for Bins (no Warehouse navigation on Bin)
    }
} 
