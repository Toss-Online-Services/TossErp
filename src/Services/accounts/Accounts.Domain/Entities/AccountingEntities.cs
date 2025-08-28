using TossErp.Accounts.Domain.Enums;
using TossErp.Accounts.Domain.SeedWork;
using TossErp.Accounts.Domain.ValueObjects;
using TossErp.Accounts.Domain.Aggregates;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TossErp.Accounts.Domain.Entities;

/// <summary>
/// Journal Entry Line entity representing individual accounting entries
/// </summary>
public class JournalEntryLine : Entity
{
    public Guid JournalEntryId { get; private set; }
    public Guid AccountId { get; private set; }
    public string AccountName { get; private set; }
    public SignedMoney Amount { get; private set; }
    public string? Description { get; private set; }
    public Guid? CostCenterId { get; private set; }
    public string? Reference { get; private set; }
    public new DateTime CreatedAt { get; private set; }

    // Application layer compatibility properties
    public string? AccountCode { get; private set; }
    public Money? DebitAmount => Amount.Type == DebitCredit.Debit ? Amount.ToMoney() : null;
    public Money? CreditAmount => Amount.Type == DebitCredit.Credit ? Amount.ToMoney() : null;

    private JournalEntryLine()
    {
        AccountName = null!;
        Amount = null!;
    } // EF Core

    public JournalEntryLine(
        Guid id,
        Guid journalEntryId,
        Guid accountId,
        string accountName,
        SignedMoney amount,
        string? description = null,
        Guid? costCenterId = null,
        string? reference = null,
        string? accountCode = null)
    {
        Id = id;
        JournalEntryId = journalEntryId;
        AccountId = accountId;
        AccountName = accountName?.Trim() ?? throw new ArgumentException("Account name cannot be empty");
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        Description = description?.Trim();
        CostCenterId = costCenterId;
        Reference = reference?.Trim();
        AccountCode = accountCode?.Trim();
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string? description)
    {
        Description = description?.Trim();
    }

    public void UpdateReference(string? reference)
    {
        Reference = reference?.Trim();
    }

    public bool IsDebit => Amount.Type == DebitCredit.Debit;
    public bool IsCredit => Amount.Type == DebitCredit.Credit;
}

/// <summary>
/// Payment Line entity for payment allocations
/// </summary>
public class PaymentLine : Entity
{
    public Guid PaymentId { get; private set; }
    public Guid InvoiceId { get; private set; }
    public string InvoiceNumber { get; private set; }
    public Money AllocatedAmount { get; private set; }
    public DateTime AllocationDate { get; private set; }
    public string? Notes { get; private set; }

    private PaymentLine()
    {
        InvoiceNumber = null!;
        AllocatedAmount = null!;
    } // EF Core

    public PaymentLine(
        Guid id,
        Guid paymentId,
        Guid invoiceId,
        string invoiceNumber,
        Money allocatedAmount,
        string? notes = null)
    {
        Id = id;
        PaymentId = paymentId;
        InvoiceId = invoiceId;
        InvoiceNumber = invoiceNumber?.Trim() ?? throw new ArgumentException("Invoice number cannot be empty");
        AllocatedAmount = allocatedAmount ?? throw new ArgumentNullException(nameof(allocatedAmount));
        AllocationDate = DateTime.UtcNow;
        Notes = notes?.Trim();
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes?.Trim();
    }
}

