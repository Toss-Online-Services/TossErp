namespace TossErp.Accounts.Infrastructure.Data.Configurations;

/// <summary>
/// Bank Account entity configuration
/// </summary>
public class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
{
    public void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        builder.ToTable("BankAccounts", "accounts");

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Tenant
        builder.Property(e => e.TenantId)
            .IsRequired();

        // Bank Details
        builder.Property(e => e.AccountNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.AccountName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.AccountType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.BankName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.BankCode)
            .HasMaxLength(20);

        builder.Property(e => e.BankBranch)
            .HasMaxLength(100);

        builder.Property(e => e.RoutingNumber)
            .HasMaxLength(20);

        builder.Property(e => e.SwiftCode)
            .HasMaxLength(20);

        builder.Property(e => e.IbanNumber)
            .HasMaxLength(50);

        // Contact Information
        builder.ComplexProperty(e => e.BankAddress, ba =>
        {
            ba.Property(b => b.Street)
                .HasColumnName("BankAddressStreet")
                .HasMaxLength(200);

            ba.Property(b => b.City)
                .HasColumnName("BankAddressCity")
                .HasMaxLength(100);

            ba.Property(b => b.State)
                .HasColumnName("BankAddressState")
                .HasMaxLength(100);

            ba.Property(b => b.PostalCode)
                .HasColumnName("BankAddressPostalCode")
                .HasMaxLength(20);

            ba.Property(b => b.Country)
                .HasColumnName("BankAddressCountry")
                .HasMaxLength(100);
        });

        // Financial Information
        builder.ComplexProperty(e => e.CurrentBalance, cb =>
        {
            cb.Property(c => c.Amount)
                .HasColumnName("CurrentBalance")
                .HasPrecision(18, 2)
                .IsRequired()
                .HasDefaultValue(0);

            cb.Property(c => c.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");
        });

        builder.ComplexProperty(e => e.OverdraftLimit, ol =>
        {
            ol.Property(o => o.Amount)
                .HasColumnName("OverdraftLimit")
                .HasPrecision(18, 2)
                .IsRequired()
                .HasDefaultValue(0);

            ol.Property(o => o.Currency)
                .HasColumnName("OverdraftCurrency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");
        });

        // Status and Settings
        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.IsDefault)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.AllowOverdraft)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.AutoReconcile)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.LastReconciledDate);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.Notes)
            .HasMaxLength(1000);

        // Audit Properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(256);

        builder.Property(e => e.UpdatedAt);

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(256);

        // Relationships
        builder.HasMany(e => e.Transactions)
            .WithOne(t => t.BankAccount)
            .HasForeignKey(t => t.BankAccountId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => new { e.TenantId, e.AccountNumber })
            .IsUnique()
            .HasDatabaseName("IX_BankAccounts_TenantId_AccountNumber_Unique")
            .HasFillFactor(95);

        builder.HasIndex(e => new { e.TenantId, e.IsActive, e.IsDefault })
            .HasDatabaseName("IX_BankAccounts_TenantId_IsActive_IsDefault")
            .HasFillFactor(95);

        builder.HasIndex(e => new { e.TenantId, e.BankName, e.AccountType })
            .HasDatabaseName("IX_BankAccounts_TenantId_BankName_AccountType")
            .HasFillFactor(90);
    }
}

/// <summary>
/// Bank Transaction entity configuration
/// </summary>
public class BankTransactionConfiguration : IEntityTypeConfiguration<BankTransaction>
{
    public void Configure(EntityTypeBuilder<BankTransaction> builder)
    {
        builder.ToTable("BankTransactions", "accounts");

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Tenant
        builder.Property(e => e.TenantId)
            .IsRequired();

        // Foreign Keys
        builder.Property(e => e.BankAccountId)
            .IsRequired();

        // Transaction Details
        builder.Property(e => e.TransactionId)
            .HasMaxLength(100);

        builder.Property(e => e.TransactionDate)
            .IsRequired();

        builder.Property(e => e.TransactionType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Reference)
            .HasMaxLength(100);

        builder.Property(e => e.CheckNumber)
            .HasMaxLength(50);

        // Financial Information
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

            am.Property(a => a.IsCredit)
                .HasColumnName("IsCredit")
                .IsRequired();
        });

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

        // Status and Reconciliation
        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.IsReconciled)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.ReconciledDate);

        builder.Property(e => e.ReconciledBy)
            .HasMaxLength(256);

        // Bank Information
        builder.Property(e => e.BankReference)
            .HasMaxLength(100);

        builder.Property(e => e.BankMemo)
            .HasMaxLength(500);

        // Counterparty Information
        builder.Property(e => e.CounterpartyName)
            .HasMaxLength(200);

        builder.Property(e => e.CounterpartyAccount)
            .HasMaxLength(100);

        // Processing Information
        builder.Property(e => e.ImportedDate);

        builder.Property(e => e.ImportBatchId)
            .HasMaxLength(100);

        builder.Property(e => e.Notes)
            .HasMaxLength(1000);

        // Audit Properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(256);

        builder.Property(e => e.UpdatedAt);

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(256);

        // Relationships
        builder.HasOne(e => e.BankAccount)
            .WithMany(b => b.Transactions)
            .HasForeignKey(e => e.BankAccountId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => new { e.TenantId, e.BankAccountId, e.TransactionDate })
            .HasDatabaseName("IX_BankTransactions_TenantId_BankAccountId_TransactionDate")
            .HasFillFactor(85);

        builder.HasIndex(e => new { e.TenantId, e.TransactionId })
            .HasDatabaseName("IX_BankTransactions_TenantId_TransactionId")
            .HasFillFactor(95);

        builder.HasIndex(e => new { e.TenantId, e.IsReconciled, e.TransactionDate })
            .HasDatabaseName("IX_BankTransactions_TenantId_IsReconciled_TransactionDate")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.Status, e.TransactionType })
            .HasDatabaseName("IX_BankTransactions_TenantId_Status_TransactionType")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.CheckNumber })
            .HasDatabaseName("IX_BankTransactions_TenantId_CheckNumber")
            .HasFillFactor(95);
    }
}
