using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Context.Configurations;

public class ProductAttributeMappingConfiguration : IEntityTypeConfiguration<ProductAttributeMapping>
{
    public void Configure(EntityTypeBuilder<ProductAttributeMapping> builder)
    {
        builder.ToTable("ProductAttributeMappings");

        builder.HasKey(pam => pam.Id);

        builder.Property(pam => pam.TextPrompt)
            .HasMaxLength(400);

        builder.HasMany(pam => pam.ProductAttributeValues)
            .WithOne(pav => pav.ProductAttributeMapping)
            .HasForeignKey(pav => pav.ProductAttributeMappingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pam => pam.Product)
            .WithMany(p => p.ProductAttributeMappings)
            .HasForeignKey(pam => pam.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pam => pam.ProductAttribute)
            .WithMany(pa => pa.ProductAttributeMappings)
            .HasForeignKey(pam => pam.ProductAttributeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 