/// <summary>
/// Invoice Line entity for detailed billing
/// </summary>
public class InvoiceLine : Entity
{
    public Guid InvoiceId { get; private set; }
    public string ItemName { get; private set; }
    public string? Description { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    public Money? LineTotal { get; private set; }
    public TaxRate? TaxRate { get; private set; }
    public Money? TaxAmount { get; private set; }
    public Guid? ProductId { get; private set; }
    public string? ProductCode { get; private set; }

    private InvoiceLine()
    {
        ItemName = null!;
        UnitPrice = null!;
        LineTotal = null!;
    } // EF Core

    public InvoiceLine(
        Guid id,
        Guid invoiceId,
        string itemName,
        int quantity,
        Money unitPrice,
        string? description = null,
        TaxRate? taxRate = null,
        Guid? productId = null,
        string? productCode = null)
    {
        Id = id;
        InvoiceId = invoiceId;
        ItemName = itemName?.Trim() ?? throw new ArgumentException("Item name cannot be empty");
        Description = description?.Trim();
        Quantity = quantity > 0 ? quantity : throw new ArgumentException("Quantity must be positive");
        UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
        TaxRate = taxRate;
        ProductId = productId;
        ProductCode = productCode?.Trim();

        CalculateAmounts();
    }

    public void UpdateQuantity(int quantity)
    {
        Quantity = quantity > 0 ? quantity : throw new ArgumentException("Quantity must be positive");
        CalculateAmounts();
    }

    public void UpdateUnitPrice(Money unitPrice)
    {
        UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
        CalculateAmounts();
    }

    public void UpdateTaxRate(TaxRate? taxRate)
    {
        TaxRate = taxRate;
        CalculateAmounts();
    }

    private void CalculateAmounts()
    {
        LineTotal = UnitPrice.Multiply(Quantity);
        TaxAmount = TaxRate?.CalculateTax(LineTotal);
    }

    public Money TotalWithTax => LineTotal != null 
        ? (TaxAmount != null ? LineTotal.Add(TaxAmount) : LineTotal) 
        : Money.Zero(CurrencyCode.USD);
}

/// <summary>
/// Bank Transaction entity for bank reconciliation
/// </summary>
public class BankTransaction : Entity
{
    public Guid BankAccountId { get; private set; }
    public DateTime TransactionDate { get; private set; }
    public string Description { get; private set; }
    public string? Reference { get; private set; }
    public SignedMoney Amount { get; private set; }
    public Money Balance { get; private set; }
    public ReconciliationStatus ReconciliationStatus { get; private set; }
    public Guid? MatchedJournalEntryId { get; private set; }
    public DateTime ImportedAt { get; private set; }
    public string? BankReference { get; private set; }

    private BankTransaction()
    {
        Description = null!;
        Amount = null!;
        Balance = null!;
    } // EF Core

    public BankTransaction(
        Guid id,
        Guid bankAccountId,
        DateTime transactionDate,
        string description,
        SignedMoney amount,
        Money balance,
        string? reference = null,
        string? bankReference = null)
    {
        Id = id;
        BankAccountId = bankAccountId;
        TransactionDate = transactionDate.Date;
        Description = description?.Trim() ?? throw new ArgumentException("Description cannot be empty");
        Reference = reference?.Trim();
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        Balance = balance ?? throw new ArgumentNullException(nameof(balance));
        ReconciliationStatus = ReconciliationStatus.Unreconciled;
        ImportedAt = DateTime.UtcNow;
        BankReference = bankReference?.Trim();
    }

    public void Reconcile(Guid journalEntryId)
    {
        if (ReconciliationStatus == ReconciliationStatus.Reconciled)
            throw new InvalidOperationException("Transaction is already reconciled");

        MatchedJournalEntryId = journalEntryId;
        ReconciliationStatus = ReconciliationStatus.Reconciled;
    }

    public void MarkAsDisputed(string reason)
    {
        ReconciliationStatus = ReconciliationStatus.Disputed;
        // Could add reason tracking if needed
    }

    public void RequireAdjustment()
    {
        ReconciliationStatus = ReconciliationStatus.Adjustment;
    }

