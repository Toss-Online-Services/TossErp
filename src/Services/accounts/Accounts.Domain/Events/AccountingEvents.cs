using MediatR;
using TossErp.Accounts.Domain.Entities;
using TossErp.Accounts.Domain.Enums;
using TossErp.Accounts.Domain.ValueObjects;
using TossErp.Shared.SeedWork;

namespace TossErp.Accounts.Domain.Events;

// Chart of Accounts Events
public record AccountCreatedEvent(
    Guid AccountId,
    string TenantId,
    string AccountNumber,
    string AccountName,
    AccountType AccountType) : IDomainEvent, INotification
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record AccountUpdatedEvent(
    Guid AccountId,
    string TenantId,
    string AccountNumber,
    string NewAccountName) : IDomainEvent, INotification
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record AccountActivatedEvent(
    Guid AccountId,
    string TenantId,
    string AccountNumber) : IDomainEvent, INotification
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record AccountDeactivatedEvent(
    Guid AccountId,
    string TenantId,
    string AccountNumber) : IDomainEvent, INotification
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record AccountBalanceUpdatedEvent(
    Guid AccountId,
    string TenantId,
    string AccountNumber,
    Money NewBalance) : IDomainEvent, INotification
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// Financial Transaction Events
public record TransactionCreatedEvent(
    Guid TransactionId,
    string TenantId,
    string TransactionNumber,
    TransactionType TransactionType) : IDomainEvent, INotification
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record TransactionSubmittedEvent(
    Guid TransactionId,
    string TenantId,
    string TransactionNumber,
    string SubmittedBy) : IDomainEvent, INotification
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record TransactionApprovedEvent(
    Guid TransactionId,
    string TenantId,
    string TransactionNumber,
    string ApprovedBy) : IDomainEvent, INotification
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record TransactionRejectedEvent(
    Guid TransactionId,
    string TenantId,
    string TransactionNumber,
    string RejectedBy,
    string Reason) : IDomainEvent, INotification
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record TransactionPostedEvent(
    Guid TransactionId,
    string TenantId,
    string TransactionNumber,
    List<JournalEntryLine> JournalLines) : IDomainEvent, INotification
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record TransactionCancelledEvent(
    Guid TransactionId,
    string TenantId,
    string TransactionNumber,
    string CancelledBy,
    string Reason) : IDomainEvent, INotification
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// Invoice Events
public record InvoiceCreatedEvent(
    Guid InvoiceId,
    string TenantId,
    string InvoiceNumber,
    Guid CustomerId,
    InvoiceType InvoiceType) : IDomainEvent, INotification
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record InvoiceSubmittedEvent(
    Guid InvoiceId,
    string TenantId,
    string InvoiceNumber,
    Money TotalAmount) : IDomainEvent, INotification
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record InvoiceApprovedEvent(
    Guid InvoiceId,
    string TenantId,
    string InvoiceNumber,
    string ApprovedBy) : IDomainEvent, INotification
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record InvoiceSentEvent(
    Guid InvoiceId,
    string TenantId,
    string InvoiceNumber,
    Guid CustomerId,
    Money TotalAmount,
    DateTime DueDate) : IDomainEvent, INotification
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record InvoicePaymentReceivedEvent(
    Guid InvoiceId,
    string TenantId,
    string InvoiceNumber,
    Money PaymentAmount,
    DateTime PaymentDate,
    string PaymentReference) : IDomainEvent, INotification
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record InvoicePaidEvent(
    Guid InvoiceId,
    string TenantId,
    string InvoiceNumber,
    Money TotalAmount,
    DateTime PaymentDate) : IDomainEvent, INotification
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record InvoiceOverdueEvent(
    Guid InvoiceId,
    string TenantId,
    string InvoiceNumber,
    Money OutstandingAmount,
    DateTime DueDate) : IDomainEvent, INotification
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record InvoiceCancelledEvent(
    Guid InvoiceId,
    string TenantId,
    string InvoiceNumber,
    string Reason) : IDomainEvent, INotification
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// Budget Events
public record BudgetCreatedEvent(
    Guid BudgetId,
    string TenantId,
    string BudgetName,
    BudgetType BudgetType,
    Money TotalBudget) : IDomainEvent, INotification
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record BudgetSubmittedEvent(
    Guid BudgetId,
    string TenantId,
    string BudgetName) : IDomainEvent, INotification
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record BudgetApprovedEvent(
    Guid BudgetId,
    string TenantId,
    string BudgetName,
    string ApprovedBy) : IDomainEvent, INotification
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record BudgetActivatedEvent(
    Guid BudgetId,
    string TenantId,
    string BudgetName) : IDomainEvent, INotification
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record BudgetExpenseRecordedEvent(
    Guid BudgetId,
    string TenantId,
    string BudgetName,
    Money ExpenseAmount,
    string Description) : IDomainEvent, INotification
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record BudgetFundsCommittedEvent(
    Guid BudgetId,
    string TenantId,
    string BudgetName,
    Money CommittedAmount,
    string Description) : IDomainEvent, INotification
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record BudgetThresholdExceededEvent(
    Guid BudgetId,
    string TenantId,
    string BudgetName,
    decimal CurrentUtilization,
    decimal ThresholdPercentage) : IDomainEvent, INotification
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record BudgetClosedEvent(
    Guid BudgetId,
    string TenantId,
    string BudgetName,
    Money ActualSpent,
    Money TotalBudget) : IDomainEvent, INotification
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// Payment Events
public record PaymentCreatedEvent(
    Guid PaymentId,
    string TenantId,
    string PaymentNumber,
    PaymentType PaymentType,
    Money Amount) : INotification;

