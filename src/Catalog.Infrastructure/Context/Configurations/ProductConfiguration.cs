using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Context.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(2000);

        builder.Property(p => p.Price)
            .HasPrecision(18, 2);

        builder.Property(p => p.PictureFileName)
            .HasMaxLength(200);

        // Relationships
        builder.HasOne(p => p.CatalogBrand)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.CatalogBrandId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.CatalogType)
            .WithMany(t => t.Products)
            .HasForeignKey(p => p.CatalogTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.ParentProduct)
            .WithMany(p => p.ChildProducts)
            .HasForeignKey(p => p.ParentProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Pictures)
            .WithOne(pp => pp.Product)
            .HasForeignKey(pp => pp.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.ProductReviews)
            .WithOne(pr => pr.Product)
            .HasForeignKey(pr => pr.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.ProductAttributeMappings)
            .WithOne(pam => pam.Product)
            .HasForeignKey(pam => pam.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.RelatedProducts)
            .WithOne(rp => rp.Product1)
            .HasForeignKey(rp => rp.ProductId1)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.CrossSellProducts)
            .WithOne(csp => csp.Product1)
            .HasForeignKey(csp => csp.ProductId1)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.ProductProductTagMappings)
            .WithOne(pptm => pptm.Product)
            .HasForeignKey(pptm => pptm.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.ProductTags)
            .WithMany(pt => pt.Products)
            .UsingEntity(j => j.ToTable("ProductProductTagMappings"));

        // Indexes
        builder.HasIndex(p => p.Name);
        builder.HasIndex(p => p.CatalogBrandId);
        builder.HasIndex(p => p.CatalogTypeId);
        builder.HasIndex(p => p.CategoryId);
        builder.HasIndex(p => p.ParentProductId);
    }
} 