    public bool IsReconciled => ReconciliationStatus == ReconciliationStatus.Reconciled;
    public bool RequiresAttention => ReconciliationStatus == ReconciliationStatus.Disputed || 
                                   ReconciliationStatus == ReconciliationStatus.Adjustment;
}

/// <summary>
/// Tax Entry entity for detailed tax tracking
/// </summary>
public class TaxEntry : Entity
{
    public Guid TransactionId { get; private set; }
    public TransactionType TransactionType { get; private set; }
    public TaxType TaxType { get; private set; }
    public TaxRate TaxRate { get; private set; }
    public Money TaxableAmount { get; private set; }
    public Money TaxAmount { get; private set; }
    public Guid TaxAccountId { get; private set; }
    public DateTime TaxDate { get; private set; }
    public string? TaxPeriod { get; private set; }

    private TaxEntry()
    {
        TaxRate = null!;
        TaxableAmount = null!;
        TaxAmount = null!;
    } // EF Core

    public TaxEntry(
        Guid id,
        Guid transactionId,
        TransactionType transactionType,
        TaxType taxType,
        TaxRate taxRate,
        Money taxableAmount,
        Guid taxAccountId,
        DateTime taxDate,
        string? taxPeriod = null)
    {
        Id = id;
        TransactionId = transactionId;
        TransactionType = transactionType;
        TaxType = taxType;
        TaxRate = taxRate ?? throw new ArgumentNullException(nameof(taxRate));
        TaxableAmount = taxableAmount ?? throw new ArgumentNullException(nameof(taxableAmount));
        TaxAmount = taxRate.CalculateTax(taxableAmount);
        TaxAccountId = taxAccountId;
        TaxDate = taxDate.Date;
        TaxPeriod = taxPeriod?.Trim();
    }

    public void UpdateTaxPeriod(string taxPeriod)
    {
        TaxPeriod = taxPeriod?.Trim();
    }

    public decimal EffectiveTaxRate => TaxableAmount.Amount > 0 ? 
        (TaxAmount.Amount / TaxableAmount.Amount) * 100 : 0;
}

/// <summary>
/// Recurring Transaction Template entity for automated entries
/// </summary>
public class RecurringTransactionTemplate : Entity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public BillingFrequency Frequency { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public DateTime NextProcessDate { get; private set; }
    public bool IsActive { get; private set; }
    public TransactionType TransactionType { get; private set; }
    public new string CreatedBy { get; private set; }
    public new DateTime CreatedAt { get; private set; }
    public int ProcessedCount { get; private set; }

    private readonly List<RecurringTransactionLine> _lines;
    public IReadOnlyList<RecurringTransactionLine> Lines => _lines.AsReadOnly();

    private RecurringTransactionTemplate()
    {
        _lines = new List<RecurringTransactionLine>();
        Name = null!;
        CreatedBy = null!;
    } // EF Core

    public RecurringTransactionTemplate(
        Guid id,
        string name,
        BillingFrequency frequency,
        DateTime startDate,
        TransactionType transactionType,
        string createdBy,
        string? description = null,
        DateTime? endDate = null)
    {
        Id = id;
        Name = name?.Trim() ?? throw new ArgumentException("Name cannot be empty");
        Description = description?.Trim();
        Frequency = frequency;
        StartDate = startDate.Date;
        EndDate = endDate?.Date;
        TransactionType = transactionType;
        CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
        ProcessedCount = 0;
        _lines = new List<RecurringTransactionLine>();

        CalculateNextProcessDate();
    }

    public void AddLine(RecurringTransactionLine line)
    {
        if (line == null)
            throw new ArgumentNullException(nameof(line));

        _lines.Add(line);
    }

    public void RemoveLine(Guid lineId)
    {
        var line = _lines.FirstOrDefault(l => l.Id == lineId);
        if (line != null)
            _lines.Remove(line);
    }

