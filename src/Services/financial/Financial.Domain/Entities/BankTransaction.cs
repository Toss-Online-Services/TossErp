namespace Financial.Domain.Entities;

/// <summary>
/// Represents a bank transaction imported from linked accounts
/// </summary>
public class BankTransaction
{
    public Guid Id { get; set; }
    public Guid LinkedBankAccountId { get; set; }
    public string TenantId { get; set; } = string.Empty;

    // Transaction Details
    public string TransactionId { get; set; } = string.Empty; // Bank's transaction ID
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime TransactionDate { get; set; }
    public DateTime PostedDate { get; set; }
    public string TransactionType { get; set; } = string.Empty; // Debit, Credit, Transfer, etc.

    // Description and Categorization
    public string Description { get; set; } = string.Empty;
    public string OriginalDescription { get; set; } = string.Empty;
    public string MerchantName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Subcategory { get; set; } = string.Empty;

    // Account Information
    public decimal RunningBalance { get; set; }
    public string CheckNumber { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;

    // Counterparty Information
    public string CounterpartyName { get; set; } = string.Empty;
    public string CounterpartyAccount { get; set; } = string.Empty;
    public string CounterpartyBank { get; set; } = string.Empty;

    // Location Information
    public string LocationCity { get; set; } = string.Empty;
    public string LocationState { get; set; } = string.Empty;
    public string LocationCountry { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    // Reconciliation
    public bool IsReconciled { get; set; }
    public DateTime? ReconciledDate { get; set; }
    public string? ReconciledBy { get; set; }
    public Guid? MatchedTransactionId { get; set; } // Matched with internal transaction

    // Flags and Status
    public bool IsPending { get; set; }
    public bool IsDisputed { get; set; }
    public bool IsTransfer { get; set; }
    public bool IsRecurring { get; set; }
    public string Status { get; set; } = string.Empty;

    // Classification
    public bool IsBusinessExpense { get; set; }
    public bool IsPersonalExpense { get; set; }
    public string ExpenseCategory { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty;

    // Additional Metadata
    public string AdditionalData { get; set; } = string.Empty; // JSON for extra fields
    public string Notes { get; set; } = string.Empty;

    // Import Information
    public DateTime ImportedDate { get; set; }
    public string ImportedBy { get; set; } = string.Empty;
    public string ImportSource { get; set; } = string.Empty;

    // Audit Trail
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation Properties
    public LinkedBankAccount LinkedBankAccount { get; set; } = null!;
}
