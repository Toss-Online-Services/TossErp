using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Buying;

namespace Toss.Infrastructure.Data.Configurations;

public class PurchaseReceiptConfiguration : IEntityTypeConfiguration<PurchaseReceipt>
{
    public void Configure(EntityTypeBuilder<PurchaseReceipt> builder)
    {
        builder.Property(r => r.ReceiptNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.ReceivedBy)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(r => r.Notes)
            .HasMaxLength(1000);

        builder.Property(r => r.QualityNotes)
            .HasMaxLength(1000);

        builder.HasIndex(r => r.ReceiptNumber)
            .IsUnique();

        builder.HasIndex(r => r.ReceivedDate);
    }
}

