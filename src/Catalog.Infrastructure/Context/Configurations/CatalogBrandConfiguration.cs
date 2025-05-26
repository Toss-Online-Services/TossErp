using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Context.Configurations;

public class CatalogBrandConfiguration : IEntityTypeConfiguration<CatalogBrand>
{
    public void Configure(EntityTypeBuilder<CatalogBrand> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Brand).IsRequired().HasMaxLength(100);
        builder.HasMany(e => e.Products)
            .WithOne(p => p.CatalogBrand)
            .HasForeignKey(p => p.CatalogBrandId)
            .OnDelete(DeleteBehavior.Restrict);
    }
} 
