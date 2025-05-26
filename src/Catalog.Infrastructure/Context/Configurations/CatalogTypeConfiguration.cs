using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Context.Configurations;

public class CatalogTypeConfiguration : IEntityTypeConfiguration<CatalogType>
{
    public void Configure(EntityTypeBuilder<CatalogType> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Type).IsRequired().HasMaxLength(100);
        builder.HasMany(e => e.Products)
            .WithOne(p => p.CatalogType)
            .HasForeignKey(p => p.CatalogTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
} 