    public void MarkAsProcessed()
    {
        ProcessedCount++;
        CalculateNextProcessDate();

        if (EndDate.HasValue && NextProcessDate > EndDate.Value)
        {
            IsActive = false;
        }
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private void CalculateNextProcessDate()
    {
        var baseDate = ProcessedCount == 0 ? StartDate : NextProcessDate;
        
        NextProcessDate = Frequency switch
        {
            BillingFrequency.Daily => baseDate.AddDays(1),
            BillingFrequency.Weekly => baseDate.AddDays(7),
            BillingFrequency.Monthly => baseDate.AddMonths(1),
            BillingFrequency.Quarterly => baseDate.AddMonths(3),
            BillingFrequency.SemiAnnually => baseDate.AddMonths(6),
            BillingFrequency.Annually => baseDate.AddYears(1),
            BillingFrequency.Biennial => baseDate.AddYears(2),
            _ => throw new InvalidOperationException($"Unsupported frequency: {Frequency}")
        };
    }

    public bool IsDueForProcessing => IsActive && NextProcessDate <= DateTime.UtcNow.Date;
}

/// <summary>
/// Recurring Transaction Line entity
/// </summary>
public class RecurringTransactionLine : Entity
{
    public Guid TemplateId { get; private set; }
    public Guid AccountId { get; private set; }
    public string AccountName { get; private set; }
    public SignedMoney Amount { get; private set; }
    public string? Description { get; private set; }
    public Guid? CostCenterId { get; private set; }

    private RecurringTransactionLine()
    {
        AccountName = null!;
        Amount = null!;
    } // EF Core

    public RecurringTransactionLine(
        Guid id,
        Guid templateId,
        Guid accountId,
        string accountName,
        SignedMoney amount,
        string? description = null,
        Guid? costCenterId = null)
    {
        Id = id;
        TemplateId = templateId;
        AccountId = accountId;
        AccountName = accountName?.Trim() ?? throw new ArgumentException("Account name cannot be empty");
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        Description = description?.Trim();
        CostCenterId = costCenterId;
    }

    public void UpdateAmount(SignedMoney amount)
    {
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
    }

    public void UpdateDescription(string? description)
    {
        Description = description?.Trim();
    }
}

/// <summary>
/// Cashbook aggregate root representing a cash management book for simplified accounting
/// </summary>
public class Cashbook : AggregateRoot
{
    private readonly List<CashbookEntry> _entries = new();

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Money OpeningBalance { get; private set; } = Money.Zero(CurrencyCode.USD);
    public DateTime OpeningBalanceDate { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation properties
    public IReadOnlyList<CashbookEntry> Entries => _entries.AsReadOnly();

    private Cashbook() : base() { } // For EF Core

    public Cashbook(
        Guid id,
        string name,
        string tenantId,
        Money openingBalance,
        string? description = null) : base(id, tenantId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Cashbook name cannot be empty", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
        OpeningBalance = openingBalance;
        OpeningBalanceDate = DateTime.UtcNow;
        IsActive = true;
    }

    public static Cashbook Create(string name, string tenantId, Money openingBalance, string? description = null)
    {
        return new Cashbook(Guid.NewGuid(), name, tenantId, openingBalance, description);
    }

    public void AddEntry(CashbookEntry entry)
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot add entries to inactive cashbook");

        if (entry == null)
            throw new ArgumentNullException(nameof(entry));

        _entries.Add(entry);
    }

    public CashbookEntry CreateAndAddEntry(DateTime transactionDate, string reference, string description,
        Money amount, CashbookEntryType type, CashbookEntryCategory category, Guid accountId,
        string? relatedEntityId = null, string? relatedEntityType = null)
    {
        var entry = CashbookEntry.Create(transactionDate, reference, description, amount, type,
            category, accountId, TenantId, relatedEntityId, relatedEntityType);

        AddEntry(entry);
        return entry;
    }

    public Money GetCurrentBalance()
    {
        var totalDebits = _entries.Where(e => e.Type == CashbookEntryType.Debit).Sum(e => e.Amount.Amount);
        var totalCredits = _entries.Where(e => e.Type == CashbookEntryType.Credit).Sum(e => e.Amount.Amount);

        var netAmount = totalDebits - totalCredits;
        netAmount += OpeningBalance.Amount;

        return new Money(netAmount, OpeningBalance.Currency);
    }

