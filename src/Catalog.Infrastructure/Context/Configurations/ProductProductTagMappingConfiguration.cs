using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Context.Configurations;

public class ProductProductTagMappingConfiguration : IEntityTypeConfiguration<ProductProductTagMapping>
{
    public void Configure(EntityTypeBuilder<ProductProductTagMapping> builder)
    {
        builder.ToTable("ProductProductTagMappings");

        builder.HasKey(pptm => pptm.Id);

        builder.HasOne(pptm => pptm.Product)
            .WithMany(p => p.ProductProductTagMappings)
            .HasForeignKey(pptm => pptm.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pptm => pptm.ProductTag)
            .WithMany(pt => pt.ProductProductTagMappings)
            .HasForeignKey(pptm => pptm.ProductTagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 
