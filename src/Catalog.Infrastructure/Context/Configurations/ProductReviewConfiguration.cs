using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Context.Configurations;

public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
{
    public void Configure(EntityTypeBuilder<ProductReview> builder)
    {
        builder.ToTable("ProductReviews");

        builder.HasKey(pr => pr.Id);

        builder.Property(pr => pr.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(pr => pr.ReviewText)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(pr => pr.ReplyText)
            .HasMaxLength(4000);

        builder.Property(pr => pr.CustomerName)
            .HasMaxLength(400);

        builder.Property(pr => pr.CustomerEmail)
            .HasMaxLength(400);

        builder.HasOne(pr => pr.Product)
            .WithMany(p => p.ProductReviews)
            .HasForeignKey(pr => pr.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(pr => pr.ProductId);
        builder.HasIndex(pr => pr.CustomerId);
        builder.HasIndex(pr => pr.IsApproved);
        builder.HasIndex(pr => pr.CreatedOnUtc);
    }
} 
