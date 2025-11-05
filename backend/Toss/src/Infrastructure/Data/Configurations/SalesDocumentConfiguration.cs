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

        builder.HasOne(d => d.Sale)
            .WithMany()
            .HasForeignKey(d => d.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

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
    }
}
