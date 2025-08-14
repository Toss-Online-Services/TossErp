using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TossErp.Stock.Domain.Aggregates.StockEntryAggregate.Entities;
using TossErp.Stock.Domain.Aggregates.WarehouseAggregate.Entities;
using TossErp.Stock.Domain.ValueObjects;

namespace TossErp.Stock.Infrastructure.Data.Configurations;

public class StockEntryDetailConfiguration : IEntityTypeConfiguration<StockEntryDetail>
{
    public void Configure(EntityTypeBuilder<StockEntryDetail> builder)
    {
        builder.ToTable("StockEntryDetails");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        // Foreign Keys
        builder.Property(x => x.StockEntryId).IsRequired();
        builder.Property(x => x.ItemId).IsRequired();
        builder.Property(x => x.WarehouseId).IsRequired();
        builder.Property(x => x.BinId);

        // Value Objects
        builder.Property(x => x.Quantity)
            .HasConversion(
                v => v.Value,
                v => new Quantity(v, "PCS")) // Default unit
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.Rate)
            .HasConversion(
                v => v.Value,
                v => new Rate(v, "ZAR"))
            .HasPrecision(18, 4)
            .IsRequired();

        // Properties
        builder.Property(x => x.BatchNo).HasMaxLength(100);
        builder.Property(x => x.SerialNo).HasMaxLength(100);
        builder.Property(x => x.ExpiryDate);
        builder.Property(x => x.Remarks).HasMaxLength(500);
        builder.Property(x => x.IsValid).IsRequired();

        // Relationships
        builder.HasOne<StockEntryAggregate>()
            .WithMany(x => x.Details)
            .HasForeignKey(x => x.StockEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ItemAggregate>()
            .WithMany()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<WarehouseAggregate>()
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Bin>()
            .WithMany()
            .HasForeignKey(x => x.BinId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }
} 