public record PaymentProcessedEvent(
    Guid PaymentId,
    string TenantId,
    string PaymentNumber,
    PaymentStatus Status,
    DateTime ProcessedAt) : INotification;

public record PaymentFailedEvent(
    Guid PaymentId,
    string TenantId,
    string PaymentNumber,
    string FailureReason) : INotification;

public record PaymentReconciledEvent(
    Guid PaymentId,
    string TenantId,
    string PaymentNumber,
    Guid BankTransactionId) : INotification;

// Bank Reconciliation Events
public record BankTransactionImportedEvent(
    Guid TransactionId,
    string TenantId,
    Guid BankAccountId,
    DateTime TransactionDate,
    Money Amount) : INotification;

public record BankTransactionReconciledEvent(
    Guid TransactionId,
    string TenantId,
    Guid BankAccountId,
    Guid MatchedJournalEntryId) : INotification;

public record BankReconciliationCompletedEvent(
    Guid ReconciliationId,
    string TenantId,
    Guid BankAccountId,
    DateTime ReconciliationDate,
    Money ReconciledAmount) : INotification;

// Tax Events
public record TaxCalculatedEvent(
    Guid TransactionId,
    string TenantId,
    TaxType TaxType,
    Money TaxableAmount,
    Money TaxAmount,
    decimal TaxRate) : INotification;

public record TaxReturnGeneratedEvent(
    Guid TaxReturnId,
    string TenantId,
    string TaxPeriod,
    DateTime PeriodStartDate,
    DateTime PeriodEndDate,
    Money TotalTaxLiability) : INotification;

public record TaxPaymentDueEvent(
    Guid TaxReturnId,
    string TenantId,
    string TaxPeriod,
    Money TaxDue,
    DateTime DueDate) : INotification;

// Subscription Billing Events (SaaS specific)
public record SubscriptionBillingCycleStartedEvent(
    Guid SubscriptionId,
    string TenantId,
    BillingPeriod BillingPeriod,
    DateTime CycleStartDate,
    DateTime CycleEndDate) : INotification;

public record SubscriptionInvoiceGeneratedEvent(
    Guid InvoiceId,
    Guid SubscriptionId,
    string TenantId,
    BillingPeriod BillingPeriod,
    Money Amount) : INotification;

public record SubscriptionPaymentProcessedEvent(
    Guid PaymentId,
    Guid SubscriptionId,
    string TenantId,
    Money Amount,
    PaymentMethod PaymentMethod) : INotification;

