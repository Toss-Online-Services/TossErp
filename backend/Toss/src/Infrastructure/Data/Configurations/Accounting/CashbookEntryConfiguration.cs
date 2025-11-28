using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Accounting;

namespace Toss.Infrastructure.Data.Configurations.Accounting;

/// <summary>
/// Entity Framework Core configuration for the CashbookEntry entity.
/// Defines table schema, constraints, indexes, and relationships.
/// </summary>
public class CashbookEntryConfiguration : IEntityTypeConfiguration<CashbookEntry>
{
    public void Configure(EntityTypeBuilder<CashbookEntry> builder)
    {
        // Required fields with length constraints
        builder.Property(e => e.Reference)
            .HasMaxLength(100);

        builder.Property(e => e.Notes)
            .HasMaxLength(1000);

        builder.Property(e => e.SourceType)
            .HasMaxLength(50);

        // Decimal precision for amount
        builder.Property(e => e.Amount)
            .HasPrecision(18, 2);

        // Relationships
        builder.HasOne(e => e.Business)
            .WithMany()
            .HasForeignKey(e => e.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Account)
            .WithMany(a => a.Entries)
            .HasForeignKey(e => e.AccountId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deletion if entries exist

        builder.HasOne(e => e.CounterpartyAccount)
            .WithMany()
            .HasForeignKey(e => e.CounterpartyAccountId)
            .OnDelete(DeleteBehavior.SetNull); // Allow entries without counterparty (non-transfer entries)

        builder.HasOne(e => e.Payment)
            .WithMany()
            .HasForeignKey(e => e.PaymentId)
            .OnDelete(DeleteBehavior.SetNull); // Allow entries without payment linkage

        // Indexes for performance
        builder.HasIndex(e => new { e.BusinessId, e.EntryDate });
        builder.HasIndex(e => new { e.AccountId, e.EntryDate });
        builder.HasIndex(e => new { e.SourceType, e.SourceId });
        builder.HasIndex(e => e.PaymentId);
        builder.HasIndex(e => e.Type);
    }
}

