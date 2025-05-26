using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Context.Configurations;

public class ProductPictureConfiguration : IEntityTypeConfiguration<ProductPicture>
{
    public void Configure(EntityTypeBuilder<ProductPicture> builder)
    {
        builder.ToTable("ProductPictures");

        builder.HasKey(pp => pp.Id);

        builder.Property(pp => pp.Url)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(pp => pp.AltAttribute)
            .HasMaxLength(100);

        builder.Property(pp => pp.TitleAttribute)
            .HasMaxLength(100);

        // Relationships
        builder.HasOne(pp => pp.Product)
            .WithMany(p => p.Pictures)
            .HasForeignKey(pp => pp.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(pp => pp.ProductId);
        builder.HasIndex(pp => pp.DisplayOrder);
    }
} 
