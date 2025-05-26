using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Context.Configurations;

public class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
{
    public void Configure(EntityTypeBuilder<ProductAttribute> builder)
    {
        builder.ToTable("ProductAttributes");

        builder.HasKey(pa => pa.Id);

        builder.Property(pa => pa.Name)
            .IsRequired()
            .HasMaxLength(400);

        builder.Property(pa => pa.Description)
            .HasMaxLength(4000);

        builder.HasMany(pa => pa.ProductAttributeMappings)
            .WithOne(pam => pam.ProductAttribute)
            .HasForeignKey(pam => pam.ProductAttributeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 
