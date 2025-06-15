using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.SaleAggregate;

namespace TossErp.POS.Infrastructure.EntityConfigurations;

public class SaleDiscountEntityTypeConfiguration : IEntityTypeConfiguration<SaleDiscount>
{
    public void Configure(EntityTypeBuilder<SaleDiscount> builder)
    {
        builder.ToTable("sale_discounts", "POS");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).HasConversion<string>();

        builder.Property(d => d.SaleId).HasConversion<string>().IsRequired();

        builder.Property(d => d.Name).HasMaxLength(100).IsRequired();
        builder.Property(d => d.Description).HasMaxLength(500);
        builder.Property(d => d.Type).HasMaxLength(20).IsRequired();
        builder.Property(d => d.Value).HasPrecision(18, 2).IsRequired();
        builder.Property(d => d.Amount).HasPrecision(18, 2).IsRequired();

        builder.HasIndex(d => d.SaleId);

        // Relationships
        builder.HasOne<Sale>()
            .WithMany(s => s.Discounts)
            .HasForeignKey(sd => sd.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 