    public IEnumerable<CashbookEntry> GetEntriesForDateRange(DateTime fromDate, DateTime toDate)
    {
        return _entries.Where(e => e.TransactionDate.Date >= fromDate.Date &&
                                  e.TransactionDate.Date <= toDate.Date)
                      .OrderBy(e => e.TransactionDate)
                      .ThenBy(e => e.CreatedAt);
    }

    public IEnumerable<CashbookEntry> GetEntriesForAccount(Guid accountId)
    {
        return _entries.Where(e => e.AccountId == accountId)
                      .OrderBy(e => e.TransactionDate)
                      .ThenBy(e => e.CreatedAt);
    }

    public IEnumerable<CashbookEntry> GetEntriesByCategory(CashbookEntryCategory category)
    {
        return _entries.Where(e => e.Category == category)
                      .OrderBy(e => e.TransactionDate)
                      .ThenBy(e => e.CreatedAt);
    }

    public IEnumerable<CashbookEntry> GetUnreconciledEntries()
    {
        return _entries.Where(e => !e.IsReconciled)
                      .OrderBy(e => e.TransactionDate)
                      .ThenBy(e => e.CreatedAt);
    }

    public void UpdateDetails(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Cashbook name cannot be empty", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("Cashbook is already inactive");

        IsActive = false;
    }
}

/// <summary>
/// Cashbook Entry entity representing individual cash transactions
/// </summary>
public class CashbookEntry : Entity
{
    public Guid CashbookId { get; private set; }
    public DateTime TransactionDate { get; private set; }
    public string Reference { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Money Amount { get; private set; } = Money.Zero(CurrencyCode.USD);
    public CashbookEntryType Type { get; private set; }
    public CashbookEntryCategory Category { get; private set; }
    public Guid AccountId { get; private set; }
    public bool IsReconciled { get; private set; }
    public DateTime? ReconciledDate { get; private set; }
    public string? ReconciledBy { get; private set; }
    public string? RelatedEntityId { get; private set; }
    public string? RelatedEntityType { get; private set; }

    // Navigation properties
    public ChartOfAccounts? Account { get; private set; }

    private CashbookEntry() : base() { } // For EF Core

    public CashbookEntry(
        Guid id,
        Guid cashbookId,
        DateTime transactionDate,
        string reference,
        string description,
        Money amount,
        CashbookEntryType type,
        CashbookEntryCategory category,
        Guid accountId,
        string tenantId,
        string? relatedEntityId = null,
        string? relatedEntityType = null) : base(id, tenantId)
    {
        CashbookId = cashbookId;
        TransactionDate = transactionDate;
        Reference = reference?.Trim() ?? throw new ArgumentException("Reference cannot be empty");
        Description = description?.Trim() ?? throw new ArgumentException("Description cannot be empty");
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        Type = type;
        Category = category;
        AccountId = accountId;
        RelatedEntityId = relatedEntityId?.Trim();
        RelatedEntityType = relatedEntityType?.Trim();
        IsReconciled = false;
    }

    public static CashbookEntry Create(DateTime transactionDate, string reference, string description,
        Money amount, CashbookEntryType type, CashbookEntryCategory category, Guid accountId,
        string tenantId, string? relatedEntityId = null, string? relatedEntityType = null)
    {
        return new CashbookEntry(Guid.NewGuid(), Guid.Empty, transactionDate, reference, description,
            amount, type, category, accountId, tenantId, relatedEntityId, relatedEntityType);
    }

    public void MarkAsReconciled(string reconciledBy)
    {
        if (IsReconciled)
            throw new InvalidOperationException("Entry is already reconciled");

        IsReconciled = true;
        ReconciledDate = DateTime.UtcNow;
        ReconciledBy = reconciledBy?.Trim();
    }

    public void UpdateDescription(string description)
    {
        Description = description?.Trim() ?? throw new ArgumentException("Description cannot be empty");
    }
}

/// <summary>
/// Company entity representing a company in the ERP system.
/// Based on ERPNext Company doctype specifications.
/// </summary>
[Table("Companies")]
public class Company : Entity
{
    /// <summary>
    /// Company name (required, unique)
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Company abbreviation for short references
    /// </summary>
    [Required]
    [StringLength(10)]
    public string Abbreviation { get; private set; } = string.Empty;

