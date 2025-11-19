using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Catalog;

namespace Toss.Infrastructure.Data.Configurations;

public class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
{
    public void Configure(EntityTypeBuilder<ProductAttribute> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(400);

        builder.Property(x => x.Description);

        builder.HasMany(x => x.ProductAttributeValues)
            .WithOne(x => x.ProductAttribute)
            .HasForeignKey(x => x.ProductAttributeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

