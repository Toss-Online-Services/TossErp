namespace Financial.Application.DTOs;

/// <summary>
/// DTO for displaying linked bank account information
/// </summary>
public class LinkedBankAccountDto
{
    public Guid Id { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public decimal CurrentBalance { get; set; }
    public decimal AvailableBalance { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime LastUpdated { get; set; }
    public bool IsActive { get; set; }
    public bool IsVerified { get; set; }
    public bool IsPrimary { get; set; }
    public string Provider { get; set; } = string.Empty;
    public DateTime ConnectedDate { get; set; }
    public DateTime? LastSyncDate { get; set; }
    public bool RequiresReauth { get; set; }
    public int TransactionCount { get; set; }
    public DateTime? LastTransactionDate { get; set; }
}

/// <summary>
/// DTO for linking a new bank account
/// </summary>
public class LinkBankAccountDto
{
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string BankCode { get; set; } = string.Empty;
    public string BranchCode { get; set; } = string.Empty;
    public string SwiftCode { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public bool AutoSync { get; set; } = true;
    public int SyncFrequencyHours { get; set; } = 24;
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// DTO for bank transaction information
/// </summary>
public class BankTransactionDto
{
    public Guid Id { get; set; }
    public Guid LinkedBankAccountId { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime TransactionDate { get; set; }
    public DateTime PostedDate { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string OriginalDescription { get; set; } = string.Empty;
    public string MerchantName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Subcategory { get; set; } = string.Empty;
    public decimal RunningBalance { get; set; }
    public string CounterpartyName { get; set; } = string.Empty;
    public string LocationCity { get; set; } = string.Empty;
    public bool IsReconciled { get; set; }
    public DateTime? ReconciledDate { get; set; }
    public bool IsPending { get; set; }
    public bool IsBusinessExpense { get; set; }
    public string ExpenseCategory { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
}

/// <summary>
/// DTO for updating transaction categorization
/// </summary>
public class UpdateTransactionCategoryDto
{
    public Guid TransactionId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Subcategory { get; set; } = string.Empty;
    public bool IsBusinessExpense { get; set; }
    public string ExpenseCategory { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// DTO for bank reconciliation
/// </summary>
public class ReconcileTransactionDto
{
    public Guid TransactionId { get; set; }
    public Guid? MatchedTransactionId { get; set; }
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// DTO for financial summary across all accounts
/// </summary>
public class FinancialSummaryDto
{
    public decimal TotalBalance { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetCashFlow { get; set; }
    public int TotalTransactions { get; set; }
    public int UnreconciledTransactions { get; set; }
    public List<AccountSummaryDto> AccountSummaries { get; set; } = new();
    public List<CategorySummaryDto> ExpenseCategories { get; set; } = new();
}

/// <summary>
/// DTO for individual account summary
/// </summary>
public class AccountSummaryDto
{
    public Guid AccountId { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public decimal CurrentBalance { get; set; }
    public decimal MonthlyIncome { get; set; }
    public decimal MonthlyExpenses { get; set; }
    public int TransactionCount { get; set; }
}

/// <summary>
/// DTO for expense category summary
/// </summary>
public class CategorySummaryDto
{
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int TransactionCount { get; set; }
    public decimal Percentage { get; set; }
}
