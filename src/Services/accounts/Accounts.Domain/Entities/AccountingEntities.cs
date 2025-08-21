using TossErp.Accounts.Domain.Enums;
using TossErp.Accounts.Domain.SeedWork;
using TossErp.Accounts.Domain.ValueObjects;

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
    public DateTime CreatedAt { get; private set; }

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
        string? reference = null)
    {
        Id = id;
        JournalEntryId = journalEntryId;
        AccountId = accountId;
        AccountName = accountName?.Trim() ?? throw new ArgumentException("Account name cannot be empty");
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        Description = description?.Trim();
        CostCenterId = costCenterId;
        Reference = reference?.Trim();
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
    public Money LineTotal { get; private set; }
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

    public Money TotalWithTax => TaxAmount != null ? LineTotal.Add(TaxAmount) : LineTotal;
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
    public string CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
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
