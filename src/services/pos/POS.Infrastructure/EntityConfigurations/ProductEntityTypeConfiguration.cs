using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.ProductAggregate;

namespace POS.Infrastructure.EntityConfigurations;

public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products", POSContext.DEFAULT_SCHEMA);

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .UseHiLo("productseq", POSContext.DEFAULT_SCHEMA);

        builder.Property(p => p.StoreId)
            .IsRequired();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.SKU)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Barcode)
            .HasMaxLength(50);

        // Configure Price value object
        builder.OwnsOne(p => p.Price, money =>
        {
            money.Property(m => m.Amount)
                .IsRequired()
                .HasPrecision(18, 2);
            money.Property(m => m.Currency)
                .IsRequired()
                .HasMaxLength(3);
        });

        builder.Property(p => p.CostPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.CategoryId)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .IsRequired();

        builder.Property(p => p.StockQuantity)
            .IsRequired();

        builder.Property(p => p.LowStockThreshold)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.LastModifiedAt);

        // Configure navigation properties
        builder.HasOne(p => p.Store)
            .WithMany()
            .HasForeignKey(p => p.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure indexes
        builder.HasIndex(p => p.SKU)
            .IsUnique();

        builder.HasIndex(p => p.Barcode)
            .IsUnique();

        builder.HasIndex(p => p.StoreId);

        builder.HasIndex(p => p.CategoryId);

        builder.HasIndex(p => p.IsActive);

        builder.HasIndex(p => p.Name);

        builder.HasIndex(p => p.LastModifiedAt);

        builder.HasIndex(p => p.StockQuantity);

        builder.HasIndex(p => p.CostPrice);
    }
} 
