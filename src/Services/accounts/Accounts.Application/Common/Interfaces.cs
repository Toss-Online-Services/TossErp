namespace TossErp.Accounts.Application.Common.Interfaces;

/// <summary>
/// Repository interface for Customer operations
/// </summary>
public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Customer?> GetByCustomerNumberAsync(string customerNumber, CancellationToken cancellationToken = default);
    Task<(List<Customer> Customers, int TotalCount)> GetFilteredAsync(
        int pageNumber, int pageSize, string? searchTerm = null, CustomerStatus? status = null,
        string? sortBy = null, bool sortDescending = false, CancellationToken cancellationToken = default);
    Task<List<Customer>> GetByIdsAsync(List<Guid> ids, CancellationToken cancellationToken = default);
    Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Task<Customer> UpdateAsync(Customer customer, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(List<Customer> customers, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for Invoice operations
/// </summary>
public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default);
    Task<(List<Invoice> Invoices, int TotalCount)> GetFilteredAsync(
        int pageNumber, int pageSize, Guid? customerId = null, InvoiceStatus? status = null,
        DateOnly? issueDateFrom = null, DateOnly? issueDateTo = null, DateOnly? dueDateFrom = null, DateOnly? dueDateTo = null,
        decimal? amountFrom = null, decimal? amountTo = null, string? sortBy = null, bool sortDescending = false,
        CancellationToken cancellationToken = default);
    Task<List<Invoice>> GetOverdueInvoicesAsync(DateOnly currentDate, CancellationToken cancellationToken = default);
    Task<List<Invoice>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<Invoice> AddAsync(Invoice invoice, CancellationToken cancellationToken = default);
    Task<Invoice> UpdateAsync(Invoice invoice, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(List<Invoice> invoices, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for Payment operations
/// </summary>
public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Payment?> GetByPaymentNumberAsync(string paymentNumber, CancellationToken cancellationToken = default);
    Task<(List<Payment> Payments, int TotalCount)> GetFilteredAsync(
        int pageNumber, int pageSize, Guid? customerId = null, Guid? invoiceId = null, PaymentStatus? status = null,
        PaymentMethod? method = null, DateOnly? paymentDateFrom = null, DateOnly? paymentDateTo = null,
        decimal? amountFrom = null, decimal? amountTo = null, string? sortBy = null, bool sortDescending = false,
        CancellationToken cancellationToken = default);
    Task<List<Payment>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<List<Payment>> GetByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    Task<Payment> AddAsync(Payment payment, CancellationToken cancellationToken = default);
    Task<Payment> UpdateAsync(Payment payment, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(List<Payment> payments, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for Subscription operations
/// </summary>
public interface ISubscriptionRepository
{
    Task<Subscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Subscription?> GetBySubscriptionNumberAsync(string subscriptionNumber, CancellationToken cancellationToken = default);
    Task<(List<Subscription> Subscriptions, int TotalCount)> GetFilteredAsync(
        int pageNumber, int pageSize, Guid? customerId = null, SubscriptionStatus? status = null,
        string? planName = null, DateOnly? startDateFrom = null, DateOnly? startDateTo = null,
        DateOnly? endDateFrom = null, DateOnly? endDateTo = null, string? sortBy = null, bool sortDescending = false,
        CancellationToken cancellationToken = default);
    Task<List<Subscription>> GetExpiringSubscriptionsAsync(DateOnly expirationDate, CancellationToken cancellationToken = default);
    Task<List<Subscription>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<Subscription> AddAsync(Subscription subscription, CancellationToken cancellationToken = default);
    Task<Subscription> UpdateAsync(Subscription subscription, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(List<Subscription> subscriptions, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for ChartOfAccounts operations
/// </summary>
public interface IChartOfAccountsRepository
{
    Task<ChartOfAccounts?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ChartOfAccounts?> GetByAccountCodeAsync(string accountCode, CancellationToken cancellationToken = default);
    Task<List<ChartOfAccounts>> GetByAccountTypeAsync(AccountType accountType, CancellationToken cancellationToken = default);
    Task<List<ChartOfAccounts>> GetActiveAccountsAsync(CancellationToken cancellationToken = default);
    Task<(List<ChartOfAccounts> Accounts, int TotalCount)> GetFilteredAsync(
        int pageNumber, int pageSize, AccountType? accountType = null, string? searchTerm = null,
        bool? isActive = null, string? sortBy = null, bool sortDescending = false,
        CancellationToken cancellationToken = default);
    Task<ChartOfAccounts> AddAsync(ChartOfAccounts account, CancellationToken cancellationToken = default);
    Task<ChartOfAccounts> UpdateAsync(ChartOfAccounts account, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(List<ChartOfAccounts> accounts, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for JournalEntry operations
/// </summary>
public interface IJournalEntryRepository
{
    Task<JournalEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<JournalEntry?> GetByEntryNumberAsync(string entryNumber, CancellationToken cancellationToken = default);
    Task<(List<JournalEntry> Entries, int TotalCount)> GetFilteredAsync(
        int pageNumber, int pageSize, DateOnly? entryDateFrom = null, DateOnly? entryDateTo = null,
        Guid? accountId = null, string? reference = null, string? sortBy = null, bool sortDescending = false,
        CancellationToken cancellationToken = default);
    Task<List<JournalEntry>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<List<JournalEntry>> GetByDateRangeAsync(DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken = default);
    Task<JournalEntry> AddAsync(JournalEntry entry, CancellationToken cancellationToken = default);
    Task<JournalEntry> UpdateAsync(JournalEntry entry, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(List<JournalEntry> entries, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
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
    Task<FinancialSummaryDto> GetFinancialSummaryAsync(DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken = default);
    Task<TrialBalanceDto> GetTrialBalanceAsync(DateOnly asOfDate, CancellationToken cancellationToken = default);
    Task<ProfitAndLossDto> GetProfitAndLossAsync(DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken = default);
    Task<BalanceSheetDto> GetBalanceSheetAsync(DateOnly asOfDate, CancellationToken cancellationToken = default);
    Task<CashFlowDto> GetCashFlowAsync(DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken = default);
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
    InvoiceCancelled
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
