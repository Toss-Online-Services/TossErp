namespace TossErp.Accounts.Application.Queries;

/// <summary>
/// Query to get subscriptions with filtering and pagination
/// </summary>
public record GetSubscriptionsQuery(
    Guid? CustomerId = null,
    SubscriptionStatus? Status = null,
    BillingFrequency? BillingFrequency = null,
    DateOnly? StartDateFrom = null,
    DateOnly? StartDateTo = null,
    DateOnly? EndDateFrom = null,
    DateOnly? EndDateTo = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    string? Currency = null,
    int PageNumber = 1,
    int PageSize = 10,
    string SortBy = "StartDate",
    bool SortDescending = true
) : IRequest<PaginatedResult<SubscriptionDto>>;

/// <summary>
/// Handler for getting subscriptions
/// </summary>
public class GetSubscriptionsQueryHandler : IRequestHandler<GetSubscriptionsQuery, PaginatedResult<SubscriptionDto>>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<GetSubscriptionsQueryHandler> _logger;

    public GetSubscriptionsQueryHandler(
        ISubscriptionRepository subscriptionRepository,
        ICustomerRepository customerRepository,
        ILogger<GetSubscriptionsQueryHandler> logger)
    {
        _subscriptionRepository = subscriptionRepository;
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<PaginatedResult<SubscriptionDto>> Handle(GetSubscriptionsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting subscriptions with customer: {CustomerId}, Status: {Status}", 
            request.CustomerId, request.Status);

        var filter = new SubscriptionFilter
        {
            CustomerId = request.CustomerId,
            Status = request.Status,
            // BillingFrequency = request.BillingFrequency, // Not available in filter
            StartDateFrom = request.StartDateFrom?.ToDateTime(TimeOnly.MinValue),
            StartDateTo = request.StartDateTo?.ToDateTime(TimeOnly.MaxValue),
            EndDateFrom = request.EndDateFrom?.ToDateTime(TimeOnly.MinValue),
            EndDateTo = request.EndDateTo?.ToDateTime(TimeOnly.MaxValue),
            // MinPrice = request.MinPrice, // Not available in filter
            // MaxPrice = request.MaxPrice, // Not available in filter
            // Currency = request.Currency // Not available in filter
        };

        var subscriptions = await _subscriptionRepository.GetPagedAsync(
            filter,
            request.PageNumber,
            request.PageSize,
            request.SortBy,
            request.SortDescending,
            cancellationToken);

        // Get customer information for mapping
        var customerIds = subscriptions.Subscriptions.Select(s => s.CustomerId).Distinct().ToList();
        var customers = new Dictionary<Guid, Customer>();
        
        foreach (var customerId in customerIds)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
            if (customer != null)
            {
                customers[customerId] = customer;
            }
        }

        var subscriptionDtos = subscriptions.Subscriptions.Select(subscription => MapToDto(subscription, customers.GetValueOrDefault(subscription.CustomerId))).ToList();

        return new PaginatedResult<SubscriptionDto>
        {
            Items = subscriptionDtos,
            TotalCount = subscriptions.TotalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)subscriptions.TotalCount / request.PageSize)
        };
    }

    private static SubscriptionDto MapToDto(Subscription subscription, Customer? customer)
    {
        return new SubscriptionDto
        {
            Id = subscription.Id,
            TenantId = subscription.TenantId,
            CustomerId = subscription.CustomerId,
            CustomerName = customer?.Name ?? "Unknown Customer",
            PlanName = subscription.PlanName,
            PlanPrice = subscription.MonthlyAmount.Amount,
            Currency = subscription.Currency,
            BillingFrequency = subscription.BillingFrequency,
            Status = subscription.Status,
            StartDate = DateOnly.FromDateTime(subscription.StartDate),
            EndDate = subscription.EndDate.HasValue ? DateOnly.FromDateTime(subscription.EndDate.Value) : null,
            NextBillingDate = DateOnly.FromDateTime(subscription.NextBillingDate),
            BillingCycleDay = 1, // Default billing cycle day
            TrialDays = subscription.TrialDays,
            TrialEndDate = subscription.TrialEndDate.HasValue ? DateOnly.FromDateTime(subscription.TrialEndDate.Value) : null,
            AutoRenew = subscription.AutoRenew,
            Notes = null, // Notes not available in domain entity
            CancellationReason = subscription.CancellationReason,
            CancelledAt = null, // CancelledAt not available in domain entity
            CancelledBy = subscription.CancelledBy,
            CreatedAt = subscription.CreatedAt,
            CreatedBy = subscription.CreatedBy,
            LastModified = subscription.UpdatedAt,
            LastModifiedBy = subscription.UpdatedBy
        };
    }
}

/// <summary>
/// Query to get a subscription by ID
/// </summary>
public record GetSubscriptionByIdQuery(Guid SubscriptionId) : IRequest<SubscriptionDto?>;

