namespace TossErp.Accounts.Application.Queries;

/// <summary>
/// Query to get financial summary
/// </summary>
public record GetFinancialSummaryQuery(
    DateOnly? FromDate = null,
    DateOnly? ToDate = null,
    string? Currency = null
) : IRequest<FinancialSummaryDto>;

/// <summary>
/// Handler for getting financial summary
/// </summary>
public class GetFinancialSummaryQueryHandler : IRequestHandler<GetFinancialSummaryQuery, FinancialSummaryDto>
{
    private readonly IFinancialReportingService _financialReportingService;
    private readonly ILogger<GetFinancialSummaryQueryHandler> _logger;

    public GetFinancialSummaryQueryHandler(
        IFinancialReportingService financialReportingService,
        ILogger<GetFinancialSummaryQueryHandler> logger)
    {
        _financialReportingService = financialReportingService;
        _logger = logger;
    }

    public async Task<FinancialSummaryDto> Handle(GetFinancialSummaryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting financial summary from {FromDate} to {ToDate}", 
            request.FromDate, request.ToDate);

        var fromDate = request.FromDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1));
        var toDate = request.ToDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

        var summary = await _financialReportingService.GetFinancialSummaryAsync(
            fromDate, toDate, request.Currency, cancellationToken);

        return summary;
    }
}

/// <summary>
/// Query to get trial balance report
/// </summary>
public record GetTrialBalanceQuery(
    DateOnly AsOfDate,
    bool IncludeZeroBalances = false
) : IRequest<TrialBalanceDto>;

/// <summary>
/// Handler for getting trial balance
/// </summary>
public class GetTrialBalanceQueryHandler : IRequestHandler<GetTrialBalanceQuery, TrialBalanceDto>
{
    private readonly IFinancialReportingService _financialReportingService;
    private readonly ILogger<GetTrialBalanceQueryHandler> _logger;

    public GetTrialBalanceQueryHandler(
        IFinancialReportingService financialReportingService,
        ILogger<GetTrialBalanceQueryHandler> logger)
    {
        _financialReportingService = financialReportingService;
        _logger = logger;
    }

    public async Task<TrialBalanceDto> Handle(GetTrialBalanceQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting trial balance as of {AsOfDate}", request.AsOfDate);

        var trialBalance = await _financialReportingService.GetTrialBalanceAsync(
            request.AsOfDate, null, cancellationToken);

        return trialBalance;
    }
}

/// <summary>
/// Query to get profit and loss statement
/// </summary>
public record GetProfitAndLossQuery(
    DateOnly FromDate,
    DateOnly ToDate,
    string? Currency = null
) : IRequest<ProfitAndLossDto>;

/// <summary>
/// Handler for getting profit and loss statement
/// </summary>
public class GetProfitAndLossQueryHandler : IRequestHandler<GetProfitAndLossQuery, ProfitAndLossDto>
{
    private readonly IFinancialReportingService _financialReportingService;
    private readonly ILogger<GetProfitAndLossQueryHandler> _logger;

    public GetProfitAndLossQueryHandler(
        IFinancialReportingService financialReportingService,
        ILogger<GetProfitAndLossQueryHandler> logger)
    {
        _financialReportingService = financialReportingService;
        _logger = logger;
    }

    public async Task<ProfitAndLossDto> Handle(GetProfitAndLossQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting profit and loss statement from {FromDate} to {ToDate}", 
            request.FromDate, request.ToDate);

        var profitAndLoss = await _financialReportingService.GetProfitAndLossAsync(
            request.FromDate, request.ToDate, request.Currency, cancellationToken);

        return profitAndLoss;
    }
}

/// <summary>
/// Query to get balance sheet
/// </summary>
public record GetBalanceSheetQuery(
    DateOnly AsOfDate,
    string? Currency = null
) : IRequest<BalanceSheetDto>;

/// <summary>
/// Handler for getting balance sheet
/// </summary>
public class GetBalanceSheetQueryHandler : IRequestHandler<GetBalanceSheetQuery, BalanceSheetDto>
{
    private readonly IFinancialReportingService _financialReportingService;
    private readonly ILogger<GetBalanceSheetQueryHandler> _logger;

    public GetBalanceSheetQueryHandler(
        IFinancialReportingService financialReportingService,
        ILogger<GetBalanceSheetQueryHandler> logger)
    {
        _financialReportingService = financialReportingService;
        _logger = logger;
    }

    public async Task<BalanceSheetDto> Handle(GetBalanceSheetQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting balance sheet as of {AsOfDate}", request.AsOfDate);

        var balanceSheet = await _financialReportingService.GetBalanceSheetAsync(
            request.AsOfDate, request.Currency, cancellationToken);

        return balanceSheet;
    }
}

