using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Catalog;

namespace Toss.Infrastructure.Data.Configurations;

public class ProductTagConfiguration : IEntityTypeConfiguration<ProductTag>
{
    public void Configure(EntityTypeBuilder<ProductTag> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(400);

        builder.HasMany(x => x.ProductProductTagMappings)
            .WithOne(x => x.ProductTag)
            .HasForeignKey(x => x.ProductTagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