/// <summary>
/// Handler for getting a subscription by ID
/// </summary>
public class GetSubscriptionByIdQueryHandler : IRequestHandler<GetSubscriptionByIdQuery, SubscriptionDto?>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<GetSubscriptionByIdQueryHandler> _logger;

    public GetSubscriptionByIdQueryHandler(
        ISubscriptionRepository subscriptionRepository,
        ICustomerRepository customerRepository,
        ILogger<GetSubscriptionByIdQueryHandler> logger)
    {
        _subscriptionRepository = subscriptionRepository;
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<SubscriptionDto?> Handle(GetSubscriptionByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting subscription by ID: {SubscriptionId}", request.SubscriptionId);

        var subscription = await _subscriptionRepository.GetByIdAsync(request.SubscriptionId, cancellationToken);
        if (subscription == null)
        {
            return null;
        }

        var customer = await _customerRepository.GetByIdAsync(subscription.CustomerId, cancellationToken);

        return MapToDto(subscription, customer);
    }

    private static SubscriptionDto MapToDto(Subscription subscription, Customer? customer)
    {
        return new SubscriptionDto
        {
            Id = subscription.Id,
            TenantId = subscription.TenantId,
            CustomerId = subscription.CustomerId,
            CustomerName = customer?.Name ?? "Unknown Customer",
            PlanName = subscription.PlanName,
            PlanPrice = subscription.MonthlyAmount.Amount,
            Currency = subscription.Currency,
            BillingFrequency = subscription.BillingFrequency,
            Status = subscription.Status,
            StartDate = DateOnly.FromDateTime(subscription.StartDate),
            EndDate = subscription.EndDate.HasValue ? DateOnly.FromDateTime(subscription.EndDate.Value) : null,
            NextBillingDate = DateOnly.FromDateTime(subscription.NextBillingDate),
            BillingCycleDay = 1, // Default billing cycle day
            TrialDays = subscription.TrialDays,
            TrialEndDate = subscription.TrialEndDate.HasValue ? DateOnly.FromDateTime(subscription.TrialEndDate.Value) : null,
            AutoRenew = subscription.AutoRenew,
            Notes = null, // Notes not available in domain entity
            CancellationReason = subscription.CancellationReason,
            CancelledAt = null, // CancelledAt not available in domain entity
            CancelledBy = subscription.CancelledBy,
            CreatedAt = subscription.CreatedAt,
            CreatedBy = subscription.CreatedBy,
            LastModified = subscription.UpdatedAt,
            LastModifiedBy = subscription.UpdatedBy
        };
    }
}

/// <summary>
/// Query to get subscriptions due for renewal
/// </summary>
public record GetSubscriptionsDueForRenewalQuery(
    int DaysAhead = 7,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedResult<SubscriptionDto>>;

/// <summary>
/// Handler for getting subscriptions due for renewal
/// </summary>
public class GetSubscriptionsDueForRenewalQueryHandler : IRequestHandler<GetSubscriptionsDueForRenewalQuery, PaginatedResult<SubscriptionDto>>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<GetSubscriptionsDueForRenewalQueryHandler> _logger;

    public GetSubscriptionsDueForRenewalQueryHandler(
        ISubscriptionRepository subscriptionRepository,
        ICustomerRepository customerRepository,
        ILogger<GetSubscriptionsDueForRenewalQueryHandler> logger)
    {
        _subscriptionRepository = subscriptionRepository;
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<PaginatedResult<SubscriptionDto>> Handle(GetSubscriptionsDueForRenewalQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting subscriptions due for renewal in {DaysAhead} days", request.DaysAhead);

        var renewalDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(request.DaysAhead));
        
        var subscriptions = await _subscriptionRepository.GetSubscriptionsDueForRenewalAsync(
            renewalDate,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        // Get customer information for mapping
        var customerIds = subscriptions.Subscriptions.Select(s => s.CustomerId).Distinct().ToList();
        var customers = new Dictionary<Guid, Customer>();
        
        foreach (var customerId in customerIds)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
            if (customer != null)
            {
                customers[customerId] = customer;
            }
        }

        var subscriptionDtos = subscriptions.Subscriptions.Select(subscription => MapToDto(subscription, customers.GetValueOrDefault(subscription.CustomerId))).ToList();

        return new PaginatedResult<SubscriptionDto>
        {
            Items = subscriptionDtos,
            TotalCount = subscriptions.TotalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)subscriptions.TotalCount / request.PageSize)
        };
    }

    private static SubscriptionDto MapToDto(Subscription subscription, Customer? customer)
    {
        return new SubscriptionDto
        {
            Id = subscription.Id,
            TenantId = subscription.TenantId,
            CustomerId = subscription.CustomerId,
            CustomerName = customer?.Name ?? "Unknown Customer",
            PlanName = subscription.PlanName,
            PlanPrice = subscription.MonthlyAmount.Amount,
            Currency = subscription.Currency,
            BillingFrequency = subscription.BillingFrequency,
            Status = subscription.Status,
            StartDate = DateOnly.FromDateTime(subscription.StartDate),
            EndDate = subscription.EndDate.HasValue ? DateOnly.FromDateTime(subscription.EndDate.Value) : null,
            NextBillingDate = DateOnly.FromDateTime(subscription.NextBillingDate),
            BillingCycleDay = 1, // Default billing cycle day
            TrialDays = subscription.TrialDays,
            TrialEndDate = subscription.TrialEndDate.HasValue ? DateOnly.FromDateTime(subscription.TrialEndDate.Value) : null,
            AutoRenew = subscription.AutoRenew,
            Notes = null, // Notes not available in domain entity
            CancellationReason = subscription.CancellationReason,
            CancelledAt = null, // CancelledAt not available in domain entity
            CancelledBy = subscription.CancelledBy,
            CreatedAt = subscription.CreatedAt,
            CreatedBy = subscription.CreatedBy,
            LastModified = subscription.UpdatedAt,
            LastModifiedBy = subscription.UpdatedBy
        };
    }
}