    /// <summary>
    /// Business domain/industry
    /// </summary>
    [StringLength(100)]
    public string? Domain { get; private set; }

    /// <summary>
    /// Default currency for the company
    /// </summary>
    [Required]
    [StringLength(3)]
    public string Currency { get; private set; } = string.Empty;

    /// <summary>
    /// Country where company is registered
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Country { get; private set; } = string.Empty;

    /// <summary>
    /// Tax identification number
    /// </summary>
    [StringLength(50)]
    public string? TaxId { get; private set; }

    /// <summary>
    /// Parent company ID for subsidiary relationships
    /// </summary>
    public Guid? ParentCompanyId { get; private set; }

    /// <summary>
    /// Navigation property for parent company
    /// </summary>
    [ForeignKey(nameof(ParentCompanyId))]
    public virtual Company? ParentCompany { get; private set; }

    /// <summary>
    /// Child companies (subsidiaries)
    /// </summary>
    public virtual ICollection<Company> ChildCompanies { get; private set; } = new List<Company>();

    /// <summary>
    /// Indicates if this is a group company (has subsidiaries)
    /// </summary>
    public bool IsGroup { get; private set; } = false;

    /// <summary>
    /// Whether the company is active
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Company description
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; private set; }

    /// <summary>
    /// Company logo URL or path
    /// </summary>
    [StringLength(500)]
    public string? Logo { get; private set; }

