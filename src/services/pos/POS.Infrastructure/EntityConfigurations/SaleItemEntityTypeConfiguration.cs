using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.SaleAggregate;

namespace TossErp.POS.Infrastructure.EntityConfigurations;

public class SaleItemEntityTypeConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("sale_items", "POS");

        builder.HasKey(si => si.Id);
        builder.Property(si => si.Id)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(si => si.SaleId)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(si => si.ProductId)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(si => si.ProductName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(si => si.Quantity)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(si => si.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(si => si.TaxRate)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(si => si.SubTotal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(si => si.TaxAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(si => si.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(si => si.Discount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(si => si.CreatedAt)
            .IsRequired();

        builder.Property(si => si.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(si => si.SaleId);
        builder.HasIndex(si => si.ProductId);

        // Relationships
        builder.HasOne<Sale>()
            .WithMany(s => s.Items)
            .HasForeignKey(si => si.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 
