using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Context.Configurations;

public class ProductAttributeValueConfiguration : IEntityTypeConfiguration<ProductAttributeValue>
{
    public void Configure(EntityTypeBuilder<ProductAttributeValue> builder)
    {
        builder.ToTable("ProductAttributeValues");

        builder.HasKey(pav => pav.Id);

        builder.Property(pav => pav.Name)
            .IsRequired()
            .HasMaxLength(400);

        builder.Property(pav => pav.ColorSquaresRgb)
            .HasMaxLength(100);

        builder.Property(pav => pav.PriceAdjustment)
            .HasPrecision(18, 4);

        builder.Property(pav => pav.WeightAdjustment)
            .HasPrecision(18, 4);

        builder.HasOne(pav => pav.ProductAttributeMapping)
            .WithMany(pam => pam.ProductAttributeValues)
            .HasForeignKey(pav => pav.ProductAttributeMappingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 
