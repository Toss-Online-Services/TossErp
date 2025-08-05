using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Stock.Domain.Entities;

namespace TossErp.Stock.Infrastructure.Data.Configurations;

public class BatchConfiguration : IEntityTypeConfiguration<Batch>
{
    public void Configure(EntityTypeBuilder<Batch> builder)
    {
        builder.ToTable("Batches");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        // Properties
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ItemCode).HasMaxLength(50).IsRequired();
        builder.Property(x => x.ExpiryDate);
        builder.Property(x => x.ManufacturingDate);
        builder.Property(x => x.WarrantyExpiryDate);
        builder.Property(x => x.Supplier).HasMaxLength(200);
        builder.Property(x => x.ReferenceDocumentType).HasMaxLength(50);
        builder.Property(x => x.ReferenceDocumentNo).HasMaxLength(100);
        builder.Property(x => x.ReferenceDocumentDetailNo).HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.Remarks).HasMaxLength(500);
        builder.Property(x => x.IsDisabled).IsRequired();

        // Decimal properties
        builder.Property(x => x.Quantity).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.TransferQuantity).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.ConsumedQuantity).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.DispatchedQuantity).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.ReturnedQuantity).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.ScrappedQuantity).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.RetainSample).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.RetainSampleQuantity).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.RetainSampleUOM).HasMaxLength(50).IsRequired();
        builder.Property(x => x.RetainSampleUOMQuantity).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.RetainSampleWarehouse).HasMaxLength(100).IsRequired();
        builder.Property(x => x.RetainSampleBin).HasMaxLength(100).IsRequired();

        // Indexes
        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.ItemCode);
        builder.HasIndex(x => x.ExpiryDate);
        builder.HasIndex(x => x.ManufacturingDate);
        builder.HasIndex(x => x.Supplier);
        builder.HasIndex(x => x.IsDisabled);

        // Relationships
        builder.HasOne(x => x.Item)
            .WithMany()
            .HasForeignKey(x => x.ItemCode)
            .HasPrincipalKey(x => x.ItemCode)
            .OnDelete(DeleteBehavior.Restrict);

        // TODO: Implement Batch-StockLedgerEntry relationship when needed
        // StockLedgerEntry has BatchNo (string) but not BatchId (foreign key)
    }
} 
