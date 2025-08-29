using FluentValidation;

namespace TossErp.Accounts.Application.Commands;

/// <summary>
/// Command to create a new subscription
/// </summary>
public record CreateSubscriptionCommand(
    Guid CustomerId,
    string PlanName,
    decimal PlanPrice,
    string Currency,
    BillingFrequency BillingFrequency,
    DateOnly StartDate,
    DateOnly? EndDate,
    int? BillingCycleDay,
    int? TrialDays,
    bool AutoRenew,
    string? Notes
) : IRequest<SubscriptionDto>;

/// <summary>
/// Handler for creating subscriptions
/// </summary>
public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, SubscriptionDto>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<CreateSubscriptionCommandHandler> _logger;

    public CreateSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        ILogger<CreateSubscriptionCommandHandler> logger)
    {
        _subscriptionRepository = subscriptionRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<SubscriptionDto> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating subscription for customer: {CustomerId}, Plan: {PlanName}", 
            request.CustomerId, request.PlanName);

        var tenantId = _currentUserService.TenantId ?? throw new InvalidOperationException("Tenant ID is required");
        var currentUserId = _currentUserService.UserId ?? "system";

        // Validate customer
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer == null)
        {
            throw new InvalidOperationException($"Customer with ID {request.CustomerId} not found");
        }

        if (customer.Status != CustomerStatus.Active)
        {
            throw new InvalidOperationException($"Cannot create subscription for inactive customer {request.CustomerId}");
        }

        // Check for existing active subscription
        var existingSubscription = await _subscriptionRepository.GetActiveByCustomerIdAsync(request.CustomerId, cancellationToken);
        if (existingSubscription != null)
        {
            throw new InvalidOperationException($"Customer {request.CustomerId} already has an active subscription");
        }

        // Calculate billing cycle day
        var billingCycleDay = request.BillingCycleDay ?? request.StartDate.Day;
        if (billingCycleDay > 28)
        {
            billingCycleDay = 28; // Ensure valid day for all months
        }

        // Create subscription
        var subscription = Subscription.Create(
            tenantId: tenantId,
            subscriptionNumber: $"SUB-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}",
            customerId: request.CustomerId,
            customerName: customer.Name,
            planName: request.PlanName,
            monthlyAmount: new Money(request.PlanPrice, CurrencyCode.ZAR),
            billingFrequency: request.BillingFrequency,
            startDate: request.StartDate.ToDateTime(TimeOnly.MinValue),
            createdBy: currentUserId,
            planDescription: request.Notes);

        // Save subscription
        await _subscriptionRepository.AddAsync(subscription, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send subscription confirmation
        if (!string.IsNullOrEmpty(customer.Email))
        {
            await _notificationService.SendSubscriptionNotificationAsync(
                subscription.Id, customer.Email, SubscriptionNotificationType.SubscriptionCreated, cancellationToken);
        }

        // Publish domain events
        await _domainEventService.PublishAsync(subscription.DomainEvents, cancellationToken);
        subscription.ClearDomainEvents();

        _logger.LogInformation("Successfully created subscription {SubscriptionId}", subscription.Id);

        return MapToDto(subscription, customer);
    }

    private static SubscriptionDto MapToDto(Subscription subscription, Customer customer)
    {
        return new SubscriptionDto
        {
            Id = subscription.Id,
            TenantId = subscription.TenantId,
            CustomerId = subscription.CustomerId,
            CustomerName = customer.Name,
            PlanName = subscription.PlanName,
            PlanPrice = subscription.MonthlyAmount.Amount,
            Currency = subscription.Currency,
            BillingFrequency = subscription.BillingFrequency,
            Status = subscription.Status,
            StartDate = DateOnly.FromDateTime(subscription.StartDate),
            EndDate = subscription.EndDate.HasValue ? DateOnly.FromDateTime(subscription.EndDate.Value) : null,
            NextBillingDate = DateOnly.FromDateTime(subscription.NextBillingDate),
            BillingCycleDay = subscription.BillingCycle,
            TrialDays = subscription.TrialDays,
            TrialEndDate = subscription.TrialEndDate.HasValue ? DateOnly.FromDateTime(subscription.TrialEndDate.Value) : null,
            AutoRenew = subscription.AutoRenew,
            Notes = null,
            CancellationReason = subscription.CancellationReason,
            CancelledAt = subscription.CancelledDate,
            CancelledBy = subscription.CancelledBy,
            CreatedAt = subscription.CreatedAt,
            CreatedBy = subscription.CreatedBy,
            LastModified = subscription.ModifiedAt,
            LastModifiedBy = subscription.ModifiedBy
        };
    }
}

