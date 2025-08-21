namespace TossErp.Accounts.Infrastructure.Data;

/// <summary>
/// Multi-tenant DbContext for Accounts service with EF Core 9 optimizations
/// </summary>
public class AccountsDbContext : DbContext
{
    private readonly ICurrentTenantService _currentTenantService;

    public AccountsDbContext(
        DbContextOptions<AccountsDbContext> options,
        ICurrentTenantService currentTenantService) : base(options)
    {
        _currentTenantService = currentTenantService;
    }

    // Chart of Accounts
    public DbSet<ChartOfAccount> ChartOfAccounts => Set<ChartOfAccount>();
    public DbSet<AccountType> AccountTypes => Set<AccountType>();
    
    // Journal Entries and Transactions
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<JournalEntryLine> JournalEntryLines => Set<JournalEntryLine>();
    public DbSet<GeneralLedger> GeneralLedgers => Set<GeneralLedger>();
    
    // Customers and Vendors
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<Contact> Contacts => Set<Contact>();
    
    // Invoicing
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceLine> InvoiceLines => Set<InvoiceLine>();
    public DbSet<InvoicePayment> InvoicePayments => Set<InvoicePayment>();
    
    // Bills and Expenses
    public DbSet<Bill> Bills => Set<Bill>();
    public DbSet<BillLine> BillLines => Set<BillLine>();
    public DbSet<BillPayment> BillPayments => Set<BillPayment>();
    public DbSet<Expense> Expenses => Set<Expense>();
    
    // Banking and Payments
    public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
    public DbSet<BankTransaction> BankTransactions => Set<BankTransaction>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    
    // Financial Reports
    public DbSet<FinancialPeriod> FinancialPeriods => Set<FinancialPeriod>();
    public DbSet<BudgetEntry> BudgetEntries => Set<BudgetEntry>();
    public DbSet<TaxRate> TaxRates => Set<TaxRate>();
    