/// <summary>
/// Query to get cash flow statement
/// </summary>
public record GetCashFlowQuery(
    DateOnly FromDate,
    DateOnly ToDate,
    string? Currency = null
) : IRequest<CashFlowDto>;

/// <summary>
/// Handler for getting cash flow statement
/// </summary>
public class GetCashFlowQueryHandler : IRequestHandler<GetCashFlowQuery, CashFlowDto>
{
    private readonly IFinancialReportingService _financialReportingService;
    private readonly ILogger<GetCashFlowQueryHandler> _logger;

    public GetCashFlowQueryHandler(
        IFinancialReportingService financialReportingService,
        ILogger<GetCashFlowQueryHandler> logger)
    {
        _financialReportingService = financialReportingService;
        _logger = logger;
    }

    public async Task<CashFlowDto> Handle(GetCashFlowQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting cash flow statement from {FromDate} to {ToDate}", 
            request.FromDate, request.ToDate);

        var cashFlow = await _financialReportingService.GetCashFlowAsync(
            request.FromDate, request.ToDate, request.Currency, cancellationToken);

        return cashFlow;
    }
}

/// <summary>
/// Query to get accounts receivable aging report
/// </summary>
public record GetAccountsReceivableAgingQuery(
    DateOnly AsOfDate,
    int PageNumber = 1,
    int PageSize = 50
) : IRequest<PaginatedResult<AccountsReceivableAgingDto>>;

/// <summary>
/// DTO for accounts receivable aging
/// </summary>
public class AccountsReceivableAgingDto
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalOutstanding { get; set; }
    public decimal Current { get; set; }
    public decimal Days1To30 { get; set; }
    public decimal Days31To60 { get; set; }
    public decimal Days61To90 { get; set; }
    public decimal Over90Days { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime LastPaymentDate { get; set; }
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
}

/// <summary>
/// Handler for getting accounts receivable aging
/// </summary>
public class GetAccountsReceivableAgingQueryHandler : IRequestHandler<GetAccountsReceivableAgingQuery, PaginatedResult<AccountsReceivableAgingDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<GetAccountsReceivableAgingQueryHandler> _logger;

    public GetAccountsReceivableAgingQueryHandler(
        IInvoiceRepository invoiceRepository,
        ICustomerRepository customerRepository,
        IPaymentRepository paymentRepository,
        ILogger<GetAccountsReceivableAgingQueryHandler> logger)
    {
        _invoiceRepository = invoiceRepository;
        _customerRepository = customerRepository;
        _paymentRepository = paymentRepository;
        _logger = logger;
    }

    public async Task<PaginatedResult<AccountsReceivableAgingDto>> Handle(GetAccountsReceivableAgingQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting accounts receivable aging as of {AsOfDate}", request.AsOfDate);

        // Get all outstanding invoices
        var outstandingInvoices = await _invoiceRepository.GetOutstandingInvoicesAsync(request.AsOfDate, cancellationToken);
        
        // Group by customer
        var customerGroups = outstandingInvoices.GroupBy(i => i.CustomerId).ToList();
        
        var agingData = new List<AccountsReceivableAgingDto>();

        foreach (var group in customerGroups)
        {
            var customerId = group.Key;
            var customerInvoices = group.ToList();
            
            // Get customer details
            var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
            if (customer == null) continue;

            // Get last payment date
            var lastPayment = await _paymentRepository.GetLastPaymentByCustomerAsync(customerId, cancellationToken);

            // Calculate aging buckets
            var agingDto = new AccountsReceivableAgingDto
            {
                CustomerId = customerId,
                CustomerName = customer.Name,
                Currency = customerInvoices.First().Currency,
                LastPaymentDate = lastPayment?.PaymentDate.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue,
                ContactEmail = customer.Email ?? string.Empty,
                ContactPhone = customer.Phone ?? string.Empty
            };

            foreach (var invoice in customerInvoices)
            {
                var daysOverdue = (request.AsOfDate.DayNumber - invoice.DueDate.DayNumber);
                var amount = invoice.BalanceAmount;

                agingDto.TotalOutstanding += amount;

                if (daysOverdue <= 0)
                {
                    agingDto.Current += amount;
                }
                else if (daysOverdue <= 30)
                {
                    agingDto.Days1To30 += amount;
                }
                else if (daysOverdue <= 60)
                {
                    agingDto.Days31To60 += amount;
                }
                else if (daysOverdue <= 90)
                {
                    agingDto.Days61To90 += amount;
                }
                else
                {
                    agingDto.Over90Days += amount;
                }
            }

            agingData.Add(agingDto);
        }

        // Sort by total outstanding descending
        agingData = agingData.OrderByDescending(a => a.TotalOutstanding).ToList();

        // Apply pagination
        var totalCount = agingData.Count;
        var pagedData = agingData
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new PaginatedResult<AccountsReceivableAgingDto>
        {
            Items = pagedData,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }
}

