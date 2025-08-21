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
            BillingFrequency = request.BillingFrequency,
            StartDateFrom = request.StartDateFrom,
            StartDateTo = request.StartDateTo,
            EndDateFrom = request.EndDateFrom,
            EndDateTo = request.EndDateTo,
            MinPrice = request.MinPrice,
            MaxPrice = request.MaxPrice,
            Currency = request.Currency
        };

        var subscriptions = await _subscriptionRepository.GetPagedAsync(
            filter,
            request.PageNumber,
            request.PageSize,
            request.SortBy,
            request.SortDescending,
            cancellationToken);

        // Get customer information for mapping
        var customerIds = subscriptions.Items.Select(s => s.CustomerId).Distinct().ToList();
        var customers = new Dictionary<Guid, Customer>();
        
        foreach (var customerId in customerIds)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
            if (customer != null)
            {
                customers[customerId] = customer;
            }
        }

        var subscriptionDtos = subscriptions.Items.Select(subscription => MapToDto(subscription, customers.GetValueOrDefault(subscription.CustomerId))).ToList();

        return new PaginatedResult<SubscriptionDto>
        {
            Items = subscriptionDtos,
            TotalCount = subscriptions.TotalCount,
            PageNumber = subscriptions.PageNumber,
            PageSize = subscriptions.PageSize,
            TotalPages = subscriptions.TotalPages
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
            PlanPrice = subscription.PlanPrice.Amount,
            Currency = subscription.Currency,
            BillingFrequency = subscription.BillingFrequency,
            Status = subscription.Status,
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            NextBillingDate = subscription.NextBillingDate,
            BillingCycleDay = subscription.BillingCycleDay,
            TrialDays = subscription.TrialDays,
            TrialEndDate = subscription.TrialEndDate,
            AutoRenew = subscription.AutoRenew,
            Notes = subscription.Notes,
            CancellationReason = subscription.CancellationReason,
            CancelledAt = subscription.CancelledAt,
            CancelledBy = subscription.CancelledBy,
            CreatedAt = subscription.CreatedAt,
            CreatedBy = subscription.CreatedBy,
            LastModified = subscription.LastModified,
            LastModifiedBy = subscription.LastModifiedBy
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
            PlanPrice = subscription.PlanPrice.Amount,
            Currency = subscription.Currency,
            BillingFrequency = subscription.BillingFrequency,
            Status = subscription.Status,
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            NextBillingDate = subscription.NextBillingDate,
            BillingCycleDay = subscription.BillingCycleDay,
            TrialDays = subscription.TrialDays,
            TrialEndDate = subscription.TrialEndDate,
            AutoRenew = subscription.AutoRenew,
            Notes = subscription.Notes,
            CancellationReason = subscription.CancellationReason,
            CancelledAt = subscription.CancelledAt,
            CancelledBy = subscription.CancelledBy,
            CreatedAt = subscription.CreatedAt,
            CreatedBy = subscription.CreatedBy,
            LastModified = subscription.LastModified,
            LastModifiedBy = subscription.LastModifiedBy
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
        var customerIds = subscriptions.Items.Select(s => s.CustomerId).Distinct().ToList();
        var customers = new Dictionary<Guid, Customer>();
        
        foreach (var customerId in customerIds)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
            if (customer != null)
            {
                customers[customerId] = customer;
            }
        }

        var subscriptionDtos = subscriptions.Items.Select(subscription => MapToDto(subscription, customers.GetValueOrDefault(subscription.CustomerId))).ToList();

        return new PaginatedResult<SubscriptionDto>
        {
            Items = subscriptionDtos,
            TotalCount = subscriptions.TotalCount,
            PageNumber = subscriptions.PageNumber,
            PageSize = subscriptions.PageSize,
            TotalPages = subscriptions.TotalPages
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
            PlanPrice = subscription.PlanPrice.Amount,
            Currency = subscription.Currency,
            BillingFrequency = subscription.BillingFrequency,
            Status = subscription.Status,
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            NextBillingDate = subscription.NextBillingDate,
            BillingCycleDay = subscription.BillingCycleDay,
            TrialDays = subscription.TrialDays,
            TrialEndDate = subscription.TrialEndDate,
            AutoRenew = subscription.AutoRenew,
            Notes = subscription.Notes,
            CancellationReason = subscription.CancellationReason,
            CancelledAt = subscription.CancelledAt,
            CancelledBy = subscription.CancelledBy,
            CreatedAt = subscription.CreatedAt,
            CreatedBy = subscription.CreatedBy,
            LastModified = subscription.LastModified,
            LastModifiedBy = subscription.LastModifiedBy
        };
    }
}
