namespace TossErp.Accounting.Domain.Enums;

/// <summary>
/// Categories of accounts for better organization
/// </summary>
public enum AccountCategory
{
    // Asset Categories
    Cash = 1000,
    Bank = 1001,
    AccountsReceivable = 1002,
    Inventory = 1003,
    FixedAssets = 1004,
    PrepaidExpenses = 1005,
    
    // Liability Categories
    AccountsPayable = 2000,
    Loans = 2001,
    CreditCards = 2002,
    AccruedExpenses = 2003,
    
    // Equity Categories
    OwnerEquity = 3000,
    RetainedEarnings = 3001,
    CapitalContributions = 3002,
    Drawings = 3003,
    
    // Revenue Categories
    Sales = 4000,
    ServiceRevenue = 4001,
    InterestIncome = 4002,
    OtherIncome = 4003,
    
    // Expense Categories
    CostOfGoodsSold = 5000,
    OperatingExpenses = 5001,
    PayrollExpenses = 5002,
    RentExpense = 5003,
    UtilitiesExpense = 5004,
    InsuranceExpense = 5005,
    DepreciationExpense = 5006,
    InterestExpense = 5007,
    OtherExpenses = 5008
}
