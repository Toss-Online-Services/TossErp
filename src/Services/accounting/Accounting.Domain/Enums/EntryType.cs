namespace TossErp.Accounting.Domain.Enums;

/// <summary>
/// Types of accounting entries
/// </summary>
public enum EntryType
{
    /// <summary>
    /// Debit entry (increases assets/expenses, decreases liabilities/equity/revenue)
    /// </summary>
    Debit = 1,
    
    /// <summary>
    /// Credit entry (decreases assets/expenses, increases liabilities/equity/revenue)
    /// </summary>
    Credit = 2
}
