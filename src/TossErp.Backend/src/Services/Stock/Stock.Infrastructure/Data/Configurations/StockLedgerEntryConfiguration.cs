using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.ValueObjects;

namespace TossErp.Stock.Infrastructure.Data.Configurations;

public class StockLedgerEntryConfiguration : IEntityTypeConfiguration<StockLedgerEntry>
{
    public void Configure(EntityTypeBuilder<StockLedgerEntry> builder)
    {
        builder.ToTable("StockLedgerEntries");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.ItemCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.WarehouseCode)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.BinName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Qty)
            .HasConversion(
                v => v.Value,
                v => new Quantity(v))
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.ValuationRate)
            .HasConversion(
                v => v.Value,
                v => new Rate(v, "ZAR"))
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.StockValue)
            .HasConversion(
                v => v.Value,
                v => new Rate(v, "ZAR"))
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.PostingDate).IsRequired();
        builder.Property(x => x.PostingTime).IsRequired();
        builder.Property(x => x.VoucherType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.VoucherNo).HasMaxLength(50).IsRequired();
        builder.Property(x => x.VoucherDetailNo).HasMaxLength(50);
        builder.Property(x => x.SerialNo).HasMaxLength(100);
        builder.Property(x => x.BatchNo).HasMaxLength(100);
        builder.Property(x => x.ExpiryDate);
        builder.Property(x => x.Project).HasMaxLength(100);
        builder.Property(x => x.CostCenter).HasMaxLength(100);
        builder.Property(x => x.Company).HasMaxLength(100);
        builder.Property(x => x.FiscalYear).HasMaxLength(10);
        builder.Property(x => x.StockUOM).HasMaxLength(20);
        builder.Property(x => x.ConversionFactor).HasPrecision(18, 4);
        builder.Property(x => x.ReferenceDocumentType).HasMaxLength(50);
        builder.Property(x => x.ReferenceDocumentNo).HasMaxLength(50);
        builder.Property(x => x.ReferenceDocumentDetailNo).HasMaxLength(50);
        builder.Property(x => x.Remarks).HasMaxLength(500);
        builder.Property(x => x.IsCancelled);
        builder.Property(x => x.CancelledDate);
        builder.Property(x => x.CancelledBy).HasMaxLength(100);
        builder.Property(x => x.CancellationReason).HasMaxLength(500);
        builder.Property(x => x.IsDisabled);

        builder.HasIndex(x => new { x.ItemCode, x.PostingDate });
    }
} 
