using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Sales;

namespace Toss.Infrastructure.Data.Configurations;

public class SalesDocumentConfiguration : IEntityTypeConfiguration<SalesDocument>
{
    public void Configure(EntityTypeBuilder<SalesDocument> builder)
    {
        builder.Property(d => d.DocumentNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(d => d.Subtotal).HasPrecision(18, 2);
        builder.Property(d => d.TaxAmount).HasPrecision(18, 2);
        builder.Property(d => d.TotalAmount).HasPrecision(18, 2);

        builder.Property(d => d.Notes).HasMaxLength(1000);

        // Explicitly configure Sale relationship to avoid EF Core auto-discovery conflicts
        builder.HasOne(d => d.Sale)
            .WithMany(s => s.Documents)
            .HasForeignKey(d => d.SaleId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne(d => d.Customer)
            .WithMany()
            .HasForeignKey(d => d.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Shop)
            .WithMany()
            .HasForeignKey(d => d.ShopId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(d => d.DocumentNumber).IsUnique();
        builder.HasIndex(d => d.DocumentDate);
        builder.HasIndex(d => new { d.SaleId, d.DocumentType }).IsUnique();
        
        // Composite indexes for common query patterns
        builder.HasIndex(d => new { d.ShopId, d.DocumentDate }); // For shop document reports
        builder.HasIndex(d => new { d.ShopId, d.DocumentType, d.DocumentDate }); // For filtered document queries
        builder.HasIndex(d => new { d.CustomerId, d.DocumentDate }); // For customer document history
        builder.HasIndex(d => new { d.ShopId, d.IsPaid, d.DocumentDate }); // For payment status queries
    }
}
