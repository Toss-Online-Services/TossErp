namespace TossErp.Setup.Application.Commands;

// ============================================================================
// USER MANAGEMENT COMMANDS
// ============================================================================

/// <summary>
/// Create user command
/// </summary>
public record CreateUserCommand(
    string Email,
    string Username,
    string FirstName,
    string LastName,
    Guid TenantId,
    ContactInfo? ContactInfo = null,
    UserPreferences? Preferences = null,
    List<Guid>? RoleIds = null
) : IRequest<UserDto>;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256)
            .WithMessage("Valid email address is required");

        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(50)
            .Matches(@"^[a-zA-Z0-9_.-]+$")
            .WithMessage("Username must be 3-50 characters and contain only letters, numbers, dots, hyphens, and underscores");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("First name is required and must not exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Last name is required and must not exceed 100 characters");

        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("Tenant ID is required");
    }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IUserService _userService;
    private readonly IAuditService _auditService;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        ITenantRepository tenantRepository,
        IUserService userService,
        IAuditService auditService,
        IMapper mapper,
        ILogger<CreateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
        _userService = userService;
        _auditService = auditService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating user: {Email} for tenant: {TenantId}", 
            request.Email, request.TenantId);

        // Validate tenant exists
        var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken)
            ?? throw new NotFoundException($"Tenant with ID {request.TenantId} not found");

        // Validate unique constraints
        if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            throw new ValidationException($"User with email '{request.Email}' already exists");
        }

        if (await _userRepository.ExistsByUsernameAsync(request.Username, cancellationToken))
        {
            throw new ValidationException($"User with username '{request.Username}' already exists");
        }

        // Create user using domain service
        var user = await _userService.CreateUserAsync(
            request.Email,
            request.Username,
            request.FirstName,
            request.LastName,
            request.TenantId,
            cancellationToken);

        // Update additional properties
        if (request.ContactInfo != null)
        {
            user.UpdateContactInfo(request.ContactInfo);
        }

        if (request.Preferences != null)
        {
            user.UpdatePreferences(request.Preferences);
        }

        // Save user
        user = await _userRepository.UpdateAsync(user, cancellationToken);

        // Assign roles if provided
        if (request.RoleIds != null && request.RoleIds.Any())
        {
            foreach (var roleId in request.RoleIds)
            {
                await _userService.AssignRoleAsync(user.Id, roleId, cancellationToken);
            }
        }

        // Log audit trail
        await _auditService.LogAsync(
            "UserCreated",
            nameof(User),
            user.Id,
            Guid.Empty, // System action
            user.TenantId,
            null,
            new { user.Email, user.Username, user.FirstName, user.LastName },
            cancellationToken);

        // Send welcome email
        await _userService.SendWelcomeEmailAsync(user.Id, cancellationToken);

        _logger.LogInformation("Successfully created user: {UserId}", user.Id);

        return _mapper.Map<UserDto>(user);
    }
}

/// <summary>
/// Update user command
/// </summary>
public record UpdateUserCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    ContactInfo? ContactInfo = null,
    UserPreferences? Preferences = null
) : IRequest<UserDto>;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("First name is required and must not exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Last name is required and must not exceed 100 characters");
    }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuditService _auditService;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateUserCommandHandler> _logger;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IAuditService auditService,
        IMapper mapper,
        ILogger<UpdateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _auditService = auditService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating user: {UserId}", request.UserId);

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException($"User with ID {request.UserId} not found");

        var oldValues = new { user.FirstName, user.LastName, user.ContactInfo, user.Preferences };

        // Update user properties
        user.UpdateName(request.FirstName, request.LastName);

        if (request.ContactInfo != null)
        {
            user.UpdateContactInfo(request.ContactInfo);
        }

        if (request.Preferences != null)
        {
            user.UpdatePreferences(request.Preferences);
        }

        // Save changes
        user = await _userRepository.UpdateAsync(user, cancellationToken);

        // Log audit trail
        await _auditService.LogAsync(
            "UserUpdated",
            nameof(User),
            user.Id,
            user.Id,
            user.TenantId,
            oldValues,
            new { user.FirstName, user.LastName, user.ContactInfo, user.Preferences },
            cancellationToken);

        _logger.LogInformation("Successfully updated user: {UserId}", user.Id);

        return _mapper.Map<UserDto>(user);
    }
}

