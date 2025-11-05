using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Orders;

namespace Toss.Infrastructure.Data.Configurations.Orders;

public class PurchaseDocumentConfiguration : IEntityTypeConfiguration<PurchaseDocument>
{
    public void Configure(EntityTypeBuilder<PurchaseDocument> builder)
    {
        builder.ToTable("PurchaseDocuments");

        builder.HasKey(pd => pd.Id);

        builder.Property(pd => pd.DocumentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(pd => pd.DocumentType)
            .IsRequired();

        builder.Property(pd => pd.Subtotal)
            .HasPrecision(18, 2);

        builder.Property(pd => pd.TaxAmount)
            .HasPrecision(18, 2);

        builder.Property(pd => pd.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(pd => pd.ApprovedBy)
            .HasMaxLength(256);

        builder.Property(pd => pd.PaymentReference)
            .HasMaxLength(100);

        builder.Property(pd => pd.Notes)
            .HasMaxLength(2000);

        // Indexes
        builder.HasIndex(pd => pd.DocumentNumber)
            .IsUnique();

        builder.HasIndex(pd => pd.DocumentType);

        builder.HasIndex(pd => pd.DocumentDate);

        builder.HasIndex(pd => pd.IsPaid);

        builder.HasIndex(pd => pd.IsApproved);

        builder.HasIndex(pd => new { pd.IsMatchedToPO, pd.IsMatchedToReceipt });

        // Relationships
        builder.HasOne(pd => pd.PurchaseOrder)
            .WithMany()
            .HasForeignKey(pd => pd.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pd => pd.Vendor)
            .WithMany()
            .HasForeignKey(pd => pd.VendorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pd => pd.Shop)
            .WithMany()
            .HasForeignKey(pd => pd.ShopId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
