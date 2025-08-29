using System.Linq.Expressions;
using TossErp.Accounts.Domain.Entities;
using TossErp.Accounts.Domain.Enums;
using AggregateInvoice = TossErp.Accounts.Domain.Aggregates.Invoice;
using TossErp.Accounts.Domain.Aggregates;

namespace TossErp.Accounts.Application.Common.Interfaces;

/// <summary>
/// Repository interface for Customer operations
/// </summary>
public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Customer?> GetByCustomerNumberAsync(string customerNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> GetByEmailDomainAsync(string emailDomain, CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> GetActiveCustomersAsync(CancellationToken cancellationToken = default);
    Task<bool> CustomerNumberExistsAsync(string customerNumber, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Customer Update(Customer customer);
    Task<Customer> UpdateAsync(Customer customer, CancellationToken cancellationToken = default);
    void Delete(Customer customer);
    Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> FindAsync(Expression<Func<Customer, bool>> predicate, CancellationToken cancellationToken = default);
    Task<Customer?> FirstOrDefaultAsync(Expression<Func<Customer, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<Customer, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<Customer, bool>>? predicate = null, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Customer> Customers, int TotalCount)> GetPagedAsync(CustomerFilter filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<DTOs.CustomerSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for Vendor operations
/// </summary>
public interface IVendorRepository
{
    Task<Vendor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Vendor?> GetByVendorNumberAsync(string vendorNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Vendor>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Vendor>> GetActiveVendorsAsync(CancellationToken cancellationToken = default);
    Task<bool> VendorNumberExistsAsync(string vendorNumber, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<Vendor> AddAsync(Vendor vendor, CancellationToken cancellationToken = default);
    Vendor Update(Vendor vendor);
    void Delete(Vendor vendor);
    Task<IEnumerable<Vendor>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Vendor>> FindAsync(Expression<Func<Vendor, bool>> predicate, CancellationToken cancellationToken = default);
    Task<Vendor?> FirstOrDefaultAsync(Expression<Func<Vendor, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<Vendor, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<Vendor, bool>>? predicate = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for Invoice operations
/// </summary>
public interface IInvoiceRepository
{
    Task<AggregateInvoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AggregateInvoice?> GetByInvoiceNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<AggregateInvoice>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AggregateInvoice>> GetOverdueInvoicesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<AggregateInvoice>> GetOverdueInvoicesAsync(DateTime? asOfDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<AggregateInvoice>> GetOutstandingInvoicesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<AggregateInvoice>> GetOutstandingInvoicesAsync(DateTime asOfDate, CancellationToken cancellationToken = default);
    Task<(IEnumerable<AggregateInvoice> Invoices, int TotalCount)> GetPagedAsync(
        DTOs.InvoiceFilter filter,
        int pageNumber,
        int pageSize,
        string? sortBy = null,
        bool sortDescending = false,
        CancellationToken cancellationToken = default);
    Task<bool> InvoiceNumberExistsAsync(string invoiceNumber, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalOutstandingAsync(CancellationToken cancellationToken = default);
    Task<AggregateInvoice> AddAsync(AggregateInvoice invoice, CancellationToken cancellationToken = default);
    Task<AggregateInvoice> UpdateAsync(AggregateInvoice invoice, CancellationToken cancellationToken = default);
    AggregateInvoice Update(AggregateInvoice invoice);
    void Delete(AggregateInvoice invoice);
    Task<IEnumerable<AggregateInvoice>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<AggregateInvoice>> FindAsync(Expression<Func<AggregateInvoice, bool>> predicate, CancellationToken cancellationToken = default);
    Task<AggregateInvoice?> FirstOrDefaultAsync(Expression<Func<AggregateInvoice, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<AggregateInvoice, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<AggregateInvoice, bool>>? predicate = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for Bill operations
/// </summary>
public interface IBillRepository
{
    Task<Bill?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Bill?> GetByBillNumberAsync(string billNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bill>> GetByVendorIdAsync(Guid vendorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bill>> GetOverdueBillsAsync(CancellationToken cancellationToken = default);
    Task<bool> BillNumberExistsAsync(string billNumber, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalOutstandingAsync(CancellationToken cancellationToken = default);
    Task<Bill> AddAsync(Bill bill, CancellationToken cancellationToken = default);
    Bill Update(Bill bill);
    void Delete(Bill bill);
    Task<IEnumerable<Bill>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Bill>> FindAsync(Expression<Func<Bill, bool>> predicate, CancellationToken cancellationToken = default);
    Task<Bill?> FirstOrDefaultAsync(Expression<Func<Bill, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<Bill, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<Bill, bool>>? predicate = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for Payment operations
/// </summary>
public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Payment?> GetByPaymentNumberAsync(string paymentNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetByPaymentMethodAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken = default);
    Task<Payment?> GetLastPaymentByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetUnallocatedPaymentsAsync(DateTime? asOfDate = null, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Payment> Payments, int TotalCount)> GetPagedAsync(
        DTOs.PaymentFilter filter,
        int pageNumber,
        int pageSize,
        string? sortBy = null,
        bool sortDescending = false,
        CancellationToken cancellationToken = default);
    Task<bool> PaymentNumberExistsAsync(string paymentNumber, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<Payment> AddAsync(Payment payment, CancellationToken cancellationToken = default);
    Task<Payment> UpdateAsync(Payment payment, CancellationToken cancellationToken = default);
    Payment Update(Payment payment);
    void Delete(Payment payment);
    Task<IEnumerable<Payment>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> FindAsync(Expression<Func<Payment, bool>> predicate, CancellationToken cancellationToken = default);
    Task<Payment?> FirstOrDefaultAsync(Expression<Func<Payment, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<Payment, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<Payment, bool>>? predicate = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for ChartOfAccounts operations
/// </summary>
public interface IChartOfAccountsRepository
{
    Task<ChartOfAccounts?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ChartOfAccounts?> GetByCodeAsync(string accountCode, CancellationToken cancellationToken = default);
    Task<ChartOfAccounts?> GetByAccountCodeAsync(string accountCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<ChartOfAccounts>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ChartOfAccounts>> GetByAccountTypeAsync(AccountType accountType, CancellationToken cancellationToken = default);
    Task<IEnumerable<ChartOfAccounts>> GetActiveAccountsAsync(CancellationToken cancellationToken = default);
    Task<bool> AccountCodeExistsAsync(string accountCode, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<(IEnumerable<ChartOfAccounts> Accounts, int TotalCount)> GetPagedAsync(
        AccountType? accountType = null,
        Guid? parentAccountId = null,
        bool? isActive = null,
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);
    Task<ChartOfAccounts> AddAsync(ChartOfAccounts account, CancellationToken cancellationToken = default);
    ChartOfAccounts Update(ChartOfAccounts account);
    void Delete(ChartOfAccounts account);
    Task<IEnumerable<ChartOfAccounts>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ChartOfAccounts>> FindAsync(Expression<Func<ChartOfAccounts, bool>> predicate, CancellationToken cancellationToken = default);
    Task<ChartOfAccounts?> FirstOrDefaultAsync(Expression<Func<ChartOfAccounts, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<ChartOfAccounts, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<ChartOfAccounts, bool>>? predicate = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for Company operations
/// </summary>
public interface ICompanyRepository
{
    Task<Company?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Company?> GetByNameAsync(string tenantId, string name, CancellationToken cancellationToken = default);
    Task<Company?> GetByAbbreviationAsync(string tenantId, string abbreviation, CancellationToken cancellationToken = default);
    Task<IEnumerable<Company>> GetChildCompaniesAsync(Guid parentCompanyId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Company>> GetGroupCompaniesAsync(string tenantId, bool? isActive = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Company>> GetAllAsync(string tenantId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Company> Companies, int TotalCount)> GetPagedAsync(
        string tenantId,
        string? searchTerm = null,
        bool? isActive = null,
        bool? isGroup = null,
        string? currency = null,
        string? country = null,
        Guid? parentCompanyId = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);
    Task<Company> AddAsync(Company company, CancellationToken cancellationToken = default);
    Task<Company> UpdateAsync(Company company, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Company>> FindAsync(Expression<Func<Company, bool>> predicate, CancellationToken cancellationToken = default);
    Task<Company?> FirstOrDefaultAsync(Expression<Func<Company, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<Company, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<Company, bool>>? predicate = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for Cashbook operations
/// </summary>
public interface ICashbookRepository
{
    Task<Cashbook?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Cashbook?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Cashbook?> GetDefaultCashbookAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Cashbook>> GetActiveCashbooksAsync(CancellationToken cancellationToken = default);
    Task<decimal> GetCashBalanceAsync(Guid cashbookId, CancellationToken cancellationToken = default);
    Task<Cashbook> AddAsync(Cashbook cashbook, CancellationToken cancellationToken = default);
    Cashbook Update(Cashbook cashbook);
    void Delete(Cashbook cashbook);
    Task<IEnumerable<Cashbook>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Cashbook>> FindAsync(Expression<Func<Cashbook, bool>> predicate, CancellationToken cancellationToken = default);
    Task<Cashbook?> FirstOrDefaultAsync(Expression<Func<Cashbook, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<Cashbook, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<Cashbook, bool>>? predicate = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for JournalEntry operations
/// </summary>
public interface IJournalEntryRepository
{
    Task<JournalEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<JournalEntry?> GetByNumberAsync(string entryNumber, CancellationToken cancellationToken = default);
    Task<JournalEntry?> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default);
    Task<IEnumerable<JournalEntry>> GetByDateRangeAsync(DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<JournalEntry>> GetByStatusAsync(JournalEntryStatus status, CancellationToken cancellationToken = default);
    Task<bool> EntryNumberExistsAsync(string entryNumber, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<(IEnumerable<JournalEntry> Entries, int TotalCount)> GetPagedAsync(
        DTOs.JournalEntryFilter filter,
        int pageNumber,
        int pageSize,
        string? sortBy = null,
        bool sortDescending = false,
        CancellationToken cancellationToken = default);
    Task<JournalEntry> AddAsync(JournalEntry journalEntry, CancellationToken cancellationToken = default);
    JournalEntry Update(JournalEntry journalEntry);
    Task<JournalEntry> UpdateAsync(JournalEntry journalEntry, CancellationToken cancellationToken = default);
    void Delete(JournalEntry journalEntry);
    Task<IEnumerable<JournalEntry>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<JournalEntry>> FindAsync(Expression<Func<JournalEntry, bool>> predicate, CancellationToken cancellationToken = default);
    Task<JournalEntry?> FirstOrDefaultAsync(Expression<Func<JournalEntry, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<JournalEntry, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<JournalEntry, bool>>? predicate = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for Subscription operations
/// </summary>
public interface ISubscriptionRepository
{
    Task<Subscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Subscription?> GetByNameAsync(string subscriptionName, CancellationToken cancellationToken = default);
    Task<IEnumerable<Subscription>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Subscription>> GetByStatusAsync(SubscriptionStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Subscription>> GetExpiringSubscriptionsAsync(DateOnly beforeDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<Subscription>> GetActiveSubscriptionsAsync(CancellationToken cancellationToken = default);
    Task<Subscription?> GetActiveByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Subscription>> GetSubscriptionsForMRRAnalysisAsync(
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);
    Task<(IEnumerable<Subscription> Subscriptions, int TotalCount)> GetPagedAsync(
        DTOs.SubscriptionFilter filter,
        int pageNumber,
        int pageSize,
        string? sortBy = null,
        bool sortDescending = false,
        CancellationToken cancellationToken = default);
    Task<Subscription> AddAsync(Subscription subscription, CancellationToken cancellationToken = default);
    Subscription Update(Subscription subscription);
    void Delete(Subscription subscription);
    Task<IEnumerable<Subscription>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Subscription>> FindAsync(Expression<Func<Subscription, bool>> predicate, CancellationToken cancellationToken = default);
    Task<Subscription?> FirstOrDefaultAsync(Expression<Func<Subscription, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<Subscription, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<Subscription, bool>>? predicate = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Unit of work pattern for maintaining consistency across repositories
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Service for publishing domain events
/// </summary>
public interface IDomainEventService
{
    Task PublishAsync<T>(T domainEvent, CancellationToken cancellationToken = default) where T : class;
    Task PublishAsync(IEnumerable<object> domainEvents, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service for getting current user information
/// </summary>
public interface ICurrentUserService
{
    string? UserId { get; }
    string? Email { get; }
    string? TenantId { get; }
    bool IsAuthenticated { get; }
}

/// <summary>
/// Service for multi-tenant support
/// </summary>
public interface ICurrentTenantService
{
    string TenantId { get; }
    string? UserId { get; }
}

/// <summary>
/// Service for date/time operations
/// </summary>
public interface IDateTimeService
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
    DateOnly Today { get; }
    DateOnly TodayUtc { get; }
}

/// <summary>
/// Service for payment processing operations
/// </summary>
public interface IPaymentProcessingService
{
    Task<PaymentResult> ProcessPaymentAsync(ProcessPaymentRequest request, CancellationToken cancellationToken = default);
    Task<RefundResult> ProcessRefundAsync(ProcessRefundRequest request, CancellationToken cancellationToken = default);
    Task<bool> ValidatePaymentMethodAsync(PaymentMethod method, string details, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service for email notifications
/// </summary>
public interface INotificationService
{
    Task SendInvoiceNotificationAsync(Guid invoiceId, string customerEmail, NotificationType type, CancellationToken cancellationToken = default);
    Task SendPaymentConfirmationAsync(Guid paymentId, string customerEmail, CancellationToken cancellationToken = default);
    Task SendPaymentNotificationAsync(Guid paymentId, string customerEmail, NotificationType type, CancellationToken cancellationToken = default);
    Task SendSubscriptionNotificationAsync(Guid subscriptionId, string customerEmail, SubscriptionNotificationType type, CancellationToken cancellationToken = default);
    Task SendBulkNotificationAsync(List<string> emails, string subject, string message, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service for file operations
/// </summary>
public interface IFileService
{
    Task<string> SaveInvoicePdfAsync(Guid invoiceId, byte[] pdfData, CancellationToken cancellationToken = default);
    Task<byte[]> GenerateInvoicePdfAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    Task<string> SavePaymentReceiptAsync(Guid paymentId, byte[] receiptData, CancellationToken cancellationToken = default);
    Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service for financial reporting and calculations
/// </summary>
public interface IFinancialReportingService
{
    Task<DTOs.FinancialSummaryDto> GetFinancialSummaryAsync(DateOnly fromDate, DateOnly toDate, string? currency = null, CancellationToken cancellationToken = default);
    Task<DTOs.TrialBalanceDto> GetTrialBalanceAsync(DateOnly asOfDate, string? currency = null, CancellationToken cancellationToken = default);
    Task<DTOs.ProfitAndLossDto> GetProfitAndLossAsync(DateOnly fromDate, DateOnly toDate, string? currency = null, CancellationToken cancellationToken = default);
    Task<DTOs.BalanceSheetDto> GetBalanceSheetAsync(DateOnly asOfDate, string? currency = null, CancellationToken cancellationToken = default);
    Task<DTOs.CashFlowDto> GetCashFlowAsync(DateOnly fromDate, DateOnly toDate, string? currency = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Notification types for invoices
/// </summary>
public enum NotificationType
{
    InvoiceCreated,
    InvoiceOverdue,
    PaymentReminder,
    InvoicePaid,
    InvoiceCancelled,
    PaymentReceived,
    PaymentRefunded
}

/// <summary>
/// Notification types for subscriptions
/// </summary>
public enum SubscriptionNotificationType
{
    SubscriptionCreated,
    SubscriptionActivated,
    SubscriptionExpiring,
    SubscriptionExpired,
    SubscriptionCancelled,
    SubscriptionRenewed
}

/// <summary>
/// Payment processing request
/// </summary>
public class ProcessPaymentRequest
{
    public Guid PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public PaymentMethod Method { get; set; }
    public string PaymentDetails { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}

/// <summary>
/// Payment processing result
/// </summary>
public class PaymentResult
{
    public bool IsSuccess { get; set; }
    public string? TransactionId { get; set; }
    public string? ProviderResponse { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}

/// <summary>
/// Refund processing request
/// </summary>
public class ProcessRefundRequest
{
    public Guid PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string? OriginalTransactionId { get; set; }
}

/// <summary>
/// Refund processing result
/// </summary>
public class RefundResult
{
    public bool IsSuccess { get; set; }
    public string? RefundId { get; set; }
    public string? ProviderResponse { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}

/// <summary>
/// Customer filter for queries
/// </summary>
public class CustomerFilter
{
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
    public string? Country { get; set; }
    public decimal? MinCreditLimit { get; set; }
    public decimal? MaxCreditLimit { get; set; }
}