    // Audit and Documents
    public DbSet<AccountingDocument> AccountingDocuments => Set<AccountingDocument>();
    public DbSet<AccountingAuditLog> AccountingAuditLogs => Set<AccountingAuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountsDbContext).Assembly);

        // Apply migration helper configurations
        AccountsMigrationHelper.SeedAccountTypes(modelBuilder);
        AccountsMigrationHelper.SeedChartOfAccounts(modelBuilder);
        AccountsMigrationHelper.SeedPaymentMethods(modelBuilder);
        AccountsMigrationHelper.ConfigureDatabaseOptimizations(modelBuilder);
        AccountsMigrationHelper.ConfigureTriggers(modelBuilder);

        // Configure multi-tenant global query filters
        ConfigureGlobalQueryFilters(modelBuilder);

        // Configure database optimizations for EF Core 9
        ConfigureDatabaseOptimizations(modelBuilder);
    }

    private void ConfigureGlobalQueryFilters(ModelBuilder modelBuilder)
    {
        var tenantId = _currentTenantService.TenantId;

        // Chart of Accounts and Types
        modelBuilder.Entity<ChartOfAccount>()
            .HasQueryFilter(e => e.TenantId == tenantId || e.TenantId == Guid.Empty);
        
        modelBuilder.Entity<AccountType>()
            .HasQueryFilter(e => e.TenantId == tenantId || e.TenantId == Guid.Empty);

        // Journal Entries and GL
        modelBuilder.Entity<JournalEntry>()
            .HasQueryFilter(e => e.TenantId == tenantId);
        
        modelBuilder.Entity<JournalEntryLine>()
            .HasQueryFilter(e => e.TenantId == tenantId);
        
        modelBuilder.Entity<GeneralLedger>()
            .HasQueryFilter(e => e.TenantId == tenantId);

        // Customers and Vendors
        modelBuilder.Entity<Customer>()
            .HasQueryFilter(e => e.TenantId == tenantId);
        
        modelBuilder.Entity<Vendor>()
            .HasQueryFilter(e => e.TenantId == tenantId);
        
        modelBuilder.Entity<Contact>()
            .HasQueryFilter(e => e.TenantId == tenantId);

        // Invoicing
        modelBuilder.Entity<Invoice>()
            .HasQueryFilter(e => e.TenantId == tenantId);
        
        modelBuilder.Entity<InvoiceLine>()
            .HasQueryFilter(e => e.TenantId == tenantId);
        
        modelBuilder.Entity<InvoicePayment>()
            .HasQueryFilter(e => e.TenantId == tenantId);

        // Bills and Expenses
        modelBuilder.Entity<Bill>()
            .HasQueryFilter(e => e.TenantId == tenantId);
        
        modelBuilder.Entity<BillLine>()
            .HasQueryFilter(e => e.TenantId == tenantId);
        
        modelBuilder.Entity<BillPayment>()
            .HasQueryFilter(e => e.TenantId == tenantId);
        
        modelBuilder.Entity<Expense>()
            .HasQueryFilter(e => e.TenantId == tenantId);

        // Banking and Payments
        modelBuilder.Entity<BankAccount>()
            .HasQueryFilter(e => e.TenantId == tenantId);
        
        modelBuilder.Entity<BankTransaction>()
            .HasQueryFilter(e => e.TenantId == tenantId);
        
        modelBuilder.Entity<Payment>()
            .HasQueryFilter(e => e.TenantId == tenantId);
        
        modelBuilder.Entity<PaymentMethod>()
            .HasQueryFilter(e => e.TenantId == tenantId || e.TenantId == Guid.Empty);

        // Financial Reports and Budget
        modelBuilder.Entity<FinancialPeriod>()
            .HasQueryFilter(e => e.TenantId == tenantId);
        
        modelBuilder.Entity<BudgetEntry>()
            .HasQueryFilter(e => e.TenantId == tenantId);
        
        modelBuilder.Entity<TaxRate>()
            .HasQueryFilter(e => e.TenantId == tenantId || e.TenantId == Guid.Empty);

        // Documents and Audit
        modelBuilder.Entity<AccountingDocument>()
            .HasQueryFilter(e => e.TenantId == tenantId);
        
        modelBuilder.Entity<AccountingAuditLog>()
            .HasQueryFilter(e => e.TenantId == tenantId);
    }

    private static void ConfigureDatabaseOptimizations(ModelBuilder modelBuilder)
    {
        // Configure indexes for performance with EF Core 9 fill factors
        
        // Journal Entry performance indexes
        modelBuilder.Entity<JournalEntry>(entity =>
        {
            entity.HasIndex(e => new { e.TenantId, e.EntryDate, e.Status })
                .HasDatabaseName("IX_JournalEntries_TenantId_EntryDate_Status")
                .HasFillFactor(90);
            
            entity.HasIndex(e => new { e.TenantId, e.ReferenceNumber })
                .IsUnique()
                .HasDatabaseName("IX_JournalEntries_TenantId_ReferenceNumber_Unique")
                .HasFillFactor(95);
            
            entity.HasIndex(e => new { e.TenantId, e.FinancialPeriodId })
                .HasDatabaseName("IX_JournalEntries_TenantId_FinancialPeriodId")
                .HasFillFactor(90);
        });

        // General Ledger performance indexes
        modelBuilder.Entity<GeneralLedger>(entity =>
        {
            entity.HasIndex(e => new { e.TenantId, e.AccountId, e.TransactionDate })
                .HasDatabaseName("IX_GeneralLedger_TenantId_AccountId_Date")
                .HasFillFactor(85);
            
            entity.HasIndex(e => new { e.TenantId, e.FinancialPeriodId, e.AccountId })
                .HasDatabaseName("IX_GeneralLedger_TenantId_Period_AccountId")
                .HasFillFactor(90);
        });

        // Invoice performance indexes
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasIndex(e => new { e.TenantId, e.Status, e.DueDate })
                .HasDatabaseName("IX_Invoices_TenantId_Status_DueDate")
                .HasFillFactor(90);
            
            entity.HasIndex(e => new { e.TenantId, e.CustomerId, e.InvoiceDate })
                .HasDatabaseName("IX_Invoices_TenantId_CustomerId_Date")
                .HasFillFactor(90);
            
            entity.HasIndex(e => new { e.TenantId, e.InvoiceNumber })
                .IsUnique()
                .HasDatabaseName("IX_Invoices_TenantId_InvoiceNumber_Unique")
                .HasFillFactor(95);
        });

        // Bank Transaction indexes for reconciliation
        modelBuilder.Entity<BankTransaction>(entity =>
        {
            entity.HasIndex(e => new { e.TenantId, e.BankAccountId, e.TransactionDate })
                .HasDatabaseName("IX_BankTransactions_TenantId_AccountId_Date")
                .HasFillFactor(80);
            
            entity.HasIndex(e => new { e.TenantId, e.IsReconciled, e.TransactionDate })
                .HasDatabaseName("IX_BankTransactions_TenantId_Reconciled_Date")
                .HasFillFactor(85);
        });

        // Audit log indexes
        modelBuilder.Entity<AccountingAuditLog>(entity =>
        {
            entity.HasIndex(e => new { e.TenantId, e.EntityType, e.Timestamp })
                .HasDatabaseName("IX_AccountingAuditLogs_TenantId_EntityType_Timestamp")
                .HasFillFactor(70);
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Set tenant ID for new entities
        SetTenantIdForNewEntities();
        
        // Set audit fields
        SetAuditFields();
        
        // Generate automatic accounting entries if needed
        await GenerateAutomaticEntries();
        
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        // Set tenant ID for new entities
        SetTenantIdForNewEntities();
        
        // Set audit fields
        SetAuditFields();
        
        // Generate automatic accounting entries if needed
        GenerateAutomaticEntries().GetAwaiter().GetResult();
        
        return base.SaveChanges();
    }

    private void SetTenantIdForNewEntities()
    {
        var tenantId = _currentTenantService.TenantId;
        
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added && entry.Entity is ITenantEntity tenantEntity)
            {
                tenantEntity.TenantId = tenantId;
            }
        }
    }

    private void SetAuditFields()
    {
        var userId = _currentTenantService.UserId;
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is IAuditableEntity auditableEntity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        auditableEntity.CreatedAt = now;
                        auditableEntity.CreatedBy = userId;
                        break;
                    
                    case EntityState.Modified:
                        auditableEntity.UpdatedAt = now;
                        auditableEntity.UpdatedBy = userId;
                        break;
                }
            }
        }
    }

    private async Task GenerateAutomaticEntries()
    {
        // Auto-generate journal entries for certain transactions
        var newInvoices = ChangeTracker.Entries<Invoice>()
            .Where(e => e.State == EntityState.Added)
            .Select(e => e.Entity)
            .ToList();

        var newBills = ChangeTracker.Entries<Bill>()
            .Where(e => e.State == EntityState.Added)
            .Select(e => e.Entity)
            .ToList();

        // Generate journal entries for new invoices
        foreach (var invoice in newInvoices)
        {
            if (invoice.AutoGenerateJournalEntry)
            {
                await CreateInvoiceJournalEntry(invoice);
            }
        }

        // Generate journal entries for new bills
        foreach (var bill in newBills)
        {
            if (bill.AutoGenerateJournalEntry)
            {
                await CreateBillJournalEntry(bill);
            }
        }
    }

    private async Task CreateInvoiceJournalEntry(Invoice invoice)
    {
        // This would create appropriate debit/credit entries
        // Placeholder for automatic journal entry generation
        await Task.CompletedTask;
    }

    private async Task CreateBillJournalEntry(Bill bill)
    {
        // This would create appropriate debit/credit entries
        // Placeholder for automatic journal entry generation
        await Task.CompletedTask;
    }
}

/// <summary>
/// Interface for tenant-aware entities
/// </summary>
public interface ITenantEntity
{
    Guid TenantId { get; set; }
}

/// <summary>
/// Interface for auditable entities
/// </summary>
public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    string CreatedBy { get; set; }
    DateTime? UpdatedAt { get; set; }
    string? UpdatedBy { get; set; }
}
