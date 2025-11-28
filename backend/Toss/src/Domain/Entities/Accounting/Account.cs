using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;
using Toss.Domain.Entities.Stores;
using Toss.Domain.Enums;

namespace Toss.Domain.Entities.Accounting;

/// <summary>
/// Represents an accounting account (Cash or Bank) for tracking money in/out
/// </summary>
public class Account : BaseAuditableEntity, IBusinessScopedEntity
{
    public Account()
    {
        Name = string.Empty;
        CurrentBalance = 0;
        IsActive = true;
        Entries = new List<CashbookEntry>();
    }

    /// <summary>
    /// Gets or sets the business/tenant identifier
    /// </summary>
    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the account name (e.g., "Main Cash", "Standard Bank Account")
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the account type (Cash or Bank)
    /// </summary>
    public AccountType Type { get; set; }

    /// <summary>
    /// Gets or sets the current balance
    /// </summary>
    public decimal CurrentBalance { get; set; }

    /// <summary>
    /// Gets or sets whether the account is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the optional store ID for store-specific petty cash accounts
    /// </summary>
    public int? StoreId { get; set; }
    public Store? Store { get; set; }

    /// <summary>
    /// Gets or sets optional account number for bank accounts
    /// </summary>
    public string? AccountNumber { get; set; }

    /// <summary>
    /// Gets or sets optional bank name for bank accounts
    /// </summary>
    public string? BankName { get; set; }

    /// <summary>
    /// Gets or sets optional notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the cashbook entries for this account
    /// </summary>
    public ICollection<CashbookEntry> Entries { get; set; }
}

