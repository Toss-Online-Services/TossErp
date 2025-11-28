namespace Toss.Domain.Enums;

/// <summary>
/// Represents the type of cashbook entry
/// </summary>
public enum CashbookEntryType
{
    /// <summary>
    /// Entry from a sale transaction
    /// </summary>
    Sale = 0,

    /// <summary>
    /// Expense entry (cash out)
    /// </summary>
    Expense = 1,

    /// <summary>
    /// Transfer between accounts
    /// </summary>
    Transfer = 2
}

