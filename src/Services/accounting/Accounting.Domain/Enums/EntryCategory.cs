namespace TossErp.Accounting.Domain.Enums;

/// <summary>
/// Categories of cashbook entries for better organization and reporting
/// </summary>
public enum EntryCategory
{
    // Sales related
    Sale = 1000,
    SaleRefund = 1001,
    SalesTax = 1002,
    
    // Purchase related
    Purchase = 2000,
    PurchaseReturn = 2001,
    PurchaseTax = 2002,
    
    // Cash and bank
    CashReceipt = 3000,
    CashPayment = 3001,
    BankDeposit = 3002,
    BankWithdrawal = 3003,
    BankTransfer = 3004,
    BankCharges = 3005,
    BankInterest = 3006,
    
    // Expenses
    Rent = 4000,
    Utilities = 4001,
    Insurance = 4002,
    Salaries = 4003,
    Wages = 4004,
    Supplies = 4005,
    Maintenance = 4006,
    Advertising = 4007,
    Travel = 4008,
    Meals = 4009,
    
    // Income
    InterestIncome = 5000,
    Commission = 5001,
    RentalIncome = 5002,
    Dividend = 5003,
    
    // Adjustments
    Adjustment = 6000,
    Correction = 6001,
    OpeningBalance = 6002,
    ClosingBalance = 6003,
    
    // Other
    Miscellaneous = 9000,
    Other = 9999
}
