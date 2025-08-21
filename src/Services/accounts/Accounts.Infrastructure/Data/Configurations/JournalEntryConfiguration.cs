namespace TossErp.Accounts.Infrastructure.Data.Configurations;

/// <summary>
/// Journal Entry entity configuration
/// </summary>
public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.ToTable("JournalEntries", "accounts");

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Tenant
        builder.Property(e => e.TenantId)
            .IsRequired();

        // Entry Details
        builder.Property(e => e.ReferenceNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.EntryDate)
            .IsRequired();

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.EntryType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.SourceModule)
            .HasMaxLength(50);

        builder.Property(e => e.SourceDocumentId);

        builder.Property(e => e.FinancialPeriodId)
            .IsRequired();

        // Complex type for totals
        builder.ComplexProperty(e => e.TotalAmount, ta =>
        {
            ta.Property(t => t.Amount)
                .HasColumnName("TotalAmount")
                .HasPrecision(18, 2)
                .IsRequired();

            ta.Property(t => t.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");
        });

        // Audit Properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(256);

        builder.Property(e => e.UpdatedAt);

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(256);

        builder.Property(e => e.PostedAt);

        builder.Property(e => e.PostedBy)
            .HasMaxLength(256);

        // Relationships
        builder.HasOne(e => e.FinancialPeriod)
            .WithMany()
            .HasForeignKey(e => e.FinancialPeriodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Lines)
            .WithOne(l => l.JournalEntry)
            .HasForeignKey(l => l.JournalEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => new { e.TenantId, e.ReferenceNumber })
            .IsUnique()
            .HasDatabaseName("IX_JournalEntries_TenantId_ReferenceNumber_Unique")
            .HasFillFactor(95);

        builder.HasIndex(e => new { e.TenantId, e.EntryDate, e.Status })
            .HasDatabaseName("IX_JournalEntries_TenantId_EntryDate_Status")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.FinancialPeriodId })
            .HasDatabaseName("IX_JournalEntries_TenantId_FinancialPeriodId")
            .HasFillFactor(90);
    }
}

/// <summary>
/// Journal Entry Line entity configuration
/// </summary>
public class JournalEntryLineConfiguration : IEntityTypeConfiguration<JournalEntryLine>
{
    public void Configure(EntityTypeBuilder<JournalEntryLine> builder)
    {
        builder.ToTable("JournalEntryLines", "accounts");

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Tenant
        builder.Property(e => e.TenantId)
            .IsRequired();

        // Foreign Keys
        builder.Property(e => e.JournalEntryId)
            .IsRequired();

        builder.Property(e => e.AccountId)
            .IsRequired();

        // Line Details
        builder.Property(e => e.AccountName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.Reference)
            .HasMaxLength(100);

        builder.Property(e => e.CostCenterId);

        // Complex type for signed money
        builder.ComplexProperty(e => e.Amount, am =>
        {
            am.Property(a => a.Amount)
                .HasColumnName("Amount")
                .HasPrecision(18, 2)
                .IsRequired();

            am.Property(a => a.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");

            am.Property(a => a.Type)
                .HasColumnName("DebitCredit")
                .HasConversion<string>()
                .HasMaxLength(6)
                .IsRequired();
        });

        // Audit Properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        // Relationships
        builder.HasOne(e => e.JournalEntry)
            .WithMany(je => je.Lines)
            .HasForeignKey(e => e.JournalEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Account)
            .WithMany()
            .HasForeignKey(e => e.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(e => new { e.TenantId, e.JournalEntryId })
            .HasDatabaseName("IX_JournalEntryLines_TenantId_JournalEntryId")
            .HasFillFactor(85);

        builder.HasIndex(e => new { e.TenantId, e.AccountId })
            .HasDatabaseName("IX_JournalEntryLines_TenantId_AccountId")
            .HasFillFactor(85);
    }
}

/// <summary>
/// General Ledger entity configuration
/// </summary>
public class GeneralLedgerConfiguration : IEntityTypeConfiguration<GeneralLedger>
{
    public void Configure(EntityTypeBuilder<GeneralLedger> builder)
    {
        builder.ToTable("GeneralLedger", "accounts");

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Tenant
        builder.Property(e => e.TenantId)
            .IsRequired();

        // Foreign Keys
        builder.Property(e => e.AccountId)
            .IsRequired();

        builder.Property(e => e.JournalEntryId)
            .IsRequired();

        builder.Property(e => e.JournalEntryLineId)
            .IsRequired();

        builder.Property(e => e.FinancialPeriodId)
            .IsRequired();

        // Transaction Details
        builder.Property(e => e.TransactionDate)
            .IsRequired();

        builder.Property(e => e.Reference)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(500);

        // Complex type for signed money
        builder.ComplexProperty(e => e.Amount, am =>
        {
            am.Property(a => a.Amount)
                .HasColumnName("Amount")
                .HasPrecision(18, 2)
                .IsRequired();

            am.Property(a => a.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");

            am.Property(a => a.Type)
                .HasColumnName("DebitCredit")
                .HasConversion<string>()
                .HasMaxLength(6)
                .IsRequired();
        });

        // Running Balance (computed)
        builder.ComplexProperty(e => e.RunningBalance, rb =>
        {
            rb.Property(r => r.Amount)
                .HasColumnName("RunningBalance")
                .HasPrecision(18, 2)
                .IsRequired();

            rb.Property(r => r.Currency)
                .HasColumnName("RunningBalanceCurrency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");
        });

        builder.Property(e => e.CostCenterId);

        // Audit Properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        // Relationships
        builder.HasOne(e => e.Account)
            .WithMany()
            .HasForeignKey(e => e.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.JournalEntry)
            .WithMany()
            .HasForeignKey(e => e.JournalEntryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.FinancialPeriod)
            .WithMany()
            .HasForeignKey(e => e.FinancialPeriodId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for performance
        builder.HasIndex(e => new { e.TenantId, e.AccountId, e.TransactionDate })
            .HasDatabaseName("IX_GeneralLedger_TenantId_AccountId_TransactionDate")
            .HasFillFactor(85);

        builder.HasIndex(e => new { e.TenantId, e.FinancialPeriodId, e.AccountId })
            .HasDatabaseName("IX_GeneralLedger_TenantId_FinancialPeriodId_AccountId")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.JournalEntryId })
            .HasDatabaseName("IX_GeneralLedger_TenantId_JournalEntryId")
            .HasFillFactor(90);
    }
}
