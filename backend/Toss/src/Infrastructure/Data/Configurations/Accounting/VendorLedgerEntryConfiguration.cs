using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Accounting;

namespace Toss.Infrastructure.Data.Configurations.Accounting;

public class VendorLedgerEntryConfiguration : IEntityTypeConfiguration<VendorLedgerEntry>
{
    public void Configure(EntityTypeBuilder<VendorLedgerEntry> builder)
    {
        builder.ToTable("VendorLedgerEntries");

        builder.Property(entry => entry.Amount)
            .HasPrecision(18, 2);

        builder.Property(entry => entry.PaidAmount)
            .HasPrecision(18, 2);

        builder.Property(entry => entry.Balance)
            .HasPrecision(18, 2);

        builder.Property(entry => entry.ReferenceNumber)
            .HasMaxLength(50);

        builder.HasIndex(entry => entry.VendorId);
        builder.HasIndex(entry => entry.Status);
        builder.HasIndex(entry => entry.DueDate);
    }
}