/// <summary>
/// Validator for CreateSubscriptionCommand
/// </summary>
public class CreateSubscriptionCommandValidator : AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required");

        RuleFor(x => x.PlanName)
            .NotEmpty()
            .WithMessage("Plan name is required")
            .MaximumLength(100)
            .WithMessage("Plan name cannot exceed 100 characters");

        RuleFor(x => x.PlanPrice)
            .GreaterThan(0)
            .WithMessage("Plan price must be greater than 0");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required")
            .Length(3)
            .WithMessage("Currency must be a 3-letter code");

        RuleFor(x => x.BillingFrequency)
            .IsInEnum()
            .WithMessage("Valid billing frequency is required");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .When(x => x.EndDate.HasValue)
            .WithMessage("End date must be after start date");

        RuleFor(x => x.BillingCycleDay)
            .InclusiveBetween(1, 28)
            .When(x => x.BillingCycleDay.HasValue)
            .WithMessage("Billing cycle day must be between 1 and 28");

        RuleFor(x => x.TrialDays)
            .GreaterThan(0)
            .When(x => x.TrialDays.HasValue)
            .WithMessage("Trial days must be greater than 0");
    }
}

/// <summary>
/// Command to update a subscription
/// </summary>
public record UpdateSubscriptionCommand(
    Guid SubscriptionId,
    string PlanName,
    decimal PlanPrice,
    BillingFrequency BillingFrequency,
    DateOnly? EndDate,
    int BillingCycleDay,
    bool AutoRenew,
    string? Notes
) : IRequest<SubscriptionDto>;

/// <summary>
/// Handler for updating subscriptions
/// </summary>
public class UpdateSubscriptionCommandHandler : IRequestHandler<UpdateSubscriptionCommand, SubscriptionDto>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateSubscriptionCommandHandler> _logger;

    public UpdateSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        ILogger<UpdateSubscriptionCommandHandler> logger)
    {
        _subscriptionRepository = subscriptionRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<SubscriptionDto> Handle(UpdateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating subscription: {SubscriptionId}", request.SubscriptionId);

        var subscription = await _subscriptionRepository.GetByIdAsync(request.SubscriptionId, cancellationToken);
        if (subscription == null)
        {
            throw new InvalidOperationException($"Subscription with ID {request.SubscriptionId} not found");
        }

        if (subscription.Status == SubscriptionStatus.Cancelled)
        {
            throw new InvalidOperationException($"Cannot update cancelled subscription {request.SubscriptionId}");
        }

        var customer = await _customerRepository.GetByIdAsync(subscription.CustomerId, cancellationToken);
        if (customer == null)
        {
            throw new InvalidOperationException($"Customer with ID {subscription.CustomerId} not found");
        }

        var currentUserId = _currentUserService.UserId ?? "system";

        // Update subscription
        subscription.UpdatePlan(
            planName: request.PlanName,
            newMonthlyAmount: new Money(request.PlanPrice, CurrencyCode.ZAR),
            planDescription: request.Notes,
            modifiedBy: currentUserId);

        // Save changes
        await _subscriptionRepository.UpdateAsync(subscription, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain events
        await _domainEventService.PublishAsync(subscription.DomainEvents, cancellationToken);
        subscription.ClearDomainEvents();

        _logger.LogInformation("Successfully updated subscription {SubscriptionId}", request.SubscriptionId);

        return MapToDto(subscription, customer);
    }

    private static SubscriptionDto MapToDto(Subscription subscription, Customer customer)
    {
        return new SubscriptionDto
        {
            Id = subscription.Id,
            TenantId = subscription.TenantId,
            CustomerId = subscription.CustomerId,
            CustomerName = customer.Name,
            PlanName = subscription.PlanName,
            PlanPrice = subscription.MonthlyAmount.Amount,
            Currency = subscription.Currency,
            BillingFrequency = subscription.BillingFrequency,
            Status = subscription.Status,
            StartDate = DateOnly.FromDateTime(subscription.StartDate),
            EndDate = subscription.EndDate.HasValue ? DateOnly.FromDateTime(subscription.EndDate.Value) : null,
            NextBillingDate = DateOnly.FromDateTime(subscription.NextBillingDate),
            BillingCycleDay = subscription.BillingCycle,
            TrialDays = subscription.TrialDays,
            TrialEndDate = subscription.TrialEndDate.HasValue ? DateOnly.FromDateTime(subscription.TrialEndDate.Value) : null,
            AutoRenew = subscription.AutoRenew,
            Notes = null,
            CancellationReason = subscription.CancellationReason,
            CancelledAt = subscription.CancelledDate,
            CancelledBy = subscription.CancelledBy,
            CreatedAt = subscription.CreatedAt,
            CreatedBy = subscription.CreatedBy,
            LastModified = subscription.ModifiedAt,
            LastModifiedBy = subscription.ModifiedBy
        };
    }
}

/// <summary>
/// Validator for UpdateSubscriptionCommand
/// </summary>
public class UpdateSubscriptionCommandValidator : AbstractValidator<UpdateSubscriptionCommand>
{
    public UpdateSubscriptionCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .NotEmpty()
            .WithMessage("Subscription ID is required");

        RuleFor(x => x.PlanName)
            .NotEmpty()
            .WithMessage("Plan name is required")
            .MaximumLength(100)
            .WithMessage("Plan name cannot exceed 100 characters");

        RuleFor(x => x.PlanPrice)
            .GreaterThan(0)
            .WithMessage("Plan price must be greater than 0");

        RuleFor(x => x.BillingFrequency)
            .IsInEnum()
            .WithMessage("Valid billing frequency is required");

        RuleFor(x => x.BillingCycleDay)
            .InclusiveBetween(1, 28)
            .WithMessage("Billing cycle day must be between 1 and 28");
    }
}