    /// <summary>
    /// Registration date
    /// </summary>
    public DateTime RegistrationDate { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Financial year start month (1-12)
    /// </summary>
    public int FiscalYearStartMonth { get; private set; } = 1;

    /// <summary>
    /// Date of incorporation
    /// </summary>
    public DateTime? DateOfIncorporation { get; private set; }

    /// <summary>
    /// Date of commencement of business
    /// </summary>
    public DateTime? DateOfCommencement { get; private set; }

    /// <summary>
    /// Company registration number
    /// </summary>
    [StringLength(100)]
    public string? RegistrationNumber { get; private set; }

    /// <summary>
    /// Website URL
    /// </summary>
    [StringLength(200)]
    public string? Website { get; private set; }

    /// <summary>
    /// Phone number
    /// </summary>
    [StringLength(20)]
    public string? Phone { get; private set; }

    /// <summary>
    /// Email address
    /// </summary>
    [StringLength(100)]
    public string? Email { get; private set; }

    /// <summary>
    /// Complete address
    /// </summary>
    [StringLength(500)]
    public string? Address { get; private set; }

    /// <summary>
    /// City
    /// </summary>
    [StringLength(100)]
    public string? City { get; private set; }

    /// <summary>
    /// State/Province
    /// </summary>
    [StringLength(100)]
    public string? State { get; private set; }

    /// <summary>
    /// Postal/ZIP code
    /// </summary>
    [StringLength(20)]
    public string? PostalCode { get; private set; }

    /// <summary>
    /// Created timestamp
    /// </summary>
    public new DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Last modified timestamp
    /// </summary>
    public DateTime ModifiedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// User who created the record
    /// </summary>
    [StringLength(100)]
    public new string? CreatedBy { get; private set; }

    /// <summary>
    /// User who last modified the record
    /// </summary>
    [StringLength(100)]
    public string? ModifiedBy { get; private set; }

    private Company() : base() { } // EF Core

    public Company(
        Guid id,
        string tenantId,
        string name,
        string abbreviation,
        string currency,
        string country,
        string createdBy,
        string? domain = null,
        string? taxId = null,
        Guid? parentCompanyId = null,
        bool isGroup = false,
        string? description = null) : base(id, tenantId)
    {
        Name = name?.Trim() ?? throw new ArgumentException("Company name cannot be empty");
        Abbreviation = abbreviation?.Trim() ?? throw new ArgumentException("Abbreviation cannot be empty");
        Currency = currency?.Trim() ?? throw new ArgumentException("Currency cannot be empty");
        Country = country?.Trim() ?? throw new ArgumentException("Country cannot be empty");
        CreatedBy = createdBy?.Trim() ?? throw new ArgumentException("CreatedBy cannot be empty");
        Domain = domain?.Trim();
        TaxId = taxId?.Trim();
        ParentCompanyId = parentCompanyId;
        IsGroup = isGroup;
        Description = description?.Trim();
        CreatedAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = createdBy;
    }

    public static Company Create(
        string tenantId,
        string name,
        string abbreviation,
        string currency,
        string country,
        string createdBy,
        string? domain = null,
        string? taxId = null,
        Guid? parentCompanyId = null,
        bool isGroup = false,
        string? description = null)
    {
        return new Company(Guid.NewGuid(), tenantId, name, abbreviation, currency, country, createdBy, 
            domain, taxId, parentCompanyId, isGroup, description);
    }

    public void UpdateBasicInfo(
        string name,
        string abbreviation,
        string currency,
        string country,
        string modifiedBy,
        string? domain = null,
        string? description = null)
    {
        Name = name?.Trim() ?? throw new ArgumentException("Company name cannot be empty");
        Abbreviation = abbreviation?.Trim() ?? throw new ArgumentException("Abbreviation cannot be empty");
        Currency = currency?.Trim() ?? throw new ArgumentException("Currency cannot be empty");
        Country = country?.Trim() ?? throw new ArgumentException("Country cannot be empty");
        Domain = domain?.Trim();
        Description = description?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void UpdateContactInfo(
        string? email,
        string? phone,
        string? website,
        string modifiedBy)
    {
        Email = email?.Trim();
        Phone = phone?.Trim();
        Website = website?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void UpdateAddress(
        string? address,
        string? city,
        string? state,
        string? postalCode,
        string modifiedBy)
    {
        Address = address?.Trim();
        City = city?.Trim();
        State = state?.Trim();
        PostalCode = postalCode?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void UpdateTaxInfo(
        string? taxId,
        string? registrationNumber,
        DateTime? dateOfIncorporation,
        DateTime? dateOfCommencement,
        string modifiedBy)
    {
        TaxId = taxId?.Trim();
        RegistrationNumber = registrationNumber?.Trim();
        DateOfIncorporation = dateOfIncorporation;
        DateOfCommencement = dateOfCommencement;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void SetParentCompany(Guid? parentCompanyId, string modifiedBy)
    {
        ParentCompanyId = parentCompanyId;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void SetAsGroup(bool isGroup, string modifiedBy)
    {
        IsGroup = isGroup;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void SetFiscalYearStartMonth(int month, string modifiedBy)
    {
        if (month < 1 || month > 12)
            throw new ArgumentException("Fiscal year start month must be between 1 and 12");

        FiscalYearStartMonth = month;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void SetLogo(string? logoUrl, string modifiedBy)
    {
        Logo = logoUrl?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void Activate(string modifiedBy)
    {
        IsActive = true;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void Deactivate(string modifiedBy)
    {
        if (ChildCompanies.Any(c => c.IsActive))
            throw new InvalidOperationException("Cannot deactivate company with active subsidiaries");

        IsActive = false;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }
}
