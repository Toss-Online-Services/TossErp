using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Context.Configurations;

public class ProductTagConfiguration : IEntityTypeConfiguration<ProductTag>
{
    public void Configure(EntityTypeBuilder<ProductTag> builder)
    {
        builder.ToTable("ProductTags");

        builder.HasKey(pt => pt.Id);

        builder.Property(pt => pt.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pt => pt.MetaDescription)
            .HasMaxLength(400);

        builder.Property(pt => pt.MetaKeywords)
            .HasMaxLength(400);

        builder.Property(pt => pt.MetaTitle)
            .HasMaxLength(400);

        // Relationships
        builder.HasMany(pt => pt.Products)
            .WithMany(p => p.ProductTags)
            .UsingEntity(j => j.ToTable("ProductProductTagMapping"));

        // Indexes
        builder.HasIndex(pt => pt.Name);
    }
} 
