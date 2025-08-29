namespace TossErp.Accounts.Application.DTOs;

/// <summary>
/// Customer Data Transfer Object
/// </summary>
public class CustomerDto
{
    public Guid Id { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string CustomerNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public CustomerStatus Status { get; set; }
    public CustomerType Type { get; set; }
    public CustomerType CustomerType { get; set; } // Application layer compatibility
    public string? CompanyName { get; set; }
    public string? TaxId { get; set; }
    public string? TaxNumber { get; set; } // Application layer compatibility
    public string? Website { get; set; }
    public CustomerAddressDto? BillingAddress { get; set; }
    public CustomerAddressDto? ShippingAddress { get; set; }
    public CustomerContactDto? PrimaryContact { get; set; }
    public List<CustomerContactDto> AdditionalContacts { get; set; } = new();
    public List<CustomerAddressDto> Addresses { get; set; } = new(); // Application layer compatibility
    public List<CustomerContactDto> Contacts { get; set; } = new(); // Application layer compatibility
    public string? PreferredCurrency { get; set; }
    public string? PreferredLanguage { get; set; }
    public PaymentTerms PaymentTerms { get; set; }
    public decimal CreditLimit { get; set; }
    public decimal CurrentBalance { get; set; }
    public DateOnly? LastPaymentDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
    // Application layer compatibility properties for financial reports
    public int TotalInvoices { get; set; }
    public decimal TotalOutstanding { get; set; }
    public DateOnly? LastInvoiceDate { get; set; }
}

/// <summary>
/// Customer Summary Data Transfer Object for lists
/// </summary>
public class CustomerSummaryDto
{
    public Guid Id { get; set; }
    public string CustomerNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public CustomerStatus Status { get; set; }
    public CustomerType Type { get; set; }
    public string? CompanyName { get; set; }
    public decimal CurrentBalance { get; set; }
    public DateOnly? LastPaymentDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }
    // Application layer compatibility properties for reports
    public int TotalCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public int NewCustomersThisMonth { get; set; }
    public List<CountryStatisticDto> TopCountries { get; set; } = new();
    public int TotalInvoices { get; set; }
    public decimal TotalPaidAmount { get; set; }
    public decimal TotalOutstandingAmount { get; set; }
    public decimal OverdueAmount { get; set; }
    public int ActiveSubscriptions { get; set; }
}

/// <summary>
/// Country Statistics DTO for customer reports
/// </summary>
public class CountryStatisticDto
{
    public string Country { get; set; } = string.Empty;
    public int CustomerCount { get; set; }
}

/// <summary>
/// Customer Address Data Transfer Object
/// </summary>
public class CustomerAddressDto
{
    public string Street { get; set; } = string.Empty;
    public string? Street2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    // Application layer compatibility properties
    public string Type { get; set; } = string.Empty; // Billing, Shipping, etc.
    public bool IsPrimary { get; set; }
}

/// <summary>
/// Customer Contact Data Transfer Object
/// </summary>
public class CustomerContactDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Position { get; set; }
    public string? Title { get; set; } // Application layer compatibility
    public bool IsPrimary { get; set; }
}

/// <summary>
/// Invoice Data Transfer Object
/// </summary>
public class InvoiceDto
{
    public Guid Id { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public InvoiceStatus Status { get; set; }
    public DateOnly IssueDate { get; set; }
    public DateOnly DueDate { get; set; }
    public DateOnly? PaidDate { get; set; }
    public decimal SubtotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal BalanceAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public List<InvoiceLineItemDto> LineItems { get; set; } = new();
    public CustomerAddressDto? BillingAddress { get; set; }
    public CustomerAddressDto? ShippingAddress { get; set; }
    public string? Terms { get; set; }
    public string? Notes { get; set; }
    public string? InternalNotes { get; set; }
    public string? Reference { get; set; }
    public string? PurchaseOrderNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
}

/// <summary>
/// Invoice Summary Data Transfer Object for lists
/// </summary>
public class InvoiceSummaryDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public InvoiceStatus Status { get; set; }
    public DateOnly IssueDate { get; set; }
    public DateOnly DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal BalanceAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public bool IsOverdue { get; set; }
    public int DaysOverdue { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }
}

/// <summary>
/// Invoice Line Item Data Transfer Object
/// </summary>
public class InvoiceLineItemDto
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? TaxRate { get; set; }
    public decimal? TaxAmount { get; set; }
    public string? ProductCode { get; set; }
    public string? Unit { get; set; }
}

