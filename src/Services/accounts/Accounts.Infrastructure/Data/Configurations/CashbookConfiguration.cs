namespace TossErp.Accounts.Infrastructure.Data.Configurations;

/// <summary>
/// Cashbook entity configuration
/// </summary>
public class CashbookConfiguration : IEntityTypeConfiguration<Cashbook>
{
    public void Configure(EntityTypeBuilder<Cashbook> builder)
    {
        builder.ToTable("Cashbooks", "accounts");

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Tenant
        builder.Property(e => e.TenantId)
            .IsRequired();

        // Basic Properties
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.OpeningBalanceDate)
            .IsRequired();

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Complex Types
        builder.ComplexProperty(e => e.OpeningBalance, ob =>
        {
            ob.Property(o => o.Amount)
                .HasColumnName("OpeningBalance")
                .HasPrecision(18, 2)
                .IsRequired()
                .HasDefaultValue(0);

            ob.Property(o => o.Currency)
                .HasColumnName("OpeningBalanceCurrency")
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

        // Relationships
        builder.HasMany(e => e.Entries)
            .WithOne()
            .HasForeignKey(e => e.CashbookId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => new { e.TenantId, e.Name })
            .IsUnique()
            .HasDatabaseName("IX_Cashbooks_TenantId_Name_Unique")
            .HasFillFactor(95);

        builder.HasIndex(e => new { e.TenantId, e.IsActive })
            .HasDatabaseName("IX_Cashbooks_TenantId_IsActive")
            .HasFillFactor(95);
    }
}

/// <summary>
/// Cashbook Entry entity configuration
/// </summary>
public class CashbookEntryConfiguration : IEntityTypeConfiguration<CashbookEntry>
{
    public void Configure(EntityTypeBuilder<CashbookEntry> builder)
    {
        builder.ToTable("CashbookEntries", "accounts");

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Tenant
        builder.Property(e => e.TenantId)
            .IsRequired();

        // Foreign Keys
        builder.Property(e => e.CashbookId)
            .IsRequired();

        builder.Property(e => e.AccountId)
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

        builder.Property(e => e.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Category)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        // Financial Amount
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
        });

        // Reconciliation
        builder.Property(e => e.IsReconciled)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.ReconciledDate);

        builder.Property(e => e.ReconciledBy)
            .HasMaxLength(256);

        // Related Entity
        builder.Property(e => e.RelatedEntityId)
            .HasMaxLength(100);

        builder.Property(e => e.RelatedEntityType)
            .HasMaxLength(50);

        // Audit Properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(256);

        builder.Property(e => e.UpdatedAt);

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(256);

        // Relationships
        builder.HasOne(e => e.Account)
            .WithMany()
            .HasForeignKey(e => e.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(e => new { e.TenantId, e.CashbookId, e.TransactionDate })
            .HasDatabaseName("IX_CashbookEntries_TenantId_CashbookId_TransactionDate")
            .HasFillFactor(85);

        builder.HasIndex(e => new { e.TenantId, e.AccountId })
            .HasDatabaseName("IX_CashbookEntries_TenantId_AccountId")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.IsReconciled, e.TransactionDate })
            .HasDatabaseName("IX_CashbookEntries_TenantId_IsReconciled_TransactionDate")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.Category, e.Type })
            .HasDatabaseName("IX_CashbookEntries_TenantId_Category_Type")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.RelatedEntityId, e.RelatedEntityType })
            .HasDatabaseName("IX_CashbookEntries_TenantId_RelatedEntity")
            .HasFillFactor(95);
    }
}
