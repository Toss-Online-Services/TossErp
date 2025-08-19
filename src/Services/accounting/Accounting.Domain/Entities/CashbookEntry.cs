using TossErp.Accounting.Domain.Common;
using TossErp.Accounting.Domain.Enums;

namespace TossErp.Accounting.Domain.Entities;

/// <summary>
/// Cashbook entry representing a single transaction in the cashbook
/// </summary>
public class CashbookEntry : Entity<Guid>
{
    public DateTime TransactionDate { get; private set; }
    public string Reference { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Money Amount { get; private set; } = Money.Zero();
    public EntryType Type { get; private set; }
    public EntryCategory Category { get; private set; }
    public string? RelatedEntityId { get; private set; }
    public string? RelatedEntityType { get; private set; }
    public bool IsReconciled { get; private set; }
    public DateTime? ReconciledDate { get; private set; }
    public string? ReconciledBy { get; private set; }

    // Navigation properties
    public Guid AccountId { get; private set; }
    public virtual Account Account { get; private set; } = null!;

    protected CashbookEntry() : base() { } // For EF Core

    public CashbookEntry(Guid id, DateTime transactionDate, string reference, string description, 
        Money amount, EntryType type, EntryCategory category, Guid accountId, string tenantId,
        string? relatedEntityId = null, string? relatedEntityType = null) : base(id, tenantId)
    {
        if (string.IsNullOrWhiteSpace(reference))
            throw new ArgumentException("Reference cannot be empty", nameof(reference));
        
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));
        
        if (amount.Amount <= 0)
            throw new ArgumentException("Amount must be greater than zero", nameof(amount));

        TransactionDate = transactionDate;
        Reference = reference.Trim();
        Description = description.Trim();
        Amount = amount;
        Type = type;
        Category = category;
        AccountId = accountId;
        RelatedEntityId = relatedEntityId;
        RelatedEntityType = relatedEntityType;
        IsReconciled = false;
    }

    /// <summary>
    /// Create a new cashbook entry
    /// </summary>
    public static CashbookEntry Create(DateTime transactionDate, string reference, string description,
        Money amount, EntryType type, EntryCategory category, Guid accountId, string tenantId,
        string? relatedEntityId = null, string? relatedEntityType = null)
    {
        return new CashbookEntry(Guid.NewGuid(), transactionDate, reference, description, 
            amount, type, category, accountId, tenantId, relatedEntityId, relatedEntityType);
    }

    /// <summary>
    /// Update entry details
    /// </summary>
    public void UpdateDetails(string description, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        Description = description.Trim();
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Mark entry as reconciled
    /// </summary>
    public void MarkAsReconciled(string reconciledBy)
    {
        if (IsReconciled)
            throw new InvalidOperationException("Entry is already reconciled");

        IsReconciled = true;
        ReconciledDate = DateTime.UtcNow;
        ReconciledBy = reconciledBy;
        MarkAsUpdated(reconciledBy);
    }

    /// <summary>
    /// Mark entry as unreconciled
    /// </summary>
    public void MarkAsUnreconciled(string updatedBy)
    {
        if (!IsReconciled)
            throw new InvalidOperationException("Entry is not reconciled");

        IsReconciled = false;
        ReconciledDate = null;
        ReconciledBy = null;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Get the effective amount (positive for debits, negative for credits)
    /// </summary>
    public Money GetEffectiveAmount()
    {
        return Type == EntryType.Debit ? Amount : new Money(-Amount.Amount, Amount.Currency);
    }

    /// <summary>
    /// Check if entry is a debit
    /// </summary>
    public bool IsDebit => Type == EntryType.Debit;

    /// <summary>
    /// Check if entry is a credit
    /// </summary>
    public bool IsCredit => Type == EntryType.Credit;
}
