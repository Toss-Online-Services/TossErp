using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.ProductAggregate;

namespace TossErp.POS.Infrastructure.EntityConfigurations;

public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products", "POS");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(p => p.StoreId)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(p => p.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.SKU)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Barcode)
            .HasMaxLength(50);

        builder.Property(p => p.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.TaxRate)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(p => p.StockQuantity)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.MinStockLevel)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.MaxStockLevel)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.Status)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(p => p.StoreId);
        builder.HasIndex(p => new { p.StoreId, p.Code }).IsUnique();
        builder.HasIndex(p => new { p.StoreId, p.SKU }).IsUnique();
        builder.HasIndex(p => p.Barcode);
        builder.HasIndex(p => p.Status);

        // Relationships
        builder.HasOne<Store>()
            .WithMany()
            .HasForeignKey(p => p.StoreId)
            .OnDelete(DeleteBehavior.Restrict);
    }
} 
