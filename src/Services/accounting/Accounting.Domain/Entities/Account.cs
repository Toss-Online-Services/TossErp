using TossErp.Accounting.Domain.Common;
using TossErp.Accounting.Domain.Enums;

namespace TossErp.Accounting.Domain.Entities;

/// <summary>
/// Account entity representing a chart of accounts entry
/// </summary>
public class Account : Entity<Guid>
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public AccountType Type { get; private set; }
    public AccountCategory Category { get; private set; }
    public bool IsActive { get; private set; }
    public Money OpeningBalance { get; private set; } = Money.Zero();
    public DateTime OpeningBalanceDate { get; private set; }

    // Navigation properties
    public virtual ICollection<CashbookEntry> Entries { get; private set; } = new List<CashbookEntry>();

    protected Account() : base() { } // For EF Core

    public Account(Guid id, string code, string name, AccountType type, AccountCategory category, 
        string tenantId, Money openingBalance, string? description = null) : base(id, tenantId)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Account code cannot be empty", nameof(code));
        
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Account name cannot be empty", nameof(name));

        Code = code.Trim().ToUpper();
        Name = name.Trim();
        Description = description?.Trim();
        Type = type;
        Category = category;
        IsActive = true;
        OpeningBalance = openingBalance;
        OpeningBalanceDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Create a new account
    /// </summary>
    public static Account Create(string code, string name, AccountType type, AccountCategory category, 
        string tenantId, Money openingBalance, string? description = null)
    {
        return new Account(Guid.NewGuid(), code, name, type, category, tenantId, openingBalance, description);
    }

    /// <summary>
    /// Update account details
    /// </summary>
    public void UpdateDetails(string name, string? description, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Account name cannot be empty", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Activate the account
    /// </summary>
    public void Activate(string activatedBy)
    {
        if (IsActive)
            throw new InvalidOperationException("Account is already active");

        IsActive = true;
        MarkAsUpdated(activatedBy);
    }

    /// <summary>
    /// Deactivate the account
    /// </summary>
    public void Deactivate(string deactivatedBy, string? reason = null)
    {
        if (!IsActive)
            throw new InvalidOperationException("Account is already inactive");

        IsActive = false;
        MarkAsUpdated(deactivatedBy);
    }

    /// <summary>
    /// Get current balance for the account
    /// </summary>
    public Money GetCurrentBalance()
    {
        var totalDebits = Entries.Where(e => e.Type == EntryType.Debit).Sum(e => e.Amount.Amount);
        var totalCredits = Entries.Where(e => e.Type == EntryType.Credit).Sum(e => e.Amount.Amount);
        
        var netAmount = totalDebits - totalCredits;
        
        // Add opening balance
        netAmount += OpeningBalance.Amount;
        
        return new Money(netAmount, OpeningBalance.Currency);
    }

    /// <summary>
    /// Check if account can be deleted (no entries)
    /// </summary>
    public bool CanBeDeleted => !Entries.Any();

    /// <summary>
    /// Get account display name
    /// </summary>
    public string DisplayName => $"{Code} - {Name}";
}
