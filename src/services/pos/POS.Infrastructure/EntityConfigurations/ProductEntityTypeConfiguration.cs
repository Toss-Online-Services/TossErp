using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.ProductAggregate;
using POS.Domain.AggregatesModel.StoreAggregate;

namespace POS.Infrastructure.EntityConfigurations;

public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products", POSContext.DEFAULT_SCHEMA);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();

        builder.Property(x => x.StoreId).IsRequired();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.Barcode).HasMaxLength(50);
        builder.Property(x => x.SKU).HasMaxLength(50);
        builder.Property(x => x.Category).HasMaxLength(100);
        builder.Property(x => x.Price).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Cost).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.TaxRate).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Stock).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.MinStock).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.MaxStock).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Unit).HasMaxLength(50);
        builder.Property(x => x.Brand).HasMaxLength(100);
        builder.Property(x => x.Supplier).HasMaxLength(200);
        builder.Property(x => x.ImageUrl).HasMaxLength(500);
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.IsSynced).IsRequired();
        builder.Property(x => x.SyncedAt);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt);

        builder.HasIndex(x => x.StoreId);
        builder.HasIndex(x => x.Barcode).IsUnique();
        builder.HasIndex(x => x.SKU).IsUnique();
        builder.HasIndex(x => x.Category);
        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.IsSynced);

        builder.HasOne(x => x.Store)
            .WithMany()
            .HasForeignKey(x => x.StoreId)
            .OnDelete(DeleteBehavior.Restrict);
    }
} 
