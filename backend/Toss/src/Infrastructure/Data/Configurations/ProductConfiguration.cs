using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Catalog;

namespace Toss.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for the Product entity.
/// Defines table schema, constraints, indexes, and relationships.
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Required fields with length constraints
        builder.Property(p => p.SKU)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Barcode)
            .HasMaxLength(100);

        builder.Property(p => p.Name)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(2000);

        builder.Property(p => p.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(p => p.Unit)
            .HasMaxLength(50);

        // Decimal precision for monetary values
        builder.Property(p => p.BasePrice)
            .HasPrecision(18, 2);

        builder.Property(p => p.CostPrice)
            .HasPrecision(18, 2);

        // Relationships
        builder.HasOne(p => p.Business)
            .WithMany()
            .HasForeignKey(p => p.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull); // Allow products without category

        // Indexes for performance
        builder.HasIndex(p => new { p.BusinessId, p.SKU })
            .IsUnique();

        builder.HasIndex(p => p.Barcode); // For barcode scanning
        builder.HasIndex(p => p.CategoryId); // For category filtering
        builder.HasIndex(p => p.Name); // For product search
        builder.HasIndex(p => p.BusinessId);
    }
}