/// <summary>
/// Command to cancel a subscription
/// </summary>
public record CancelSubscriptionCommand(
    Guid SubscriptionId,
    string Reason,
    bool ImmediateCancel = false
) : IRequest<SubscriptionDto>;

/// <summary>
/// Handler for cancelling subscriptions
/// </summary>
public class CancelSubscriptionCommandHandler : IRequestHandler<CancelSubscriptionCommand, SubscriptionDto>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<CancelSubscriptionCommandHandler> _logger;

    public CancelSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        ILogger<CancelSubscriptionCommandHandler> logger)
    {
        _subscriptionRepository = subscriptionRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<SubscriptionDto> Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling subscription: {SubscriptionId}, Immediate: {ImmediateCancel}", 
            request.SubscriptionId, request.ImmediateCancel);

        var subscription = await _subscriptionRepository.GetByIdAsync(request.SubscriptionId, cancellationToken);
        if (subscription == null)
        {
            throw new InvalidOperationException($"Subscription with ID {request.SubscriptionId} not found");
        }

        if (subscription.Status == SubscriptionStatus.Cancelled)
        {
            throw new InvalidOperationException($"Subscription {request.SubscriptionId} is already cancelled");
        }

        var customer = await _customerRepository.GetByIdAsync(subscription.CustomerId, cancellationToken);
        if (customer == null)
        {
            throw new InvalidOperationException($"Customer with ID {subscription.CustomerId} not found");
        }

        var currentUserId = _currentUserService.UserId ?? "system";

        // Cancel subscription
        if (request.ImmediateCancel)
        {
            subscription.CancelImmediately(request.Reason, currentUserId);
        }
        else
        {
            subscription.CancelAtPeriodEnd(request.Reason, currentUserId);
        }

        // Save changes
        await _subscriptionRepository.UpdateAsync(subscription, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send cancellation notification
        if (!string.IsNullOrEmpty(customer.Email))
        {
            await _notificationService.SendSubscriptionNotificationAsync(
                subscription.Id, customer.Email, SubscriptionNotificationType.SubscriptionCancelled, cancellationToken);
        }

        // Publish domain events
        await _domainEventService.PublishAsync(subscription.DomainEvents, cancellationToken);
        subscription.ClearDomainEvents();

        _logger.LogInformation("Successfully cancelled subscription {SubscriptionId}", request.SubscriptionId);

        return MapToDto(subscription, customer);
    }

    private static SubscriptionDto MapToDto(Subscription subscription, Customer customer)
    {
        return new SubscriptionDto
        {
            Id = subscription.Id,
            TenantId = subscription.TenantId,
            CustomerId = subscription.CustomerId,
            CustomerName = customer.Name,
            PlanName = subscription.PlanName,
            PlanPrice = subscription.MonthlyAmount.Amount,
            Currency = subscription.Currency,
            BillingFrequency = subscription.BillingFrequency,
            Status = subscription.Status,
            StartDate = DateOnly.FromDateTime(subscription.StartDate),
            EndDate = subscription.EndDate.HasValue ? DateOnly.FromDateTime(subscription.EndDate.Value) : null,
            NextBillingDate = DateOnly.FromDateTime(subscription.NextBillingDate),
            BillingCycleDay = subscription.BillingCycle,
            TrialDays = subscription.TrialDays,
            TrialEndDate = subscription.TrialEndDate.HasValue ? DateOnly.FromDateTime(subscription.TrialEndDate.Value) : null,
            AutoRenew = subscription.AutoRenew,
            Notes = null,
            CancellationReason = subscription.CancellationReason,
            CancelledAt = subscription.CancelledDate,
            CancelledBy = subscription.CancelledBy,
            CreatedAt = subscription.CreatedAt,
            CreatedBy = subscription.CreatedBy,
            LastModified = subscription.ModifiedAt,
            LastModifiedBy = subscription.ModifiedBy
        };
    }
}

