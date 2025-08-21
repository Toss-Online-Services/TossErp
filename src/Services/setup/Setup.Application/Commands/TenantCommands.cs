namespace TossErp.Setup.Application.Commands;

// ============================================================================
// TENANT MANAGEMENT COMMANDS
// ============================================================================

/// <summary>
/// Create new tenant command
/// </summary>
public record CreateTenantCommand(
    string Name,
    string Domain,
    SubscriptionPlan SubscriptionPlan,
    ContactInfo ContactInfo,
    Dictionary<string, string>? Settings = null
) : IRequest<TenantDto>;

public class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(100)
            .WithMessage("Tenant name must be between 2 and 100 characters");

        RuleFor(x => x.Domain)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(@"^[a-z0-9-]+$")
            .WithMessage("Domain must contain only lowercase letters, numbers, and hyphens");

        RuleFor(x => x.SubscriptionPlan)
            .IsInEnum()
            .WithMessage("Invalid subscription plan");

        RuleFor(x => x.ContactInfo.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Valid email address is required");

        RuleFor(x => x.ContactInfo.Phone)
            .NotEmpty()
            .WithMessage("Phone number is required");
    }
}

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, TenantDto>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ITenantService _tenantService;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateTenantCommandHandler> _logger;

    public CreateTenantCommandHandler(
        ITenantRepository tenantRepository,
        ITenantService tenantService,
        IAuditService auditService,
        INotificationService notificationService,
        IMapper mapper,
        ILogger<CreateTenantCommandHandler> logger)
    {
        _tenantRepository = tenantRepository;
        _tenantService = tenantService;
        _auditService = auditService;
        _notificationService = notificationService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<TenantDto> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating tenant: {TenantName}", request.Name);

        // Validate domain availability
        if (await _tenantRepository.ExistsByDomainAsync(request.Domain, cancellationToken))
        {
            throw new ValidationException($"Domain '{request.Domain}' is already in use");
        }

        // Validate tenant name
        if (await _tenantRepository.ExistsByNameAsync(request.Name, cancellationToken))
        {
            throw new ValidationException($"Tenant name '{request.Name}' is already in use");
        }

        // Create tenant using domain service
        var tenant = await _tenantService.CreateTenantAsync(
            request.Name,
            request.Domain,
            request.SubscriptionPlan,
            cancellationToken);

        // Update contact info and settings
        tenant.UpdateContactInfo(request.ContactInfo);
        if (request.Settings != null)
        {
            foreach (var setting in request.Settings)
            {
                tenant.UpdateSetting(setting.Key, setting.Value);
            }
        }

        // Save tenant
        tenant = await _tenantRepository.UpdateAsync(tenant, cancellationToken);

        // Initialize tenant data
        await _tenantService.InitializeTenantDataAsync(tenant.Id, cancellationToken);

        // Log audit trail
        await _auditService.LogSystemActionAsync(
            "TenantCreated",
            $"Created tenant: {tenant.Name} with domain: {tenant.Domain}",
            cancellationToken);

        // Send welcome notification
        await _notificationService.SendTenantWelcomeEmailAsync(tenant.Id, cancellationToken);

        _logger.LogInformation("Successfully created tenant: {TenantId}", tenant.Id);

        return _mapper.Map<TenantDto>(tenant);
    }
}

/// <summary>
/// Update tenant command
/// </summary>
public record UpdateTenantCommand(
    Guid TenantId,
    string Name,
    ContactInfo ContactInfo,
    Dictionary<string, string>? Settings = null
) : IRequest<TenantDto>;

public class UpdateTenantCommandValidator : AbstractValidator<UpdateTenantCommand>
{
    public UpdateTenantCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("Tenant ID is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(100)
            .WithMessage("Tenant name must be between 2 and 100 characters");

        RuleFor(x => x.ContactInfo.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Valid email address is required");
    }
}

public class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, TenantDto>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IAuditService _auditService;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateTenantCommandHandler> _logger;

    public UpdateTenantCommandHandler(
        ITenantRepository tenantRepository,
        IAuditService auditService,
        IMapper mapper,
        ILogger<UpdateTenantCommandHandler> logger)
    {
        _tenantRepository = tenantRepository;
        _auditService = auditService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<TenantDto> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating tenant: {TenantId}", request.TenantId);

        var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken)
            ?? throw new NotFoundException($"Tenant with ID {request.TenantId} not found");

        var oldValues = new { tenant.Name, tenant.ContactInfo, tenant.Settings };

        // Update tenant properties
        tenant.UpdateName(request.Name);
        tenant.UpdateContactInfo(request.ContactInfo);

        if (request.Settings != null)
        {
            foreach (var setting in request.Settings)
            {
                tenant.UpdateSetting(setting.Key, setting.Value);
            }
        }

        // Save changes
        tenant = await _tenantRepository.UpdateAsync(tenant, cancellationToken);

        // Log audit trail
        await _auditService.LogAsync(
            "TenantUpdated",
            nameof(Tenant),
            tenant.Id,
            Guid.Empty, // System action
            tenant.Id,
            oldValues,
            new { tenant.Name, tenant.ContactInfo, tenant.Settings },
            cancellationToken);

        _logger.LogInformation("Successfully updated tenant: {TenantId}", tenant.Id);