/// <summary>
/// Assign user role command
/// </summary>
public record AssignUserRoleCommand(
    Guid UserId,
    Guid RoleId
) : IRequest<UserDto>;

public class AssignUserRoleCommandValidator : AbstractValidator<AssignUserRoleCommand>
{
    public AssignUserRoleCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage("Role ID is required");
    }
}

public class AssignUserRoleCommandHandler : IRequestHandler<AssignUserRoleCommand, UserDto>
{
    private readonly IUserService _userService;
    private readonly IUserRepository _userRepository;
    private readonly IAuditService _auditService;
    private readonly IMapper _mapper;
    private readonly ILogger<AssignUserRoleCommandHandler> _logger;

    public AssignUserRoleCommandHandler(
        IUserService userService,
        IUserRepository userRepository,
        IAuditService auditService,
        IMapper mapper,
        ILogger<AssignUserRoleCommandHandler> logger)
    {
        _userService = userService;
        _userRepository = userRepository;
        _auditService = auditService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserDto> Handle(AssignUserRoleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning role {RoleId} to user {UserId}", 
            request.RoleId, request.UserId);

        var user = await _userService.AssignRoleAsync(
            request.UserId,
            request.RoleId,
            cancellationToken);

        // Log audit trail
        await _auditService.LogAsync(
            "UserRoleAssigned",
            nameof(User),
            user.Id,
            user.Id,
            user.TenantId,
            null,
            new { RoleId = request.RoleId },
            cancellationToken);

        _logger.LogInformation("Successfully assigned role to user: {UserId}", user.Id);

        return _mapper.Map<UserDto>(user);
    }
}

/// <summary>
/// Remove user role command
/// </summary>
public record RemoveUserRoleCommand(
    Guid UserId,
    Guid RoleId
) : IRequest<UserDto>;

public class RemoveUserRoleCommandValidator : AbstractValidator<RemoveUserRoleCommand>
{
    public RemoveUserRoleCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage("Role ID is required");
    }
}

public class RemoveUserRoleCommandHandler : IRequestHandler<RemoveUserRoleCommand, UserDto>
{
    private readonly IUserService _userService;
    private readonly IAuditService _auditService;
    private readonly IMapper _mapper;
    private readonly ILogger<RemoveUserRoleCommandHandler> _logger;

    public RemoveUserRoleCommandHandler(
        IUserService userService,
        IAuditService auditService,
        IMapper mapper,
        ILogger<RemoveUserRoleCommandHandler> logger)
    {
        _userService = userService;
        _auditService = auditService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserDto> Handle(RemoveUserRoleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing role {RoleId} from user {UserId}", 
            request.RoleId, request.UserId);

        var user = await _userService.RemoveRoleAsync(
            request.UserId,
            request.RoleId,
            cancellationToken);

        // Log audit trail
        await _auditService.LogAsync(
            "UserRoleRemoved",
            nameof(User),
            user.Id,
            user.Id,
            user.TenantId,
            null,
            new { RoleId = request.RoleId },
            cancellationToken);

        _logger.LogInformation("Successfully removed role from user: {UserId}", user.Id);

        return _mapper.Map<UserDto>(user);
    }
}

/// <summary>
/// Activate user command
/// </summary>
public record ActivateUserCommand(Guid UserId) : IRequest<UserDto>;

public class ActivateUserCommandValidator : AbstractValidator<ActivateUserCommand>
{
    public ActivateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}

