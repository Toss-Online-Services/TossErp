using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.SaleAggregate;

namespace POS.Infrastructure.EntityConfigurations;

public class SaleItemEntityTypeConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems", "POS");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();

        builder.Property(x => x.SaleId).IsRequired();
        builder.Property(x => x.ProductId).IsRequired();
        builder.Property(x => x.ProductName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Category).HasMaxLength(100);
        builder.Property(x => x.Barcode).HasMaxLength(50);
        builder.Property(x => x.Variant).HasMaxLength(100);
        builder.Property(x => x.Quantity).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.UnitPrice).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.TaxRate).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.SubTotal).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Discount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt);

        builder.HasIndex(x => x.SaleId);
        builder.HasIndex(x => x.ProductId);
        builder.HasIndex(x => x.Barcode);

        builder.HasOne(x => x.Sale)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
} 