/// <summary>
/// Validator for CancelSubscriptionCommand
/// </summary>
public class CancelSubscriptionCommandValidator : AbstractValidator<CancelSubscriptionCommand>
{
    public CancelSubscriptionCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .NotEmpty()
            .WithMessage("Subscription ID is required");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Cancellation reason is required")
            .MaximumLength(500)
            .WithMessage("Cancellation reason cannot exceed 500 characters");
    }
}

/// <summary>
/// Command to renew a subscription
/// </summary>
public record RenewSubscriptionCommand(
    Guid SubscriptionId,
    DateOnly? NewEndDate
) : IRequest<SubscriptionDto>;

/// <summary>
/// Handler for renewing subscriptions
/// </summary>
public class RenewSubscriptionCommandHandler : IRequestHandler<RenewSubscriptionCommand, SubscriptionDto>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<RenewSubscriptionCommandHandler> _logger;

    public RenewSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        ILogger<RenewSubscriptionCommandHandler> logger)
    {
        _subscriptionRepository = subscriptionRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<SubscriptionDto> Handle(RenewSubscriptionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Renewing subscription: {SubscriptionId}", request.SubscriptionId);

        var subscription = await _subscriptionRepository.GetByIdAsync(request.SubscriptionId, cancellationToken);
        if (subscription == null)
        {
            throw new InvalidOperationException($"Subscription with ID {request.SubscriptionId} not found");
        }

        if (subscription.Status != SubscriptionStatus.Active && subscription.Status != SubscriptionStatus.PendingCancellation)
        {
            throw new InvalidOperationException($"Cannot renew subscription {request.SubscriptionId} in status {subscription.Status}");
        }

        var customer = await _customerRepository.GetByIdAsync(subscription.CustomerId, cancellationToken);
        if (customer == null)
        {
            throw new InvalidOperationException($"Customer with ID {subscription.CustomerId} not found");
        }

        var currentUserId = _currentUserService.UserId ?? "system";

        // Renew subscription
        subscription.Renew(request.NewEndDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.UtcNow.AddYears(1), currentUserId);

        // Save changes
        await _subscriptionRepository.UpdateAsync(subscription, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send renewal notification
        if (!string.IsNullOrEmpty(customer.Email))
        {
            await _notificationService.SendSubscriptionNotificationAsync(
                subscription.Id, customer.Email, SubscriptionNotificationType.SubscriptionRenewed, cancellationToken);
        }

        // Publish domain events
        await _domainEventService.PublishAsync(subscription.DomainEvents, cancellationToken);
        subscription.ClearDomainEvents();

        _logger.LogInformation("Successfully renewed subscription {SubscriptionId}", request.SubscriptionId);

        return MapToDto(subscription, customer);
    }

    private static SubscriptionDto MapToDto(Subscription subscription, Customer customer)
    {
        return new SubscriptionDto
        {
            Id = subscription.Id,
            TenantId = subscription.TenantId,
            CustomerId = subscription.CustomerId,
            CustomerName = customer.Name,
            PlanName = subscription.PlanName,
            PlanPrice = subscription.MonthlyAmount.Amount,
            Currency = subscription.Currency,
            BillingFrequency = subscription.BillingFrequency,
            Status = subscription.Status,
            StartDate = DateOnly.FromDateTime(subscription.StartDate),
            EndDate = subscription.EndDate.HasValue ? DateOnly.FromDateTime(subscription.EndDate.Value) : null,
            NextBillingDate = DateOnly.FromDateTime(subscription.NextBillingDate),
            BillingCycleDay = subscription.BillingCycle,
            TrialDays = subscription.TrialDays,
            TrialEndDate = subscription.TrialEndDate.HasValue ? DateOnly.FromDateTime(subscription.TrialEndDate.Value) : null,
            AutoRenew = subscription.AutoRenew,
            Notes = null,
            CancellationReason = subscription.CancellationReason,
            CancelledAt = subscription.CancelledDate,
            CancelledBy = subscription.CancelledBy,
            CreatedAt = subscription.CreatedAt,
            CreatedBy = subscription.CreatedBy,
            LastModified = subscription.ModifiedAt,
            LastModifiedBy = subscription.ModifiedBy
        };
    }
}

/// <summary>
/// Validator for RenewSubscriptionCommand
/// </summary>
public class RenewSubscriptionCommandValidator : AbstractValidator<RenewSubscriptionCommand>
{
    public RenewSubscriptionCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .NotEmpty()
            .WithMessage("Subscription ID is required");

        RuleFor(x => x.NewEndDate)
            .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow))
            .When(x => x.NewEndDate.HasValue)
            .WithMessage("New end date must be in the future");
    }
}
