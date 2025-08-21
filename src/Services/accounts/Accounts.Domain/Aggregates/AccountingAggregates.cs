using MediatR;
using TossErp.Accounts.Domain.Entities;
using TossErp.Accounts.Domain.Enums;
using TossErp.Accounts.Domain.Events;
using TossErp.Accounts.Domain.SeedWork;
using TossErp.Accounts.Domain.ValueObjects;

namespace TossErp.Accounts.Domain.Aggregates;

/// <summary>
/// Chart of Accounts aggregate managing the hierarchical account structure
/// </summary>
public class ChartOfAccounts : AggregateRoot
{
    public string TenantId { get; private set; }
    public AccountNumber AccountNumber { get; private set; }
    public string AccountName { get; private set; }
    public AccountType AccountType { get; private set; }
    public AccountSubType? AccountSubType { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public bool AllowPosting { get; private set; }
    public Guid? ParentAccountId { get; private set; }
    public CurrencyCode DefaultCurrency { get; private set; }
    public Money CurrentBalance { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public string? ModifiedBy { get; private set; }

    private readonly List<ChartOfAccounts> _childAccounts;
    public IReadOnlyList<ChartOfAccounts> ChildAccounts => _childAccounts.AsReadOnly();

    private ChartOfAccounts()
    {
        _childAccounts = new List<ChartOfAccounts>();
        TenantId = null!;
        AccountNumber = null!;
        AccountName = null!;
        CurrentBalance = null!;
        CreatedBy = null!;
    } // EF Core

    public ChartOfAccounts(
        Guid id,
        string tenantId,
        AccountNumber accountNumber,
        string accountName,
        AccountType accountType,
        CurrencyCode defaultCurrency,
        string createdBy,
        AccountSubType? accountSubType = null,
        string? description = null,
        Guid? parentAccountId = null)
    {
        Id = id;
        TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        AccountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
        AccountName = accountName?.Trim() ?? throw new ArgumentException("Account name cannot be empty");
        AccountType = accountType;
        AccountSubType = accountSubType;
        Description = description?.Trim();
        ParentAccountId = parentAccountId;
        DefaultCurrency = defaultCurrency;
        CurrentBalance = new Money(0, defaultCurrency);
        IsActive = true;
        AllowPosting = true;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
        _childAccounts = new List<ChartOfAccounts>();

        AddDomainEvent(new AccountCreatedEvent(Id, TenantId, AccountNumber.Value, AccountName, AccountType));
    }

    public void UpdateAccountDetails(string accountName, string? description, string modifiedBy)
    {
        var oldName = AccountName;
        AccountName = accountName?.Trim() ?? throw new ArgumentException("Account name cannot be empty");
        Description = description?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));

        if (oldName != AccountName)
        {
            AddDomainEvent(new AccountUpdatedEvent(Id, TenantId, AccountNumber.Value, AccountName));
        }
    }

