using TossErp.Sales.Domain.Common;
using TossErp.Sales.Domain.Enums;
using TossErp.Sales.Domain.ValueObjects;
using TossErp.Sales.Domain.Events;

namespace TossErp.Sales.Domain.Entities;

/// <summary>
/// Till/Register entity representing a POS terminal
/// </summary>
public class Till : Entity<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string Location { get; private set; } = string.Empty;
    public TillStatus Status { get; private set; }
    public Money OpeningBalance { get; private set; } = Money.Zero();
    public Money CurrentBalance { get; private set; } = Money.Zero();
    public DateTime? OpenedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public string? OpenedBy { get; private set; }
    public string? ClosedBy { get; private set; }
    public long LastReceiptSequence { get; private set; }
    public string ReceiptPrefix { get; private set; } = string.Empty;

    // Domain events
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected Till() : base() { } // For EF Core

    /// <summary>
    /// Create a new till
    /// </summary>
    public static Till Create(Guid id, string name, string description, string tenantId)
    {
        return new Till(id, name, $"TILL-{id:N}", description, "RCPT", tenantId);
    }

    public Till(Guid id, string name, string code, string location, string receiptPrefix, string tenantId) : base(id, tenantId)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Location = location ?? throw new ArgumentNullException(nameof(location));
        ReceiptPrefix = receiptPrefix ?? throw new ArgumentNullException(nameof(receiptPrefix));
        Status = TillStatus.Closed;
        LastReceiptSequence = 0;

        AddDomainEvent(new TillCreatedEvent(Id, Name, Code, tenantId));
    }

    /// <summary>
    /// Open the till for business
    /// </summary>
    public void Open(Money openingBalance, string openedBy)
    {
        if (Status != TillStatus.Closed)
            throw new InvalidOperationException($"Cannot open till with status {Status}");

        if (openingBalance.Amount < 0)
            throw new ArgumentException("Opening balance cannot be negative", nameof(openingBalance));

        Status = TillStatus.Open;
        OpeningBalance = openingBalance;
        CurrentBalance = openingBalance;
        OpenedAt = DateTime.UtcNow;
        OpenedBy = openedBy ?? throw new ArgumentNullException(nameof(openedBy));
        ClosedAt = null;
        ClosedBy = null;

        AddDomainEvent(new TillOpenedEvent(Id, openingBalance, openedBy));
    }

    /// <summary>
    /// Close the till
    /// </summary>
    public void Close(Money finalBalance, string closedBy)
    {
        if (Status != TillStatus.Open && Status != TillStatus.Reconciled)
            throw new InvalidOperationException($"Cannot close till with status {Status}");

        Status = TillStatus.Closed;
        CurrentBalance = finalBalance;
        ClosedAt = DateTime.UtcNow;
        ClosedBy = closedBy ?? throw new ArgumentNullException(nameof(closedBy));

        AddDomainEvent(new TillClosedEvent(Id, finalBalance, closedBy));
    }

    /// <summary>
    /// Suspend the till (e.g., for break)
    /// </summary>
    public void Suspend(string reason, string suspendedBy)
    {
        if (Status != TillStatus.Open)
            throw new InvalidOperationException("Can only suspend an open till");

        Status = TillStatus.Suspended;
        MarkAsUpdated(suspendedBy);

        AddDomainEvent(new TillSuspendedEvent(Id, reason, suspendedBy));
    }

    /// <summary>
    /// Resume the till from suspension
    /// </summary>
    public void Resume(string resumedBy)
    {
        if (Status != TillStatus.Suspended)
            throw new InvalidOperationException("Till is not suspended");

        Status = TillStatus.Open;
        MarkAsUpdated(resumedBy);

        AddDomainEvent(new TillResumedEvent(Id, resumedBy));
    }

    /// <summary>
    /// Start reconciliation process
    /// </summary>
    public void StartReconciliation(string reconciledBy)
    {
        if (Status != TillStatus.Open)
            throw new InvalidOperationException("Can only reconcile an open till");

        Status = TillStatus.Reconciling;
        MarkAsUpdated(reconciledBy);

        AddDomainEvent(new TillReconciliationStartedEvent(Id, reconciledBy));
    }

    /// <summary>
    /// Complete reconciliation
    /// </summary>
    public void CompleteReconciliation(Money reconciledBalance, string reconciledBy)
    {
        if (Status != TillStatus.Reconciling)
            throw new InvalidOperationException("Till is not being reconciled");

        Status = TillStatus.Reconciled;
        CurrentBalance = reconciledBalance;
        MarkAsUpdated(reconciledBy);

        AddDomainEvent(new TillReconciledEvent(Id, reconciledBalance, reconciledBy));
    }

    /// <summary>
    /// Mark till as out of order
    /// </summary>
    public void MarkOutOfOrder(string reason, string updatedBy)
    {
        Status = TillStatus.OutOfOrder;
        MarkAsUpdated(updatedBy);

        AddDomainEvent(new TillMarkedOutOfOrderEvent(Id, reason, updatedBy));
    }

    /// <summary>
    /// Restore till from out of order
    /// </summary>
    public void RestoreFromOutOfOrder(string restoredBy)
    {
        if (Status != TillStatus.OutOfOrder)
            throw new InvalidOperationException("Till is not out of order");

        Status = TillStatus.Closed;
        MarkAsUpdated(restoredBy);

        AddDomainEvent(new TillRestoredEvent(Id, restoredBy));
    }

    /// <summary>
    /// Generate next receipt number for this till
    /// </summary>
    public ReceiptNumber GenerateReceiptNumber()
    {
        if (Status != TillStatus.Open)
            throw new InvalidOperationException("Cannot generate receipt number for a closed till");

        LastReceiptSequence++;
        return ReceiptNumber.Generate(ReceiptPrefix, LastReceiptSequence);
    }

    /// <summary>
    /// Add cash to till (e.g., from sale)
    /// </summary>
    public void AddCash(Money amount, string reason = "Sale")
    {
        if (Status != TillStatus.Open)
            throw new InvalidOperationException("Cannot add cash to a closed till");

        if (amount.Amount <= 0)
            throw new ArgumentException("Amount must be positive", nameof(amount));

        CurrentBalance += amount;

        AddDomainEvent(new TillCashAddedEvent(Id, amount, reason));
    }

    /// <summary>
    /// Remove cash from till (e.g., for change or payout)
    /// </summary>
    public void RemoveCash(Money amount, string reason = "Change")
    {
        if (Status != TillStatus.Open)
            throw new InvalidOperationException("Cannot remove cash from a closed till");

        if (amount.Amount <= 0)
            throw new ArgumentException("Amount must be positive", nameof(amount));

        if (amount > CurrentBalance)
            throw new InvalidOperationException("Insufficient cash in till");

        CurrentBalance -= amount;

        AddDomainEvent(new TillCashRemovedEvent(Id, amount, reason));
    }

    /// <summary>
    /// Add domain event
    /// </summary>
    private void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Clear domain events (typically called after publishing)
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
