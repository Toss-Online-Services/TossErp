using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Context.Configurations;

public class RelatedProductConfiguration : IEntityTypeConfiguration<RelatedProduct>
{
    public void Configure(EntityTypeBuilder<RelatedProduct> builder)
    {
        builder.ToTable("RelatedProducts");

        builder.HasKey(rp => rp.Id);

        builder.HasOne(rp => rp.Product1)
            .WithMany(p => p.RelatedProducts)
            .HasForeignKey(rp => rp.ProductId1)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rp => rp.Product2)
            .WithMany()
            .HasForeignKey(rp => rp.ProductId2)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 
