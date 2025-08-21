namespace Financial.Domain.Entities;

/// <summary>
/// Represents a bank account linked for financial services
/// </summary>
public class LinkedBankAccount
{
    public Guid Id { get; set; }
    public string TenantId { get; set; } = string.Empty;

    // Account Information
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string BankCode { get; set; } = string.Empty;
    public string BranchCode { get; set; } = string.Empty;
    public string SwiftCode { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty; // Checking, Savings, Business

    // Balance Information
    public decimal CurrentBalance { get; set; }
    public decimal AvailableBalance { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime LastUpdated { get; set; }

    // Account Status
    public bool IsActive { get; set; }
    public bool IsVerified { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime? VerificationDate { get; set; }

    // Connection Details
    public string ConnectionId { get; set; } = string.Empty; // External provider connection ID
    public string Provider { get; set; } = string.Empty; // Bank API provider (Plaid, Yodlee, etc.)
    public DateTime ConnectedDate { get; set; }
    public DateTime? LastSyncDate { get; set; }

    // Security
    public string EncryptedCredentials { get; set; } = string.Empty;
    public bool RequiresReauth { get; set; }

    // Sync Preferences
    public bool AutoSync { get; set; } = true;
    public int SyncFrequencyHours { get; set; } = 24;
    public DateTime? NextSyncDate { get; set; }

    // Usage Statistics
    public int TransactionCount { get; set; }
    public DateTime? LastTransactionDate { get; set; }

    // Notes
    public string Notes { get; set; } = string.Empty;

    // Audit Trail
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation Properties
    public List<BankTransaction> Transactions { get; set; } = new();
}
