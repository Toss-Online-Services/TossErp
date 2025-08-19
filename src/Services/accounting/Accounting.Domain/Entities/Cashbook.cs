using TossErp.Accounting.Domain.Common;
using TossErp.Accounting.Domain.Enums;
using TossErp.Accounting.Domain.Events;

namespace TossErp.Accounting.Domain.Entities;

/// <summary>
/// Cashbook aggregate root representing the main cashbook for a tenant
/// </summary>
public class Cashbook : Entity<Guid>
{
    private readonly List<CashbookEntry> _entries = new();

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Money OpeningBalance { get; private set; } = Money.Zero();
    public DateTime OpeningBalanceDate { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation properties
    public IReadOnlyList<CashbookEntry> Entries => _entries.AsReadOnly();

    // Domain events
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected Cashbook() : base() { } // For EF Core

    public Cashbook(Guid id, string name, string tenantId, Money openingBalance, 
        string? description = null) : base(id, tenantId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Cashbook name cannot be empty", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
        OpeningBalance = openingBalance;
        OpeningBalanceDate = DateTime.UtcNow;
        IsActive = true;

        AddDomainEvent(new CashbookCreatedEvent(Id, Name, tenantId));
    }

    /// <summary>
    /// Create a new cashbook
    /// </summary>
    public static Cashbook Create(string name, string tenantId, Money openingBalance, 
        string? description = null)
    {
        return new Cashbook(Guid.NewGuid(), name, tenantId, openingBalance, description);
    }

    /// <summary>
    /// Add a new entry to the cashbook
    /// </summary>
    public void AddEntry(CashbookEntry entry)
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot add entries to inactive cashbook");

        if (entry == null)
            throw new ArgumentNullException(nameof(entry));

        _entries.Add(entry);
        AddDomainEvent(new CashbookEntryAddedEvent(Id, entry.Id, entry.Amount, entry.Type));
    }

    /// <summary>
    /// Create and add a new entry to the cashbook
    /// </summary>
    public CashbookEntry CreateAndAddEntry(DateTime transactionDate, string reference, string description,
        Money amount, EntryType type, EntryCategory category, Guid accountId,
        string? relatedEntityId = null, string? relatedEntityType = null)
    {
        var entry = CashbookEntry.Create(transactionDate, reference, description, amount, type, 
            category, accountId, TenantId, relatedEntityId, relatedEntityType);
        
        AddEntry(entry);
        return entry;
    }

    /// <summary>
    /// Get current cash balance
    /// </summary>
    public Money GetCurrentBalance()
    {
        var totalDebits = _entries.Where(e => e.Type == EntryType.Debit).Sum(e => e.Amount.Amount);
        var totalCredits = _entries.Where(e => e.Type == EntryType.Credit).Sum(e => e.Amount.Amount);
        
        var netAmount = totalDebits - totalCredits;
        
        // Add opening balance
        netAmount += OpeningBalance.Amount;
        
        return new Money(netAmount, OpeningBalance.Currency);
    }

    /// <summary>
    /// Get entries for a specific date range
    /// </summary>
    public IEnumerable<CashbookEntry> GetEntriesForDateRange(DateTime fromDate, DateTime toDate)
    {
        return _entries.Where(e => e.TransactionDate.Date >= fromDate.Date && 
                                  e.TransactionDate.Date <= toDate.Date)
                      .OrderBy(e => e.TransactionDate)
                      .ThenBy(e => e.CreatedAt);
    }

    /// <summary>
    /// Get entries for a specific account
    /// </summary>
    public IEnumerable<CashbookEntry> GetEntriesForAccount(Guid accountId)
    {
        return _entries.Where(e => e.AccountId == accountId)
                      .OrderBy(e => e.TransactionDate)
                      .ThenBy(e => e.CreatedAt);
    }

    /// <summary>
    /// Get entries by category
    /// </summary>
    public IEnumerable<CashbookEntry> GetEntriesByCategory(EntryCategory category)
    {
        return _entries.Where(e => e.Category == category)
                      .OrderBy(e => e.TransactionDate)
                      .ThenBy(e => e.CreatedAt);
    }

    /// <summary>
    /// Get unreconciled entries
    /// </summary>
    public IEnumerable<CashbookEntry> GetUnreconciledEntries()
    {
        return _entries.Where(e => !e.IsReconciled)
                      .OrderBy(e => e.TransactionDate)
                      .ThenBy(e => e.CreatedAt);
    }

    /// <summary>
    /// Update cashbook details
    /// </summary>
    public void UpdateDetails(string name, string? description, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Cashbook name cannot be empty", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
        MarkAsUpdated(updatedBy);

        AddDomainEvent(new CashbookUpdatedEvent(Id, Name, Description));
    }

    /// <summary>
    /// Deactivate the cashbook
    /// </summary>
    public void Deactivate(string deactivatedBy, string? reason = null)
    {
        if (!IsActive)
            throw new InvalidOperationException("Cashbook is already inactive");

        IsActive = false;
        MarkAsUpdated(deactivatedBy);

        AddDomainEvent(new CashbookDeactivatedEvent(Id, deactivatedBy, reason));
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
