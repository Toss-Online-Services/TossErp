namespace TossErp.Accounts.Domain.Enums;

/// <summary>
/// Chart of Accounts classification - following ERPNext accounting structure
/// </summary>
public enum AccountType
{
    Asset = 1,
    Liability = 2,
    Equity = 3,
    Income = 4,
    Expense = 5
}

/// <summary>
/// Sub-classification of account types for detailed reporting
/// </summary>
public enum AccountSubType
{
    // Assets
    CurrentAsset = 1,
    FixedAsset = 2,
    IntangibleAsset = 3,
    Investment = 4,
    
    // Liabilities  
    CurrentLiability = 5,
    LongTermLiability = 6,
    
    // Equity
    Capital = 7,
    RetainedEarnings = 8,
    
    // Income
    DirectIncome = 9,
    IndirectIncome = 10,
    
    // Expenses
    DirectExpense = 11,
    IndirectExpense = 12
}

/// <summary>
/// Transaction types for journal entries
/// </summary>
public enum TransactionType
{
    JournalEntry = 1,
    Payment = 2,
    Receipt = 3,
    Sale = 4,
    Purchase = 5,
    Adjustment = 6,
    OpeningBalance = 7,
    Subscription = 8,
    Refund = 9,
    Transfer = 10
}

/// <summary>
/// Transaction status for approval workflow
/// </summary>
public enum TransactionStatus
{
    Draft = 1,
    Submitted = 2,
    Approved = 3,
    Posted = 4,
    Cancelled = 5,
    Rejected = 6,
    Pending = 7
}

/// <summary>
/// Debit or Credit nature of transaction
/// </summary>
public enum DebitCredit
{
    Debit = 1,
    Credit = 2
}

/// <summary>
/// Invoice status for billing management
/// </summary>
public enum InvoiceStatus
{
    Draft = 1,
    Sent = 2,
    Paid = 3,
    Partially_Paid = 4,
    Overdue = 5,
    Cancelled = 6,
    Voided = 7,
    Pending = 8,
    Approved = 9,
    PartiallyPaid = 10  // Alternative naming
}

/// <summary>
/// Invoice types for different billing scenarios
/// </summary>
public enum InvoiceType
{
    Standard = 1,
    Subscription = 2,
    OneTime = 3,
    Recurring = 4,
    Credit = 5,
    Debit = 6,
    Proforma = 7
}

/// <summary>
/// Payment method options
/// </summary>
public enum PaymentMethod
{
    Cash = 1,
    BankTransfer = 2,
    CreditCard = 3,
    DebitCard = 4,
    PayPal = 5,
    Stripe = 6,
    Check = 7,
    Wire = 8,
    Crypto = 9,
    Other = 10
}

/// <summary>
/// Payment status tracking
/// </summary>
public enum PaymentStatus
{
    Pending = 1,
    Completed = 2,
    Failed = 3,
    Cancelled = 4,
    Refunded = 5,
    PartiallyRefunded = 6,
    Disputed = 7
}

/// <summary>
/// Payment type classification
/// </summary>
public enum PaymentType
{
    CustomerPayment = 1,
    VendorPayment = 2,
    EmployeePayment = 3,
    Refund = 4,
    Adjustment = 5,
    Internal = 6,
    Interest = 7,
    Fee = 8,
    Other = 9
}

/// <summary>
/// Tax types for different jurisdictions
/// </summary>
public enum TaxType
{
    VAT = 1,
    GST = 2,
    SalesTax = 3,
    ServiceTax = 4,
    WithholdingTax = 5,
    Custom = 6,
    None = 7
}

/// <summary>
/// Currency codes for multi-currency support
/// </summary>
public enum CurrencyCode
{
    USD = 1,
    EUR = 2,
    GBP = 3,
    JPY = 4,
    CAD = 5,
    AUD = 6,
    CHF = 7,
    CNY = 8,
    INR = 9,
    ZAR = 10
}

/// <summary>
/// Billing frequency for subscription management
/// </summary>
public enum BillingFrequency
{
    OneTime = 1,
    Daily = 2,
    Weekly = 3,
    Monthly = 4,
    Quarterly = 5,
    SemiAnnually = 6,
    Annually = 7,
    Biennial = 8
}

/// <summary>
/// Vendor types for supplier classification
/// </summary>
public enum VendorType
{
    Individual = 1,
    Company = 2,
    Cooperative = 3,
    NonProfit = 4,
    Government = 5,
    Contractor = 6,
    Consultant = 7,
    Supplier = 8,
    ServiceProvider = 9
}

/// <summary>
/// Vendor status for supplier management
/// </summary>
public enum VendorStatus
{
    Active = 1,
    Inactive = 2,
    Pending = 3,
    Suspended = 4,
    Blacklisted = 5
}

/// <summary>
/// Budget types for financial planning
/// </summary>
public enum BudgetType
{
    Operating = 1,
    Capital = 2,
    Cash = 3,
    Revenue = 4,
    Expense = 5,
    Project = 6,
    Department = 7
}

