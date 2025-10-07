using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Domain.Entities.Finance;

namespace TossErp.Infrastructure.Data.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");
        
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.AccountCode)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(a => a.AccountCode)
            .IsUnique();
        
        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(a => a.Type)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(a => a.Description)
            .HasMaxLength(1000);
        
        builder.Property(a => a.CurrentBalance)
            .HasPrecision(18, 2);
        
        builder.Property(a => a.Currency)
            .HasMaxLength(3);
        
        builder.HasIndex(a => a.Type);
        builder.HasIndex(a => a.IsActive);
        
        // Self-referencing relationship for account hierarchy
        builder.HasOne(a => a.ParentAccount)
            .WithMany(a => a.SubAccounts)
            .HasForeignKey(a => a.ParentAccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.ToTable("JournalEntries");
        
        builder.HasKey(j => j.Id);
        
        builder.Property(j => j.EntryNumber)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(j => j.EntryNumber)
            .IsUnique();
        
        builder.Property(j => j.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(j => j.ReferenceType)
            .HasMaxLength(50);
        
        builder.Property(j => j.ReferenceNumber)
            .HasMaxLength(50);
        
        builder.Property(j => j.Description)
            .HasMaxLength(500);
        
        builder.Property(j => j.Notes)
            .HasMaxLength(1000);
        
        builder.Property(j => j.PostedByName)
            .HasMaxLength(200);
        
        builder.HasIndex(j => j.EntryDate);
        builder.HasIndex(j => j.Status);
        builder.HasIndex(j => new { j.ReferenceType, j.ReferenceId });
        
        // Relationships
        builder.HasMany(j => j.Lines)
            .WithOne(l => l.JournalEntry)
            .HasForeignKey(l => l.JournalEntryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class JournalEntryLineConfiguration : IEntityTypeConfiguration<JournalEntryLine>
{
    public void Configure(EntityTypeBuilder<JournalEntryLine> builder)
    {
        builder.ToTable("JournalEntryLines");
        
        builder.HasKey(l => l.Id);
        
        builder.Property(l => l.DebitAmount)
            .HasPrecision(18, 2);
        
        builder.Property(l => l.CreditAmount)
            .HasPrecision(18, 2);
        
        builder.Property(l => l.Description)
            .HasMaxLength(500);
        
        builder.HasIndex(l => l.JournalEntryId);
        builder.HasIndex(l => l.AccountId);
        builder.HasIndex(l => l.LineNumber);
    }
}

