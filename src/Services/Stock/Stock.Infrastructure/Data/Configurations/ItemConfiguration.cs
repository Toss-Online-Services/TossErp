using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Stock.Domain.Aggregates.ItemAggregate;
using TossErp.Stock.Domain.ValueObjects;

namespace TossErp.Stock.Infrastructure.Data.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<ItemAggregate>
{
    public void Configure(EntityTypeBuilder<ItemAggregate> builder)
    {
        builder.ToTable("Items");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        // Value Objects
        builder.Property(x => x.ItemCode)
            .HasConversion(
                v => v.Value,
                v => new ItemCode(v))
            .HasMaxLength(50)
            .IsRequired();

        // Enums
        builder.Property(x => x.ItemType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.ValuationMethod)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.ItemStatus)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.PriorityLevel)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // Properties
        builder.Property(x => x.ItemName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.ItemGroup).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Brand).HasMaxLength(100);
        builder.Property(x => x.Company).HasMaxLength(100).IsRequired();

        // Decimal properties
        builder.Property(x => x.StandardRate).HasPrecision(18, 4);
        builder.Property(x => x.LastPurchaseRate).HasPrecision(18, 4);
        builder.Property(x => x.BaseRate).HasPrecision(18, 4);
        builder.Property(x => x.MinimumPrice).HasPrecision(18, 4);
        builder.Property(x => x.WeightPerUnit).HasPrecision(18, 4);
        builder.Property(x => x.WeightUOM).HasPrecision(18, 4);
        builder.Property(x => x.ReOrderLevel).HasPrecision(18, 4);
        builder.Property(x => x.ReOrderQty).HasPrecision(18, 4);
        builder.Property(x => x.MaxQty).HasPrecision(18, 4);
        builder.Property(x => x.MinQty).HasPrecision(18, 4);
        builder.Property(x => x.Length).HasPrecision(18, 4);
        builder.Property(x => x.Width).HasPrecision(18, 4);
        builder.Property(x => x.Height).HasPrecision(18, 4);

        // Boolean properties
        builder.Property(x => x.IsStockItem).IsRequired();
        builder.Property(x => x.Disabled).IsRequired();
        builder.Property(x => x.Deleted).IsRequired();

        // Indexes
        builder.HasIndex(x => x.ItemCode).IsUnique();
        builder.HasIndex(x => x.ItemName);
        builder.HasIndex(x => x.ItemGroup);
        builder.HasIndex(x => x.Brand);
        builder.HasIndex(x => x.ItemType);
        builder.HasIndex(x => x.IsStockItem);
        builder.HasIndex(x => x.Disabled);
        builder.HasIndex(x => x.Company);

        // Relationships - Configured as owned entities within the aggregate
        builder.OwnsMany(x => x.Variants, variant =>
        {
            variant.WithOwner().HasForeignKey("ItemId");
            variant.Property(x => x.VariantCode).HasMaxLength(50).IsRequired();
            variant.Property(x => x.VariantName).HasMaxLength(100).IsRequired();
            variant.Property(x => x.Description).HasMaxLength(500);
            variant.Property(x => x.AdditionalCost).HasPrecision(18, 4);
            variant.Property(x => x.IsActive).IsRequired();
        });

        builder.OwnsMany(x => x.Prices, price =>
        {
            price.WithOwner().HasForeignKey("ItemId");
            price.Property(x => x.PriceList).HasMaxLength(50).IsRequired();
            price.Property(x => x.Currency).HasMaxLength(3).IsRequired();
            price.Property(x => x.Rate).HasPrecision(18, 4).IsRequired();
        });

        builder.OwnsMany(x => x.Suppliers, supplier =>
        {
            supplier.WithOwner().HasForeignKey("ItemId");
            supplier.Property(x => x.SupplierId).IsRequired();
            supplier.Property(x => x.SupplierItemCode).HasMaxLength(50).IsRequired();
            supplier.Property(x => x.SupplierItemName).HasMaxLength(100);
            supplier.Property(x => x.LastPurchaseRate).HasPrecision(18, 4);
            supplier.Property(x => x.MinimumOrderQty).HasPrecision(18, 4);
            supplier.Property(x => x.LeadTimeDays);
            supplier.Property(x => x.IsPreferred).IsRequired();
            supplier.Property(x => x.IsActive).IsRequired();
        });

        builder.OwnsMany(x => x.Barcodes, barcode =>
        {
            barcode.WithOwner().HasForeignKey("ItemId");
            barcode.Property(x => x.Barcode).HasMaxLength(100).IsRequired();
            barcode.Property(x => x.BarcodeType).HasMaxLength(50).IsRequired();
        });

        builder.OwnsMany(x => x.Taxes, tax =>
        {
            tax.WithOwner().HasForeignKey("ItemId");
            tax.Property(x => x.TaxCode).HasMaxLength(50).IsRequired();
            tax.Property(x => x.TaxRate).HasPrecision(18, 4).IsRequired();
        });

        builder.HasMany(x => x.ReorderLevels)
            .WithOne()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Attributes)
            .WithOne()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Alternatives)
            .WithOne()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Manufacturers)
            .WithOne()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // TODO: Add WebsiteSpecifications and QualityInspectionParameters when they are implemented

        builder.HasMany(x => x.UOMConversions)
            .WithOne()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 