/// <summary>
/// Budget status for approval workflow
/// </summary>
public enum BudgetStatus
{
    Draft = 1,
    Active = 2,
    Exceeded = 3,
    Completed = 4,
    Cancelled = 5,
    Revised = 6,
    PendingApproval = 7,
    Approved = 8,
    Closed = 9
}

/// <summary>
/// Financial report types
/// </summary>
public enum ReportType
{
    BalanceSheet = 1,
    IncomeStatement = 2,
    CashFlow = 3,
    TrialBalance = 4,
    GeneralLedger = 5,
    AccountsReceivable = 6,
    AccountsPayable = 7,
    ProfitAndLoss = 8
}

/// <summary>
/// Cost center types for departmental accounting
/// </summary>
public enum CostCenterType
{
    Department = 1,
    Project = 2,
    Location = 3,
    Product = 4,
    Service = 5,
    Customer = 6,
    Region = 7
}

/// <summary>
/// Accounting period status
/// </summary>
public enum PeriodStatus
{
    Open = 1,
    Closed = 2,
    Locked = 3
}

/// <summary>
/// Subscription billing status for SaaS
/// </summary>
public enum SubscriptionBillingStatus
{
    Active = 1,
    PastDue = 2,
    Cancelled = 3,
    Suspended = 4,
    Trial = 5,
    Expired = 6
}

/// <summary>
/// Revenue recognition method for SaaS
/// </summary>
public enum RevenueRecognitionMethod
{
    Cash = 1,
    Accrual = 2,
    Deferred = 3,
    Subscription = 4,
    Usage = 5
}

/// <summary>
/// Bank reconciliation status
/// </summary>
public enum ReconciliationStatus
{
    Unreconciled = 1,
    Reconciled = 2,
    Disputed = 3,
    Adjustment = 4
}

/// <summary>
/// Financial year types
/// </summary>
public enum FiscalYearType
{
    Calendar = 1,
    April = 2,
    July = 3,
    October = 4,
    Custom = 5
}

/// <summary>
/// Cashbook entry types for cash management
/// </summary>
public enum CashbookEntryType
{
    Debit = 1,
    Credit = 2
}

/// <summary>
/// Cashbook entry categories for transaction classification
/// </summary>
public enum CashbookEntryCategory
{
    Sale = 1,
    Purchase = 2,
    SalesTax = 3,
    PurchaseTax = 4,
    CashReceipt = 5,
    CashPayment = 6,
    Adjustment = 7,
    Transfer = 8,
    Expense = 9,
    Income = 10,
    Other = 11
}

/// <summary>
/// Customer types for TOSS ERP - township SMME context
/// </summary>
public enum CustomerType
{
    Individual = 1,
    Business = 2,
    Organization = 3,
    Government = 4,
    Cooperative = 5,
    CommunityGroup = 6,
    Household = 7
}

/// <summary>
/// Customer status in the system
/// </summary>
public enum CustomerStatus
{
    Active = 1,
    Inactive = 2,
    Suspended = 3,
    Blocked = 4,
    Prospective = 5,
    Lead = 6
}

/// <summary>
/// Payment terms for customer credit management
/// </summary>
public enum PaymentTerms
{
    Net15 = 1,
    Net30 = 2,
    Net45 = 3,
    Net60 = 4,
    Net90 = 5,
    CashOnDelivery = 6,
    Prepaid = 7,
    DueOnReceipt = 8,
    Custom = 9
}

/// <summary>
/// Subscription status for recurring billing
/// </summary>
public enum SubscriptionStatus
{
    Active = 1,
    Inactive = 2,
    Cancelled = 3,
    Expired = 4,
    Trial = 5,
    PastDue = 6,
    Suspended = 7,
    PendingActivation = 8
}

/// <summary>
/// Journal entry status for posting workflow
/// </summary>
public enum JournalEntryStatus
{
    Draft = 1,
    Submitted = 2,
    Posted = 3,
    Cancelled = 4,
    Reversed = 5
}

/// <summary>
/// Bill status enumeration
/// </summary>
public enum BillStatus
{
    Draft = 0,
    Pending = 1,
    Approved = 2,
    PartiallyPaid = 3,
    Paid = 4,
    Overdue = 5,
    Cancelled = 6
}

/// <summary>
/// Bill type enumeration
/// </summary>
public enum BillType
{
    Standard = 0,
    Recurring = 1,
    Adjustment = 2,
    Credit = 3,
    Prepayment = 4
}

/// <summary>
/// Contact type enumeration for township communication preferences
/// </summary>
public enum ContactType
{
    Phone = 0,
    Email = 1,
    SMS = 2,
    WhatsApp = 3,
    InPerson = 4,
    CommunityBoard = 5,
    SocialMedia = 6,
    Other = 7
}

/// <summary>
/// Billing cycle enumeration for subscription billing
/// </summary>
public enum BillingCycle
{
    Monthly = 0,
    Quarterly = 1,
    SemiAnnually = 2,
    Annually = 3,
    Biennial = 4,
    Daily = 5,
    Weekly = 6
}