    public void Activate(string modifiedBy)
    {
        if (IsActive) return;

        IsActive = true;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));

        AddDomainEvent(new AccountActivatedEvent(Id, TenantId, AccountNumber.Value));
    }

    public void Deactivate(string modifiedBy)
    {
        if (!IsActive) return;

        if (ChildAccounts.Any(c => c.IsActive))
            throw new InvalidOperationException("Cannot deactivate account with active child accounts");

        if (!CurrentBalance.IsZero)
            throw new InvalidOperationException("Cannot deactivate account with non-zero balance");

        IsActive = false;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));

        AddDomainEvent(new AccountDeactivatedEvent(Id, TenantId, AccountNumber.Value));
    }

    public void EnablePosting(string modifiedBy)
    {
        AllowPosting = true;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void DisablePosting(string modifiedBy)
    {
        if (ChildAccounts.Any())
            throw new InvalidOperationException("Cannot disable posting for parent accounts");

        AllowPosting = false;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void UpdateBalance(SignedMoney amount)
    {
        if (amount.Currency != DefaultCurrency)
            throw new InvalidOperationException($"Currency mismatch. Expected {DefaultCurrency}, got {amount.Currency}");

        var newBalance = amount.Type == DebitCredit.Debit
            ? CurrentBalance.Add(amount.Amount)
            : CurrentBalance.Subtract(amount.Amount);

        CurrentBalance = newBalance;

        AddDomainEvent(new AccountBalanceUpdatedEvent(Id, TenantId, AccountNumber.Value, CurrentBalance));
    }

    public void AddChildAccount(ChartOfAccounts childAccount)
    {
        if (childAccount == null)
            throw new ArgumentNullException(nameof(childAccount));

        if (childAccount.TenantId != TenantId)
            throw new InvalidOperationException("Child account must belong to the same tenant");

        if (!AllowPosting)
            throw new InvalidOperationException("Parent accounts cannot have posting enabled");

        _childAccounts.Add(childAccount);
    }

    public bool IsParentAccount => ChildAccounts.Any();
    public bool IsLeafAccount => !ChildAccounts.Any();
    public bool CanPost => IsActive && AllowPosting && IsLeafAccount;
}

/// <summary>
/// Financial Transaction aggregate for journal entries
/// </summary>
public class FinancialTransaction : AggregateRoot
{
    public string TenantId { get; private set; }
    public string TransactionNumber { get; private set; }
    public DateTime TransactionDate { get; private set; }
    public TransactionType TransactionType { get; private set; }
    public TransactionStatus Status { get; private set; }
    public string Description { get; private set; }
    public string? Reference { get; private set; }
    public Money TotalAmount { get; private set; }
    public CurrencyCode Currency { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? ApprovedBy { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public string? PostedBy { get; private set; }
    public DateTime? PostedAt { get; private set; }

    private readonly List<JournalEntryLine> _journalLines;
    public IReadOnlyList<JournalEntryLine> JournalLines => _journalLines.AsReadOnly();

    private FinancialTransaction()
    {
        _journalLines = new List<JournalEntryLine>();
        TenantId = null!;
        TransactionNumber = null!;
        Description = null!;
        TotalAmount = null!;
        CreatedBy = null!;
    } // EF Core

    public FinancialTransaction(
        Guid id,
        string tenantId,
        string transactionNumber,
        DateTime transactionDate,
        TransactionType transactionType,
        string description,
        CurrencyCode currency,
        string createdBy,
        string? reference = null)
    {
        Id = id;
        TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        TransactionNumber = transactionNumber?.Trim() ?? throw new ArgumentException("Transaction number cannot be empty");
        TransactionDate = transactionDate.Date;
        TransactionType = transactionType;
        Description = description?.Trim() ?? throw new ArgumentException("Description cannot be empty");
        Reference = reference?.Trim();
        Currency = currency;
        TotalAmount = new Money(0, currency);
        Status = TransactionStatus.Draft;
        CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
        CreatedAt = DateTime.UtcNow;
        _journalLines = new List<JournalEntryLine>();

        AddDomainEvent(new TransactionCreatedEvent(Id, TenantId, TransactionNumber, TransactionType));
    }

    public void AddJournalLine(JournalEntryLine line)
    {
        if (line == null)
            throw new ArgumentNullException(nameof(line));

        if (Status != TransactionStatus.Draft)
            throw new InvalidOperationException("Cannot modify non-draft transactions");

        if (line.Amount.Currency != Currency)
            throw new InvalidOperationException($"Currency mismatch. Expected {Currency}, got {line.Amount.Currency}");

        _journalLines.Add(line);
        RecalculateTotal();
    }

    public void RemoveJournalLine(Guid lineId)
    {
        if (Status != TransactionStatus.Draft)
            throw new InvalidOperationException("Cannot modify non-draft transactions");

        var line = _journalLines.FirstOrDefault(l => l.Id == lineId);
        if (line != null)
        {
            _journalLines.Remove(line);
            RecalculateTotal();
        }
    }

    public void Submit(string submittedBy)
    {
        if (Status != TransactionStatus.Draft)
            throw new InvalidOperationException("Only draft transactions can be submitted");

        ValidateTransaction();

        Status = TransactionStatus.Pending;
        
        AddDomainEvent(new TransactionSubmittedEvent(Id, TenantId, TransactionNumber, submittedBy));
    }

    public void Approve(string approvedBy)
    {
        if (Status != TransactionStatus.Pending)
            throw new InvalidOperationException("Only pending transactions can be approved");

        Status = TransactionStatus.Approved;
        ApprovedBy = approvedBy ?? throw new ArgumentNullException(nameof(approvedBy));
        ApprovedAt = DateTime.UtcNow;

        AddDomainEvent(new TransactionApprovedEvent(Id, TenantId, TransactionNumber, approvedBy));
    }

    public void Reject(string rejectedBy, string reason)
    {
        if (Status != TransactionStatus.Pending)
            throw new InvalidOperationException("Only pending transactions can be rejected");

        Status = TransactionStatus.Rejected;
        
        AddDomainEvent(new TransactionRejectedEvent(Id, TenantId, TransactionNumber, rejectedBy, reason));
    }

    public void Post(string postedBy)
    {
        if (Status != TransactionStatus.Approved)
            throw new InvalidOperationException("Only approved transactions can be posted");

        Status = TransactionStatus.Posted;
        PostedBy = postedBy ?? throw new ArgumentNullException(nameof(postedBy));
        PostedAt = DateTime.UtcNow;

        AddDomainEvent(new TransactionPostedEvent(Id, TenantId, TransactionNumber, _journalLines.ToList()));
    }

    public void Cancel(string cancelledBy, string reason)
    {
        if (Status == TransactionStatus.Posted)
            throw new InvalidOperationException("Posted transactions cannot be cancelled");

        Status = TransactionStatus.Cancelled;
        
        AddDomainEvent(new TransactionCancelledEvent(Id, TenantId, TransactionNumber, cancelledBy, reason));
    }

    private void ValidateTransaction()
    {
        if (!_journalLines.Any())
            throw new InvalidOperationException("Transaction must have at least one journal line");

        var totalDebits = _journalLines.Where(l => l.IsDebit).Sum(l => l.Amount.Amount.Amount);
        var totalCredits = _journalLines.Where(l => l.IsCredit).Sum(l => l.Amount.Amount.Amount);

        if (Math.Abs(totalDebits - totalCredits) > 0.01m) // Allow for rounding differences
            throw new InvalidOperationException("Transaction is not balanced. Debits must equal credits.");
    }

    private void RecalculateTotal()
    {
        var total = _journalLines.Sum(l => Math.Abs(l.Amount.Amount.Amount));
        TotalAmount = new Money(total / 2, Currency); // Divide by 2 because we count both debits and credits
    }

    public bool IsBalanced
    {
        get
        {
            var totalDebits = _journalLines.Where(l => l.IsDebit).Sum(l => l.Amount.Amount.Amount);
            var totalCredits = _journalLines.Where(l => l.IsCredit).Sum(l => l.Amount.Amount.Amount);
            return Math.Abs(totalDebits - totalCredits) <= 0.01m;
        }
    }

    public bool CanBeModified => Status == TransactionStatus.Draft;
    public bool CanBeApproved => Status == TransactionStatus.Pending && IsBalanced;
    public bool CanBePosted => Status == TransactionStatus.Approved;
}

/// <summary>
/// Invoice aggregate for billing and accounts receivable
/// </summary>
public class Invoice : AggregateRoot
{
    public string TenantId { get; private set; }
    public string InvoiceNumber { get; private set; }
    public InvoiceType InvoiceType { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public Guid CustomerId { get; private set; }
    public string CustomerName { get; private set; }
    public DateTime InvoiceDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public CurrencyCode Currency { get; private set; }
    public Money SubTotal { get; private set; }
    public Money TotalTax { get; private set; }
    public Money TotalAmount { get; private set; }
    public Money PaidAmount { get; private set; }
    public Money OutstandingAmount { get; private set; }
    public string? Notes { get; private set; }
    public string? Terms { get; private set; }
    public BillingPeriod? BillingPeriod { get; private set; }
    public Guid? SubscriptionId { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<InvoiceLine> _lines;
    public IReadOnlyList<InvoiceLine> Lines => _lines.AsReadOnly();

    private readonly List<PaymentLine> _payments;
    public IReadOnlyList<PaymentLine> Payments => _payments.AsReadOnly();

    private Invoice()
    {
        _lines = new List<InvoiceLine>();
        _payments = new List<PaymentLine>();
        TenantId = null!;
        InvoiceNumber = null!;
        CustomerName = null!;
        SubTotal = null!;
        TotalTax = null!;
        TotalAmount = null!;
        PaidAmount = null!;
        OutstandingAmount = null!;
        CreatedBy = null!;
    } // EF Core

    public Invoice(
        Guid id,
        string tenantId,
        string invoiceNumber,
        InvoiceType invoiceType,
        Guid customerId,
        string customerName,
        DateTime invoiceDate,
        DateTime dueDate,
        CurrencyCode currency,
        string createdBy,
        string? notes = null,
        string? terms = null,
        BillingPeriod? billingPeriod = null,
        Guid? subscriptionId = null)
    {
        Id = id;
        TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        InvoiceNumber = invoiceNumber?.Trim() ?? throw new ArgumentException("Invoice number cannot be empty");
        InvoiceType = invoiceType;
        CustomerId = customerId;
        CustomerName = customerName?.Trim() ?? throw new ArgumentException("Customer name cannot be empty");
        InvoiceDate = invoiceDate.Date;
        DueDate = dueDate.Date;
        Currency = currency;
        Notes = notes?.Trim();
        Terms = terms?.Trim();
        BillingPeriod = billingPeriod;
        SubscriptionId = subscriptionId;
        CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
        CreatedAt = DateTime.UtcNow;
        Status = InvoiceStatus.Draft;

        SubTotal = new Money(0, currency);
        TotalTax = new Money(0, currency);
        TotalAmount = new Money(0, currency);
        PaidAmount = new Money(0, currency);
        OutstandingAmount = new Money(0, currency);

        _lines = new List<InvoiceLine>();
        _payments = new List<PaymentLine>();

        AddDomainEvent(new InvoiceCreatedEvent(Id, TenantId, InvoiceNumber, CustomerId, InvoiceType));
    }

    public void AddLine(InvoiceLine line)
    {
        if (line == null)
            throw new ArgumentNullException(nameof(line));

        if (Status != InvoiceStatus.Draft)
            throw new InvalidOperationException("Cannot modify non-draft invoices");

        _lines.Add(line);
        RecalculateAmounts();
    }

    public void RemoveLine(Guid lineId)
    {
        if (Status != InvoiceStatus.Draft)
            throw new InvalidOperationException("Cannot modify non-draft invoices");

        var line = _lines.FirstOrDefault(l => l.Id == lineId);
        if (line != null)
        {
            _lines.Remove(line);
            RecalculateAmounts();
        }
    }

    public void Submit()
    {
        if (Status != InvoiceStatus.Draft)
            throw new InvalidOperationException("Only draft invoices can be submitted");

        if (!_lines.Any())
            throw new InvalidOperationException("Invoice must have at least one line");

        Status = InvoiceStatus.Pending;
        
        AddDomainEvent(new InvoiceSubmittedEvent(Id, TenantId, InvoiceNumber, TotalAmount));
    }

    public void Approve(string approvedBy)
    {
        if (Status != InvoiceStatus.Pending)
            throw new InvalidOperationException("Only pending invoices can be approved");

        Status = InvoiceStatus.Approved;
        
        AddDomainEvent(new InvoiceApprovedEvent(Id, TenantId, InvoiceNumber, approvedBy));
    }

    public void Send()
    {
        if (Status != InvoiceStatus.Approved)
            throw new InvalidOperationException("Only approved invoices can be sent");

        Status = InvoiceStatus.Sent;
        
        AddDomainEvent(new InvoiceSentEvent(Id, TenantId, InvoiceNumber, CustomerId, TotalAmount, DueDate));
    }

    public void MarkAsPaid(Money amount, DateTime paymentDate, string paymentReference)
    {
        if (Status != InvoiceStatus.Sent && Status != InvoiceStatus.PartiallyPaid)
            throw new InvalidOperationException("Only sent or partially paid invoices can receive payments");

        if (amount.Currency != Currency)
            throw new InvalidOperationException($"Currency mismatch. Expected {Currency}, got {amount.Currency}");

        var newPaidAmount = PaidAmount.Add(amount);
        if (newPaidAmount.Amount > TotalAmount.Amount)
            throw new InvalidOperationException("Payment amount exceeds invoice total");

        PaidAmount = newPaidAmount;
        OutstandingAmount = TotalAmount.Subtract(PaidAmount);

        Status = OutstandingAmount.IsZero ? InvoiceStatus.Paid : InvoiceStatus.PartiallyPaid;

        AddDomainEvent(new InvoicePaymentReceivedEvent(Id, TenantId, InvoiceNumber, amount, paymentDate, paymentReference));

        if (Status == InvoiceStatus.Paid)
        {
            AddDomainEvent(new InvoicePaidEvent(Id, TenantId, InvoiceNumber, TotalAmount, paymentDate));
        }
    }

    public void MarkAsOverdue()
    {
        if (Status != InvoiceStatus.Sent && Status != InvoiceStatus.PartiallyPaid)
            return;

        if (DateTime.UtcNow.Date <= DueDate)
            return;

        Status = Status == InvoiceStatus.PartiallyPaid ? InvoiceStatus.PartiallyPaid : InvoiceStatus.Overdue;
        
        AddDomainEvent(new InvoiceOverdueEvent(Id, TenantId, InvoiceNumber, OutstandingAmount, DueDate));
    }

    public void Cancel(string reason)
    {
        if (Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("Paid invoices cannot be cancelled");

        Status = InvoiceStatus.Cancelled;
        
        AddDomainEvent(new InvoiceCancelledEvent(Id, TenantId, InvoiceNumber, reason));
    }

    private void RecalculateAmounts()
    {
        SubTotal = _lines.Aggregate(new Money(0, Currency), (sum, line) => sum.Add(line.LineTotal));
        TotalTax = _lines.Where(l => l.TaxAmount != null)
                        .Aggregate(new Money(0, Currency), (sum, line) => sum.Add(line.TaxAmount!));
        TotalAmount = SubTotal.Add(TotalTax);
        OutstandingAmount = TotalAmount.Subtract(PaidAmount);
    }

    public bool IsOverdue => Status != InvoiceStatus.Paid && Status != InvoiceStatus.Cancelled && DateTime.UtcNow.Date > DueDate;
    public bool IsPaid => Status == InvoiceStatus.Paid;
    public bool CanBeModified => Status == InvoiceStatus.Draft;
    public decimal PaymentPercentage => TotalAmount.Amount > 0 ? (PaidAmount.Amount / TotalAmount.Amount) * 100 : 0;
}

/// <summary>
/// Budget aggregate for financial planning and control
/// </summary>
public class Budget : AggregateRoot
{
    public string TenantId { get; private set; }
    public string BudgetName { get; private set; }
    public string? Description { get; private set; }
    public BudgetType BudgetType { get; private set; }
    public BudgetStatus Status { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public CurrencyCode Currency { get; private set; }
    public Money TotalBudget { get; private set; }
    public Money ActualSpent { get; private set; }
    public Money Committed { get; private set; }
    public Money Available { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? ApprovedBy { get; private set; }
    public DateTime? ApprovedAt { get; private set; }

    private readonly List<BudgetAllocation> _allocations;
    public IReadOnlyList<BudgetAllocation> Allocations => _allocations.AsReadOnly();

    private Budget()
    {
        _allocations = new List<BudgetAllocation>();
        TenantId = null!;
        BudgetName = null!;
        TotalBudget = null!;
        ActualSpent = null!;
        Committed = null!;
        Available = null!;
        CreatedBy = null!;
    } // EF Core

    public Budget(
        Guid id,
        string tenantId,
        string budgetName,
        BudgetType budgetType,
        DateTime startDate,
        DateTime endDate,
        CurrencyCode currency,
        Money totalBudget,
        string createdBy,
        string? description = null)
    {
        Id = id;
        TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        BudgetName = budgetName?.Trim() ?? throw new ArgumentException("Budget name cannot be empty");
        Description = description?.Trim();
        BudgetType = budgetType;
        StartDate = startDate.Date;
        EndDate = endDate.Date;
        Currency = currency;
        TotalBudget = totalBudget ?? throw new ArgumentNullException(nameof(totalBudget));
        ActualSpent = new Money(0, currency);
        Committed = new Money(0, currency);
        Available = totalBudget;
        Status = BudgetStatus.Draft;
        CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
        CreatedAt = DateTime.UtcNow;
        _allocations = new List<BudgetAllocation>();

        if (endDate <= startDate)
            throw new ArgumentException("End date must be after start date");

        AddDomainEvent(new BudgetCreatedEvent(Id, TenantId, BudgetName, BudgetType, TotalBudget));
    }

    public void AddAllocation(BudgetAllocation allocation)
    {
        if (allocation == null)
            throw new ArgumentNullException(nameof(allocation));

        if (Status != BudgetStatus.Draft)
            throw new InvalidOperationException("Cannot modify non-draft budgets");

        if (allocation.Currency != Currency)
            throw new InvalidOperationException($"Currency mismatch. Expected {Currency}, got {allocation.Currency}");

        _allocations.Add(allocation);
        RecalculateAvailableBudget();
    }

    public void RemoveAllocation(Guid allocationId)
    {
        if (Status != BudgetStatus.Draft)
            throw new InvalidOperationException("Cannot modify non-draft budgets");

        var allocation = _allocations.FirstOrDefault(a => a.Id == allocationId);
        if (allocation != null)
        {
            _allocations.Remove(allocation);
            RecalculateAvailableBudget();
        }
    }

    public void Submit()
    {
        if (Status != BudgetStatus.Draft)
            throw new InvalidOperationException("Only draft budgets can be submitted");

        ValidateBudget();

        Status = BudgetStatus.PendingApproval;
        
        AddDomainEvent(new BudgetSubmittedEvent(Id, TenantId, BudgetName));
    }

    public void Approve(string approvedBy)
    {
        if (Status != BudgetStatus.PendingApproval)
            throw new InvalidOperationException("Only pending budgets can be approved");

        Status = BudgetStatus.Approved;
        ApprovedBy = approvedBy ?? throw new ArgumentNullException(nameof(approvedBy));
        ApprovedAt = DateTime.UtcNow;

        AddDomainEvent(new BudgetApprovedEvent(Id, TenantId, BudgetName, approvedBy));
    }

    public void Activate()
    {
        if (Status != BudgetStatus.Approved)
            throw new InvalidOperationException("Only approved budgets can be activated");

        Status = BudgetStatus.Active;
        
        AddDomainEvent(new BudgetActivatedEvent(Id, TenantId, BudgetName));
    }

    public void RecordExpense(Money amount, string description, Guid? allocationId = null)
    {
        if (Status != BudgetStatus.Active)
            throw new InvalidOperationException("Only active budgets can record expenses");

        if (amount.Currency != Currency)
            throw new InvalidOperationException($"Currency mismatch. Expected {Currency}, got {amount.Currency}");

        ActualSpent = ActualSpent.Add(amount);

        if (allocationId.HasValue)
        {
            var allocation = _allocations.FirstOrDefault(a => a.Id == allocationId.Value);
            allocation?.RecordExpense(amount);
        }

        RecalculateAvailableBudget();

        AddDomainEvent(new BudgetExpenseRecordedEvent(Id, TenantId, BudgetName, amount, description));

        CheckBudgetThresholds();
    }

    public void CommitFunds(Money amount, string description, Guid? allocationId = null)
    {
        if (Status != BudgetStatus.Active)
            throw new InvalidOperationException("Only active budgets can commit funds");

        if (amount.Currency != Currency)
            throw new InvalidOperationException($"Currency mismatch. Expected {Currency}, got {amount.Currency}");

        if (Available.Amount < amount.Amount)
            throw new InvalidOperationException("Insufficient available budget");

        Committed = Committed.Add(amount);

        if (allocationId.HasValue)
        {
            var allocation = _allocations.FirstOrDefault(a => a.Id == allocationId.Value);
            allocation?.CommitFunds(amount);
        }

        RecalculateAvailableBudget();

        AddDomainEvent(new BudgetFundsCommittedEvent(Id, TenantId, BudgetName, amount, description));
    }

    public void ReleaseFunds(Money amount, Guid? allocationId = null)
    {
        if (amount.Currency != Currency)
            throw new InvalidOperationException($"Currency mismatch. Expected {Currency}, got {amount.Currency}");

        if (Committed.Amount < amount.Amount)
            throw new InvalidOperationException("Cannot release more than committed amount");

        Committed = Committed.Subtract(amount);

        if (allocationId.HasValue)
        {
            var allocation = _allocations.FirstOrDefault(a => a.Id == allocationId.Value);
            allocation?.ReleaseFunds(amount);
        }

        RecalculateAvailableBudget();
    }

    public void Close()
    {
        if (Status != BudgetStatus.Active)
            throw new InvalidOperationException("Only active budgets can be closed");

        Status = BudgetStatus.Closed;
        
        AddDomainEvent(new BudgetClosedEvent(Id, TenantId, BudgetName, ActualSpent, TotalBudget));
    }

    private void ValidateBudget()
    {
        var totalAllocated = _allocations.Sum(a => a.AllocatedAmount.Amount);
        if (totalAllocated > TotalBudget.Amount)
            throw new InvalidOperationException("Total allocations exceed budget amount");
    }

    private void RecalculateAvailableBudget()
    {
        Available = TotalBudget.Subtract(ActualSpent).Subtract(Committed);
    }

    private void CheckBudgetThresholds()
    {
        var utilizationPercentage = (ActualSpent.Amount / TotalBudget.Amount) * 100;

        if (utilizationPercentage >= 90)
        {
            AddDomainEvent(new BudgetThresholdExceededEvent(Id, TenantId, BudgetName, utilizationPercentage, 90));
        }
        else if (utilizationPercentage >= 75)
        {
            AddDomainEvent(new BudgetThresholdExceededEvent(Id, TenantId, BudgetName, utilizationPercentage, 75));
        }
    }

    public decimal UtilizationPercentage => TotalBudget.Amount > 0 ? (ActualSpent.Amount / TotalBudget.Amount) * 100 : 0;
    public bool IsOverBudget => ActualSpent.Amount > TotalBudget.Amount;
    public bool IsActive => Status == BudgetStatus.Active;
    public bool CanBeModified => Status == BudgetStatus.Draft;
}
