namespace TossErp.Accounting.Domain.Enums;

/// <summary>
/// Types of accounts in the chart of accounts
/// </summary>
public enum AccountType
{
    /// <summary>
    /// Asset accounts (debit balance)
    /// </summary>
    Asset = 1,
    
    /// <summary>
    /// Liability accounts (credit balance)
    /// </summary>
    Liability = 2,
    
    /// <summary>
    /// Equity accounts (credit balance)
    /// </summary>
    Equity = 3,
    
    /// <summary>
    /// Revenue accounts (credit balance)
    /// </summary>
    Revenue = 4,
    
    /// <summary>
    /// Expense accounts (debit balance)
    /// </summary>
    Expense = 5
}
