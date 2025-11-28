using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Orders;

namespace Toss.Infrastructure.Data.Configurations.Orders;

public class PurchaseDocumentLineConfiguration : IEntityTypeConfiguration<PurchaseDocumentLine>
{
    public void Configure(EntityTypeBuilder<PurchaseDocumentLine> builder)
    {
        builder.ToTable("PurchaseDocumentLines");

        builder.Property(line => line.Description)
            .HasMaxLength(500);

        builder.Property(line => line.Quantity)
            .HasPrecision(18, 3);

        builder.Property(line => line.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(line => line.TaxRate)
            .HasPrecision(5, 4);

        builder.Property(line => line.TaxAmount)
            .HasPrecision(18, 2);

        builder.Property(line => line.LineTotal)
            .HasPrecision(18, 2);

        builder.HasOne(line => line.PurchaseDocument)
            .WithMany(doc => doc.Lines)
            .HasForeignKey(line => line.PurchaseDocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(line => line.PurchaseOrderItem)
            .WithMany()
            .HasForeignKey(line => line.PurchaseOrderItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(line => line.PurchaseDocumentId);
        builder.HasIndex(line => line.ProductId);
    }
}