public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, UserDto>
{
    private readonly IUserService _userService;
    private readonly IAuditService _auditService;
    private readonly IMapper _mapper;
    private readonly ILogger<ActivateUserCommandHandler> _logger;

    public ActivateUserCommandHandler(
        IUserService userService,
        IAuditService auditService,
        IMapper mapper,
        ILogger<ActivateUserCommandHandler> logger)
    {
        _userService = userService;
        _auditService = auditService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserDto> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Activating user: {UserId}", request.UserId);

        var user = await _userService.ActivateUserAsync(request.UserId, cancellationToken);

        // Log audit trail
        await _auditService.LogAsync(
            "UserActivated",
            nameof(User),
            user.Id,
            user.Id,
            user.TenantId,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Successfully activated user: {UserId}", user.Id);

        return _mapper.Map<UserDto>(user);
    }
}

/// <summary>
/// Deactivate user command
/// </summary>
public record DeactivateUserCommand(
    Guid UserId,
    string Reason
) : IRequest<UserDto>;

public class DeactivateUserCommandValidator : AbstractValidator<DeactivateUserCommand>
{
    public DeactivateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(500)
            .WithMessage("Deactivation reason is required and must not exceed 500 characters");
    }
}

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, UserDto>
{
    private readonly IUserService _userService;
    private readonly IAuditService _auditService;
    private readonly IMapper _mapper;
    private readonly ILogger<DeactivateUserCommandHandler> _logger;

    public DeactivateUserCommandHandler(
        IUserService userService,
        IAuditService auditService,
        IMapper mapper,
        ILogger<DeactivateUserCommandHandler> logger)
    {
        _userService = userService;
        _auditService = auditService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserDto> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deactivating user: {UserId}", request.UserId);

        var user = await _userService.DeactivateUserAsync(
            request.UserId,
            request.Reason,
            cancellationToken);

        // Log audit trail
        await _auditService.LogAsync(
            "UserDeactivated",
            nameof(User),
            user.Id,
            user.Id,
            user.TenantId,
            null,
            new { Reason = request.Reason },
            cancellationToken);

        _logger.LogInformation("Successfully deactivated user: {UserId}", user.Id);

        return _mapper.Map<UserDto>(user);
    }
}

/// <summary>
/// Invite user command
/// </summary>
public record InviteUserCommand(
    string Email,
    string FirstName,
    string LastName,
    Guid TenantId,
    List<Guid> RoleIds,
    Guid InvitedByUserId
) : IRequest<UserInvitationDto>;

public class InviteUserCommandValidator : AbstractValidator<InviteUserCommand>
{
    public InviteUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Valid email address is required");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("First name is required");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Last name is required");

        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("Tenant ID is required");

        RuleFor(x => x.RoleIds)
            .NotEmpty()
            .WithMessage("At least one role is required");

        RuleFor(x => x.InvitedByUserId)
            .NotEmpty()
            .WithMessage("Inviting user ID is required");
    }
}

public class InviteUserCommandHandler : IRequestHandler<InviteUserCommand, UserInvitationDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly INotificationService _notificationService;
    private readonly IAuditService _auditService;
    private readonly IMapper _mapper;
    private readonly ILogger<InviteUserCommandHandler> _logger;

    public InviteUserCommandHandler(
        IUserRepository userRepository,
        ITenantRepository tenantRepository,
        INotificationService notificationService,
        IAuditService auditService,
        IMapper mapper,
        ILogger<InviteUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
        _notificationService = notificationService;
        _auditService = auditService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserInvitationDto> Handle(InviteUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Inviting user: {Email} to tenant: {TenantId}", 
            request.Email, request.TenantId);

        // Validate tenant exists
        var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken)
            ?? throw new NotFoundException($"Tenant with ID {request.TenantId} not found");

        // Validate inviting user exists
        var invitingUser = await _userRepository.GetByIdAsync(request.InvitedByUserId, cancellationToken)
            ?? throw new NotFoundException($"Inviting user with ID {request.InvitedByUserId} not found");

        // Check if user already exists
        if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            throw new ValidationException($"User with email '{request.Email}' already exists");
        }

        // Create invitation (simplified - would need UserInvitation entity)
        var invitation = new UserInvitationDto
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            TenantId = request.TenantId,
            TenantName = tenant.Name,
            InvitedByUserId = request.InvitedByUserId,
            InvitedByUserName = $"{invitingUser.FirstName} {invitingUser.LastName}",
            InvitedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Status = InvitationStatus.Pending,
            StatusName = "Pending",
            RoleIds = request.RoleIds,
            InvitationToken = Guid.NewGuid().ToString(),
            IsExpired = false,
            CanResend = true,
            ResendCount = 0
        };

        // Log audit trail
        await _auditService.LogAsync(
            "UserInvited",
            "UserInvitation",
            invitation.Id,
            request.InvitedByUserId,
            request.TenantId,
            null,
            new { invitation.Email, invitation.FirstName, invitation.LastName },
            cancellationToken);

        _logger.LogInformation("Successfully created invitation for: {Email}", request.Email);

        return invitation;
    }
}
