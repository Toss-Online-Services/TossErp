using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Catalog;

namespace Toss.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
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

        builder.Property(p => p.BasePrice)
            .HasPrecision(18, 2);

        builder.Property(p => p.CostPrice)
            .HasPrecision(18, 2);

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(p => p.SKU)
            .IsUnique();

        builder.HasIndex(p => p.Barcode);
        builder.HasIndex(p => p.CategoryId);
        builder.HasIndex(p => p.Name);
    }
}