        return _mapper.Map<TenantDto>(tenant);
    }
}

/// <summary>
/// Suspend tenant command
/// </summary>
public record SuspendTenantCommand(
    Guid TenantId,
    string Reason
) : IRequest<TenantDto>;

public class SuspendTenantCommandValidator : AbstractValidator<SuspendTenantCommand>
{
    public SuspendTenantCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("Tenant ID is required");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(500)
            .WithMessage("Suspension reason is required and must not exceed 500 characters");
    }
}

public class SuspendTenantCommandHandler : IRequestHandler<SuspendTenantCommand, TenantDto>
{
    private readonly ITenantService _tenantService;
    private readonly ITenantRepository _tenantRepository;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;
    private readonly ILogger<SuspendTenantCommandHandler> _logger;

    public SuspendTenantCommandHandler(
        ITenantService tenantService,
        ITenantRepository tenantRepository,
        IAuditService auditService,
        INotificationService notificationService,
        IMapper mapper,
        ILogger<SuspendTenantCommandHandler> logger)
    {
        _tenantService = tenantService;
        _tenantRepository = tenantRepository;
        _auditService = auditService;
        _notificationService = notificationService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<TenantDto> Handle(SuspendTenantCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Suspending tenant: {TenantId}", request.TenantId);

        var tenant = await _tenantService.SuspendTenantAsync(
            request.TenantId,
            request.Reason,
            cancellationToken);

        // Log audit trail
        await _auditService.LogAsync(
            "TenantSuspended",
            nameof(Tenant),
            tenant.Id,
            Guid.Empty, // System action
            tenant.Id,
            null,
            new { Reason = request.Reason },
            cancellationToken);

        // Send suspension notice
        await _notificationService.SendTenantSuspensionNoticeAsync(
            tenant.Id,
            request.Reason,
            cancellationToken);

        _logger.LogInformation("Successfully suspended tenant: {TenantId}", tenant.Id);

        return _mapper.Map<TenantDto>(tenant);
    }
}

/// <summary>
/// Activate tenant command
/// </summary>
public record ActivateTenantCommand(Guid TenantId) : IRequest<TenantDto>;

public class ActivateTenantCommandValidator : AbstractValidator<ActivateTenantCommand>
{
    public ActivateTenantCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("Tenant ID is required");
    }
}

public class ActivateTenantCommandHandler : IRequestHandler<ActivateTenantCommand, TenantDto>
{
    private readonly ITenantService _tenantService;
    private readonly IAuditService _auditService;
    private readonly IMapper _mapper;
    private readonly ILogger<ActivateTenantCommandHandler> _logger;

    public ActivateTenantCommandHandler(
        ITenantService tenantService,
        IAuditService auditService,
        IMapper mapper,
        ILogger<ActivateTenantCommandHandler> logger)
    {
        _tenantService = tenantService;
        _auditService = auditService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<TenantDto> Handle(ActivateTenantCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Activating tenant: {TenantId}", request.TenantId);

        var tenant = await _tenantService.ActivateTenantAsync(request.TenantId, cancellationToken);

        // Log audit trail
        await _auditService.LogAsync(
            "TenantActivated",
            nameof(Tenant),
            tenant.Id,
            Guid.Empty, // System action
            tenant.Id,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Successfully activated tenant: {TenantId}", tenant.Id);

        return _mapper.Map<TenantDto>(tenant);
    }
}

/// <summary>
/// Upgrade tenant plan command
/// </summary>
public record UpgradeTenantPlanCommand(
    Guid TenantId,
    SubscriptionPlan NewPlan
) : IRequest<TenantDto>;

public class UpgradeTenantPlanCommandValidator : AbstractValidator<UpgradeTenantPlanCommand>
{
    public UpgradeTenantPlanCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("Tenant ID is required");

        RuleFor(x => x.NewPlan)
            .IsInEnum()
            .WithMessage("Invalid subscription plan");
    }
}

public class UpgradeTenantPlanCommandHandler : IRequestHandler<UpgradeTenantPlanCommand, TenantDto>
{
    private readonly ITenantService _tenantService;
    private readonly IAuditService _auditService;
    private readonly IMapper _mapper;
    private readonly ILogger<UpgradeTenantPlanCommandHandler> _logger;

    public UpgradeTenantPlanCommandHandler(
        ITenantService tenantService,
        IAuditService auditService,
        IMapper mapper,
        ILogger<UpgradeTenantPlanCommandHandler> logger)
    {
        _tenantService = tenantService;
        _auditService = auditService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<TenantDto> Handle(UpgradeTenantPlanCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Upgrading tenant plan: {TenantId} to {NewPlan}", 
            request.TenantId, request.NewPlan);

        var tenant = await _tenantService.UpgradeTenantPlanAsync(
            request.TenantId,
            request.NewPlan,
            cancellationToken);

        // Log audit trail
        await _auditService.LogAsync(
            "TenantPlanUpgraded",
            nameof(Tenant),
            tenant.Id,
            Guid.Empty, // System action
            tenant.Id,
            null,
            new { NewPlan = request.NewPlan.ToString() },
            cancellationToken);

        _logger.LogInformation("Successfully upgraded tenant plan: {TenantId}", tenant.Id);

        return _mapper.Map<TenantDto>(tenant);
    }
}
