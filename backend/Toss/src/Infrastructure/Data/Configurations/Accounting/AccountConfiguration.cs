using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Accounting;

namespace Toss.Infrastructure.Data.Configurations.Accounting;

/// <summary>
/// Entity Framework Core configuration for the Account entity.
/// Defines table schema, constraints, indexes, and relationships.
/// </summary>
public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        // Required fields with length constraints
        builder.Property(a => a.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.AccountNumber)
            .HasMaxLength(50);

        builder.Property(a => a.BankName)
            .HasMaxLength(200);

        builder.Property(a => a.Notes)
            .HasMaxLength(1000);

        // Decimal precision for balance
        builder.Property(a => a.CurrentBalance)
            .HasPrecision(18, 2);

        // Relationships
        builder.HasOne(a => a.Business)
            .WithMany()
            .HasForeignKey(a => a.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Store)
            .WithMany()
            .HasForeignKey(a => a.StoreId)
            .OnDelete(DeleteBehavior.SetNull); // Allow accounts without store linkage

        builder.HasMany(a => a.Entries)
            .WithOne(e => e.Account)
            .HasForeignKey(e => e.AccountId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deletion if entries exist

        // Indexes for performance
        builder.HasIndex(a => new { a.BusinessId, a.Name })
            .IsUnique();

        builder.HasIndex(a => a.BusinessId);
        builder.HasIndex(a => a.StoreId);
        builder.HasIndex(a => a.Type);
        builder.HasIndex(a => a.IsActive);
    }
}