/// <summary>
/// Query to get monthly recurring revenue report
/// </summary>
public record GetMonthlyRecurringRevenueQuery(
    DateOnly FromDate,
    DateOnly ToDate,
    string? Currency = null
) : IRequest<List<MonthlyRecurringRevenueDto>>;

/// <summary>
/// DTO for monthly recurring revenue
/// </summary>
public class MonthlyRecurringRevenueDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public decimal MonthlyRecurringRevenue { get; set; }
    public decimal NewMRR { get; set; }
    public decimal ChurnedMRR { get; set; }
    public decimal ExpansionMRR { get; set; }
    public decimal ContractionMRR { get; set; }
    public int ActiveSubscriptions { get; set; }
    public int NewSubscriptions { get; set; }
    public int ChurnedSubscriptions { get; set; }
    public string Currency { get; set; } = string.Empty;
}

/// <summary>
/// Handler for getting monthly recurring revenue
/// </summary>
public class GetMonthlyRecurringRevenueQueryHandler : IRequestHandler<GetMonthlyRecurringRevenueQuery, List<MonthlyRecurringRevenueDto>>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ILogger<GetMonthlyRecurringRevenueQueryHandler> _logger;

    public GetMonthlyRecurringRevenueQueryHandler(
        ISubscriptionRepository subscriptionRepository,
        ILogger<GetMonthlyRecurringRevenueQueryHandler> logger)
    {
        _subscriptionRepository = subscriptionRepository;
        _logger = logger;
    }

    public async Task<List<MonthlyRecurringRevenueDto>> Handle(GetMonthlyRecurringRevenueQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting monthly recurring revenue from {FromDate} to {ToDate}", 
            request.FromDate, request.ToDate);

        var subscriptions = await _subscriptionRepository.GetSubscriptionsForMRRAnalysisAsync(
            request.FromDate, request.ToDate, request.Currency, cancellationToken);

        var mrrData = new List<MonthlyRecurringRevenueDto>();
        var currentDate = new DateOnly(request.FromDate.Year, request.FromDate.Month, 1);
        var endDate = new DateOnly(request.ToDate.Year, request.ToDate.Month, 1);

        while (currentDate <= endDate)
        {
            var monthEndDate = currentDate.AddMonths(1).AddDays(-1);
            
            // Active subscriptions at month end
            var activeSubscriptions = subscriptions.Where(s => 
                s.StartDate <= monthEndDate && 
                (s.EndDate == null || s.EndDate >= monthEndDate) &&
                s.Status == SubscriptionStatus.Active).ToList();

            // New subscriptions in this month
            var newSubscriptions = subscriptions.Where(s => 
                s.StartDate >= currentDate && 
                s.StartDate <= monthEndDate).ToList();

            // Churned subscriptions in this month
            var churnedSubscriptions = subscriptions.Where(s => 
                s.EndDate >= currentDate && 
                s.EndDate <= monthEndDate && 
                s.Status == SubscriptionStatus.Cancelled).ToList();

            // Calculate MRR
            var totalMRR = activeSubscriptions.Sum(s => GetMonthlyAmount(s));
            var newMRR = newSubscriptions.Sum(s => GetMonthlyAmount(s));
            var churnedMRR = churnedSubscriptions.Sum(s => GetMonthlyAmount(s));

            var mrrDto = new MonthlyRecurringRevenueDto
            {
                Year = currentDate.Year,
                Month = currentDate.Month,
                MonthName = currentDate.ToString("MMMM"),
                MonthlyRecurringRevenue = totalMRR,
                NewMRR = newMRR,
                ChurnedMRR = churnedMRR,
                ExpansionMRR = 0, // TODO: Calculate based on subscription changes
                ContractionMRR = 0, // TODO: Calculate based on subscription changes
                ActiveSubscriptions = activeSubscriptions.Count,
                NewSubscriptions = newSubscriptions.Count,
                ChurnedSubscriptions = churnedSubscriptions.Count,
                Currency = request.Currency ?? "USD"
            };

            mrrData.Add(mrrDto);
            currentDate = currentDate.AddMonths(1);
        }

        return mrrData;
    }

    private static decimal GetMonthlyAmount(Subscription subscription)
    {
        return subscription.BillingFrequency switch
        {
            BillingFrequency.Monthly => subscription.MonthlyAmount,
            BillingFrequency.Quarterly => subscription.MonthlyAmount / 3,
            BillingFrequency.Annually => subscription.MonthlyAmount / 12,
            _ => 0
        };
    }
}
