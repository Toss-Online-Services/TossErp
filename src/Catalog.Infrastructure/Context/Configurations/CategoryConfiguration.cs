using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Context.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(400);

        builder.Property(c => c.Description)
            .HasMaxLength(4000);

        builder.Property(c => c.MetaKeywords)
            .HasMaxLength(400);

        builder.Property(c => c.MetaDescription)
            .HasMaxLength(4000);

        builder.Property(c => c.MetaTitle)
            .HasMaxLength(400);

        builder.Property(c => c.SeName)
            .HasMaxLength(400);

        builder.Property(c => c.PriceRanges)
            .HasMaxLength(400);

        builder.Property(c => c.PageSizeOptions)
            .HasMaxLength(200);

        builder.HasMany(c => c.ProductCategories)
            .WithOne(pc => pc.Category)
            .HasForeignKey(pc => pc.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.SubCategories)
            .WithOne(sc => sc.ParentCategory)
            .HasForeignKey(sc => sc.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
} 