/// <summary>
/// Payment Data Transfer Object
/// </summary>
public class PaymentDto
{
    public Guid Id { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string PaymentNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid? InvoiceId { get; set; }
    public string? InvoiceNumber { get; set; }
    public PaymentStatus Status { get; set; }
    public PaymentType Type { get; set; }
    public PaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateOnly PaymentDate { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
    public string? TransactionId { get; set; }
    public string? ProcessorResponse { get; set; }
    public List<PaymentAllocationDto> Allocations { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
    // Application layer compatibility properties
    public string? ExternalTransactionId { get; set; }
    public decimal? RefundAmount { get; set; }
    public string? RefundReason { get; set; }
    public decimal AllocatedAmount { get; set; }
    public decimal UnallocatedAmount { get; set; }
}

/// <summary>
/// Payment Summary Data Transfer Object for lists
/// </summary>
public class PaymentSummaryDto
{
    public Guid Id { get; set; }
    public string PaymentNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; }
    public PaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateOnly PaymentDate { get; set; }
    public string? Reference { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }
}

/// <summary>
/// Payment Allocation Data Transfer Object
/// </summary>
public class PaymentAllocationDto
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal AllocatedAmount { get; set; }
    public DateTime AllocatedAt { get; set; }
}

/// <summary>
/// Subscription Data Transfer Object
/// </summary>
public class SubscriptionDto
{
    public Guid Id { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string SubscriptionNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public SubscriptionStatus Status { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public string? PlanDescription { get; set; }
    public decimal MonthlyAmount { get; set; }
    public decimal YearlyAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public BillingCycle BillingCycle { get; set; }
    public BillingFrequency BillingFrequency { get; set; }
    public decimal PlanPrice { get; set; }
    public int? BillingCycleDay { get; set; }
    public int? TrialDays { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateOnly? TrialEndDate { get; set; }
    public DateOnly NextBillingDate { get; set; }
    public bool AutoRenew { get; set; }
    public int? MaxUsers { get; set; }
    public Dictionary<string, object> Features { get; set; } = new();
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelledBy { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
    // Application layer compatibility properties
    public int DaysUntilRenewal { get; set; }
}

/// <summary>
/// Subscription Summary Data Transfer Object for lists
/// </summary>
public class SubscriptionSummaryDto
{
    public Guid Id { get; set; }
    public string SubscriptionNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public SubscriptionStatus Status { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public decimal MonthlyAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public BillingCycle BillingCycle { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateOnly NextBillingDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }
}

/// <summary>
/// Chart of Accounts Data Transfer Object
/// </summary>
public class ChartOfAccountsDto
{
    public Guid Id { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public string? Description { get; set; }
    public Guid? ParentAccountId { get; set; }
    public string? ParentAccountName { get; set; }
    public string? ParentAccountCode { get; set; }
    public bool IsActive { get; set; }
    public bool AllowTransactions { get; set; }
    public string? TaxCode { get; set; }
    public decimal CurrentBalance { get; set; }
    public string DefaultCurrency { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
    // Application layer compatibility properties
    public List<ChartOfAccountsDto> ChildAccounts { get; set; } = new();
}

/// <summary>
/// Journal Entry Data Transfer Object
/// </summary>
public class JournalEntryDto
{
    public Guid Id { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string EntryNumber { get; set; } = string.Empty;
    public DateOnly EntryDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public decimal TotalDebitAmount { get; set; }
    public decimal TotalCreditAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public List<JournalEntryLineDto> Lines { get; set; } = new();
    public bool IsPosted { get; set; }
    public DateTime? PostedAt { get; set; }
    public string? PostedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
    // Application layer compatibility properties
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public Guid? ReversalJournalId { get; set; }
    public string? ReversalReason { get; set; }
    public string? AccountName { get; set; }
    public string? AccountCode { get; set; }
}

/// <summary>
/// Journal Entry Line Data Transfer Object
/// </summary>
public class JournalEntryLineDto
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string? Reference { get; set; }
}

/// <summary>
/// Financial Summary Data Transfer Object
/// </summary>
public class FinancialSummaryDto
{
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetIncome { get; set; }
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal TotalEquity { get; set; }
    public decimal AccountsReceivable { get; set; }
    public decimal AccountsPayable { get; set; }
    public decimal CashAndEquivalents { get; set; }
    public decimal OutstandingInvoices { get; set; }
    public decimal OverdueInvoices { get; set; }
    public int InvoiceCount { get; set; }
    public int OverdueInvoiceCount { get; set; }
    public int PaymentCount { get; set; }
    public decimal AverageInvoiceAmount { get; set; }
    public decimal AveragePaymentAmount { get; set; }
}

/// <summary>
/// Trial Balance Data Transfer Object
/// </summary>
public class TrialBalanceDto
{
    public DateOnly AsOfDate { get; set; }
    public string Currency { get; set; } = string.Empty;
    public List<TrialBalanceLineDto> Lines { get; set; } = new();
    public decimal TotalDebits { get; set; }
    public decimal TotalCredits { get; set; }
    public bool IsBalanced { get; set; }
}

/// <summary>
/// Trial Balance Line Data Transfer Object
/// </summary>
public class TrialBalanceLineDto
{
    public Guid AccountId { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public decimal DebitBalance { get; set; }
    public decimal CreditBalance { get; set; }
}

/// <summary>
/// Profit and Loss Data Transfer Object
/// </summary>
public class ProfitAndLossDto
{
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public string Currency { get; set; } = string.Empty;
    public List<ProfitAndLossLineDto> RevenueLines { get; set; } = new();
    public List<ProfitAndLossLineDto> ExpenseLines { get; set; } = new();
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetIncome { get; set; }
}

/// <summary>
/// Profit and Loss Line Data Transfer Object
/// </summary>
public class ProfitAndLossLineDto
{
    public Guid AccountId { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

/// <summary>
/// Balance Sheet Data Transfer Object
/// </summary>
public class BalanceSheetDto
{
    public DateOnly AsOfDate { get; set; }
    public string Currency { get; set; } = string.Empty;
    public List<BalanceSheetLineDto> AssetLines { get; set; } = new();
    public List<BalanceSheetLineDto> LiabilityLines { get; set; } = new();
    public List<BalanceSheetLineDto> EquityLines { get; set; } = new();
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal TotalEquity { get; set; }
    public bool IsBalanced { get; set; }
}

/// <summary>
/// Balance Sheet Line Data Transfer Object
/// </summary>
public class BalanceSheetLineDto
{
    public Guid AccountId { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

/// <summary>
/// Cash Flow Data Transfer Object
/// </summary>
public class CashFlowDto
{
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal OpeningBalance { get; set; }
    public decimal ClosingBalance { get; set; }
    public decimal NetCashFlow { get; set; }
    public List<CashFlowLineDto> OperatingActivities { get; set; } = new();
    public List<CashFlowLineDto> InvestingActivities { get; set; } = new();
    public List<CashFlowLineDto> FinancingActivities { get; set; } = new();
    public decimal NetOperatingCashFlow { get; set; }
    public decimal NetInvestingCashFlow { get; set; }
    public decimal NetFinancingCashFlow { get; set; }
}

/// <summary>
/// Cash Flow Line Data Transfer Object
/// </summary>
public class CashFlowLineDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

/// <summary>
/// Paged result wrapper for lists
/// </summary>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

/// <summary>
/// Company Data Transfer Object
/// </summary>
public class CompanyDto
{
    public Guid Id { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public string? Domain { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? TaxId { get; set; }
    public Guid? ParentCompanyId { get; set; }
    public string? ParentCompanyName { get; set; }
    public bool IsGroup { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public string? Logo { get; set; }
    public DateTime RegistrationDate { get; set; }
    public int FiscalYearStartMonth { get; set; }
    public DateTime? DateOfIncorporation { get; set; }
    public DateTime? DateOfCommencement { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? Website { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }
    public List<CompanySummaryDto> ChildCompanies { get; set; } = new();
}

/// <summary>
/// Company Summary Data Transfer Object for lists and child references
/// </summary>
public class CompanySummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public string? Domain { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public bool IsGroup { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Create Company Request DTO
/// </summary>
public class CreateCompanyDto
{
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public string? Domain { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? TaxId { get; set; }
    public Guid? ParentCompanyId { get; set; }
    public bool IsGroup { get; set; } = false;
    public string? Description { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public DateTime? DateOfIncorporation { get; set; }
    public DateTime? DateOfCommencement { get; set; }
    public string? RegistrationNumber { get; set; }
    public int FiscalYearStartMonth { get; set; } = 1;
}

/// <summary>
/// Update Company Request DTO
/// </summary>
public class UpdateCompanyDto
{
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public string? Domain { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? TaxId { get; set; }
    public Guid? ParentCompanyId { get; set; }
    public bool IsGroup { get; set; } = false;
    public string? Description { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public DateTime? DateOfIncorporation { get; set; }
    public DateTime? DateOfCommencement { get; set; }
    public string? RegistrationNumber { get; set; }
    public int FiscalYearStartMonth { get; set; } = 1;
}
