using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Inventory.Domain.AggregatesModel.ItemAggregate;

namespace TossErp.Inventory.Infrastructure.EntityConfigurations
{
    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.ToTable("Items");

            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id).ValueGeneratedOnAdd();

            builder.Property(i => i.ItemCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(i => i.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(i => i.Description)
                .HasMaxLength(1000);

            builder.Property(i => i.Barcode)
                .HasMaxLength(100);

            builder.Property(i => i.SKU)
                .HasMaxLength(100);

            builder.Property(i => i.ItemType)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(i => i.IsActive)
                .IsRequired();

            builder.Property(i => i.IsStockable)
                .IsRequired();

            builder.Property(i => i.IsSerialized)
                .IsRequired();

            builder.Property(i => i.IsBatched)
                .IsRequired();

            builder.Property(i => i.StandardCost)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(i => i.SellingPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(i => i.UnitOfMeasure)
                .HasMaxLength(20);

            builder.Property(i => i.MinimumStockLevel)
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.MaximumStockLevel)
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.ReorderPoint)
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.ReorderQuantity)
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.CategoryId);
            builder.Property(i => i.BrandId);
            builder.Property(i => i.SupplierId);

            builder.Property(i => i.CreatedAt)
                .IsRequired();

            builder.Property(i => i.LastModifiedAt);

            builder.HasIndex(i => i.ItemCode).IsUnique();
            builder.HasIndex(i => i.Barcode).IsUnique();
            builder.HasIndex(i => i.SKU).IsUnique();
            builder.HasIndex(i => i.Name);
            builder.HasIndex(i => i.IsActive);
            builder.HasIndex(i => i.ItemType);

            builder.HasMany(i => i.Variants)
                .WithOne()
                .HasForeignKey("ItemId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(i => i.PriceHistory)
                .WithOne()
                .HasForeignKey("ItemId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 