public record SubscriptionPaymentFailedEvent(
    Guid SubscriptionId,
    string TenantId,
    Money Amount,
    string FailureReason,
    int RetryAttempt) : INotification;

public record SubscriptionSuspendedEvent(
    Guid SubscriptionId,
    string TenantId,
    string Reason,
    DateTime SuspensionDate) : INotification;

public record SubscriptionReactivatedEvent(
    Guid SubscriptionId,
    string TenantId,
    DateTime ReactivationDate) : INotification;

// Revenue Recognition Events (SaaS specific)
public record RevenueRecognizedEvent(
    Guid TransactionId,
    string TenantId,
    Guid SubscriptionId,
    Money RevenueAmount,
    DateTime RecognitionDate,
    RevenueRecognitionMethod Method) : INotification;

public record DeferredRevenueCreatedEvent(
    Guid DeferredRevenueId,
    string TenantId,
    Guid SubscriptionId,
    Money DeferredAmount,
    DateTime StartDate,
    DateTime EndDate) : INotification;

public record DeferredRevenueRecognizedEvent(
    Guid DeferredRevenueId,
    string TenantId,
    Money RecognizedAmount,
    DateTime RecognitionDate) : INotification;

// Financial Period Events
public record FiscalYearOpenedEvent(
    Guid FiscalYearId,
    string TenantId,
    int FiscalYear,
    DateTime StartDate,
    DateTime EndDate) : INotification;

public record FiscalPeriodClosedEvent(
    Guid FiscalPeriodId,
    string TenantId,
    string PeriodName,
    DateTime StartDate,
    DateTime EndDate) : INotification;

public record FiscalYearClosedEvent(
    Guid FiscalYearId,
    string TenantId,
    int FiscalYear,
    DateTime ClosedDate,
    Money NetIncome) : INotification;

// Financial Report Events
public record FinancialReportGeneratedEvent(
    Guid ReportId,
    string TenantId,
    ReportType ReportType,
    DateTime ReportDate,
    DateTime PeriodStartDate,
    DateTime PeriodEndDate) : INotification;

public record TrialBalanceGeneratedEvent(
    Guid ReportId,
    string TenantId,
    DateTime AsOfDate,
    Money TotalDebits,
    Money TotalCredits) : INotification;

public record IncomeStatementGeneratedEvent(
    Guid ReportId,
    string TenantId,
    DateTime PeriodStartDate,
    DateTime PeriodEndDate,
    Money TotalRevenue,
    Money TotalExpenses,
    Money NetIncome) : INotification;

public record BalanceSheetGeneratedEvent(
    Guid ReportId,
    string TenantId,
    DateTime AsOfDate,
    Money TotalAssets,
    Money TotalLiabilities,
    Money TotalEquity) : INotification;

public record CashFlowStatementGeneratedEvent(
    Guid ReportId,
    string TenantId,
    DateTime PeriodStartDate,
    DateTime PeriodEndDate,
    Money OperatingCashFlow,
    Money InvestingCashFlow,
    Money FinancingCashFlow) : INotification;

// Multi-Currency Events
public record CurrencyExchangeRateUpdatedEvent(
    CurrencyCode FromCurrency,
    CurrencyCode ToCurrency,
    ExchangeRate NewRate,
    DateTime EffectiveDate) : INotification;

public record CurrencyConversionPerformedEvent(
    Guid TransactionId,
    string TenantId,
    Money OriginalAmount,
    Money ConvertedAmount,
    ExchangeRate ExchangeRate) : INotification;

// Cost Center Events
public record CostCenterCreatedEvent(
    Guid CostCenterId,
    string TenantId,
    string CostCenterCode,
    string CostCenterName,
    CostCenterType Type) : INotification;

public record CostCenterBudgetAssignedEvent(
    Guid CostCenterId,
    string TenantId,
    Guid BudgetId,
    Money AllocatedAmount) : INotification;

public record CostCenterExpenseAllocatedEvent(
    Guid CostCenterId,
    string TenantId,
    Guid TransactionId,
    Money AllocatedAmount) : INotification;
