using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Catalog;

namespace Toss.Infrastructure.Data.Configurations;

public class ProductProductTagMappingConfiguration : IEntityTypeConfiguration<ProductProductTagMapping>
{
    public void Configure(EntityTypeBuilder<ProductProductTagMapping> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.ProductTag)
            .WithMany(x => x.ProductProductTagMappings)
            .HasForeignKey(x => x.ProductTagId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.ProductId, x.ProductTagId })
            .IsUnique();
    }
}

