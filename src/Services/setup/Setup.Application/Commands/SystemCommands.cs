namespace TossErp.Setup.Application.Commands;

// ============================================================================
// SYSTEM CONFIGURATION COMMANDS
// ============================================================================

/// <summary>
/// Set system configuration command
/// </summary>
public record SetSystemConfigCommand(
    string Key,
    string Value,
    string? Description = null,
    string? Category = null,
    Guid? TenantId = null
) : IRequest<SystemConfigDto>;

public class SetSystemConfigCommandValidator : AbstractValidator<SetSystemConfigCommand>
{
    public SetSystemConfigCommandValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty()
            .MaximumLength(100)
            .Matches(@"^[a-zA-Z0-9_.-]+$")
            .WithMessage("Key is required and must contain only letters, numbers, dots, hyphens, and underscores");

        RuleFor(x => x.Value)
            .NotEmpty()
            .MaximumLength(2000)
            .WithMessage("Value is required and must not exceed 2000 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters");

        RuleFor(x => x.Category)
            .MaximumLength(50)
            .WithMessage("Category must not exceed 50 characters");
    }
}

public class SetSystemConfigCommandHandler : IRequestHandler<SetSystemConfigCommand, SystemConfigDto>
{
    private readonly ISystemConfigRepository _configRepository;
    private readonly ISystemConfigService _configService;
    private readonly IAuditService _auditService;
    private readonly IMapper _mapper;
    private readonly ILogger<SetSystemConfigCommandHandler> _logger;

    public SetSystemConfigCommandHandler(
        ISystemConfigRepository configRepository,
        ISystemConfigService configService,
        IAuditService auditService,
        IMapper mapper,
        ILogger<SetSystemConfigCommandHandler> logger)
    {
        _configRepository = configRepository;
        _configService = configService;
        _auditService = auditService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<SystemConfigDto> Handle(SetSystemConfigCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Setting system config: {Key} for tenant: {TenantId}", 
            request.Key, request.TenantId);

        // Check if config already exists
        var existingConfig = await _configRepository.GetByKeyAndTenantAsync(
            request.Key, 
            request.TenantId, 
            cancellationToken);

        SystemConfig config;
        string action;

        if (existingConfig != null)
        {
            var oldValue = existingConfig.Value;
            existingConfig.UpdateValue(request.Value);
            if (!string.IsNullOrEmpty(request.Description))
            {
                existingConfig.UpdateDescription(request.Description);
            }
            if (!string.IsNullOrEmpty(request.Category))
            {
                existingConfig.UpdateCategory(request.Category);
            }

            config = await _configRepository.UpdateAsync(existingConfig, cancellationToken);
            action = "SystemConfigUpdated";

            // Log audit trail for update
            await _auditService.LogAsync(
                action,
                nameof(SystemConfig),
                config.Id,
                Guid.Empty, // System action
                request.TenantId ?? Guid.Empty,
                new { Key = request.Key, Value = oldValue },
                new { Key = request.Key, Value = request.Value },
                cancellationToken);
        }
        else
        {
            config = SystemConfig.Create(
                request.Key,
                request.Value,
                request.Description,
                request.Category,
                request.TenantId);

            config = await _configRepository.AddAsync(config, cancellationToken);
            action = "SystemConfigCreated";

            // Log audit trail for creation
            await _auditService.LogAsync(
                action,
                nameof(SystemConfig),
                config.Id,
                Guid.Empty, // System action
                request.TenantId ?? Guid.Empty,
                null,
                new { Key = request.Key, Value = request.Value },
                cancellationToken);
        }

        _logger.LogInformation("Successfully set system config: {Key}", request.Key);

        return _mapper.Map<SystemConfigDto>(config);
    }
}

/// <summary>
/// Delete system configuration command
/// </summary>
public record DeleteSystemConfigCommand(
    string Key,
    Guid? TenantId = null
) : IRequest<bool>;

public class DeleteSystemConfigCommandValidator : AbstractValidator<DeleteSystemConfigCommand>
{
    public DeleteSystemConfigCommandValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty()
            .WithMessage("Key is required");
    }
}

public class DeleteSystemConfigCommandHandler : IRequestHandler<DeleteSystemConfigCommand, bool>
{
    private readonly ISystemConfigRepository _configRepository;
    private readonly IAuditService _auditService;
    private readonly ILogger<DeleteSystemConfigCommandHandler> _logger;

    public DeleteSystemConfigCommandHandler(
        ISystemConfigRepository configRepository,
        IAuditService auditService,
        ILogger<DeleteSystemConfigCommandHandler> logger)
    {
        _configRepository = configRepository;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteSystemConfigCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting system config: {Key} for tenant: {TenantId}", 
            request.Key, request.TenantId);

        var config = await _configRepository.GetByKeyAndTenantAsync(
            request.Key,
            request.TenantId,
            cancellationToken);

        if (config == null)
        {
            _logger.LogWarning("System config not found: {Key}", request.Key);
            return false;
        }

        await _configRepository.DeleteAsync(config.Id, cancellationToken);

        // Log audit trail
        await _auditService.LogAsync(
            "SystemConfigDeleted",
            nameof(SystemConfig),
            config.Id,
            Guid.Empty, // System action
            request.TenantId ?? Guid.Empty,
            new { Key = request.Key, Value = config.Value },
            null,
            cancellationToken);

        _logger.LogInformation("Successfully deleted system config: {Key}", request.Key);

        return true;
    }
}

// ============================================================================
// ROLE AND PERMISSION COMMANDS
// ============================================================================

/// <summary>
/// Create role command
/// </summary>
public record CreateRoleCommand(
    string Name,
    string Description,
    RoleType Type,
    Guid? TenantId = null,
    List<Guid>? PermissionIds = null
) : IRequest<UserRoleDto>;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(100)
            .WithMessage("Role name must be between 2 and 100 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(500)
            .WithMessage("Description is required and must not exceed 500 characters");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid role type");
    }
}

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, UserRoleDto>
{
    private readonly IUserRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IAuditService _auditService;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateRoleCommandHandler> _logger;

    public CreateRoleCommandHandler(
        IUserRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IAuditService auditService,
        IMapper mapper,
        ILogger<CreateRoleCommandHandler> logger)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _auditService = auditService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserRoleDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating role: {RoleName} for tenant: {TenantId}", 
            request.Name, request.TenantId);

        // Validate role name uniqueness
        if (await _roleRepository.ExistsByNameAsync(request.Name, cancellationToken))
        {
            throw new ValidationException($"Role with name '{request.Name}' already exists");
        }

        // Create role
        var role = UserRole.Create(
            request.Name,
            request.Description,
            request.Type,
            request.TenantId);

        role = await _roleRepository.AddAsync(role, cancellationToken);

        // Assign permissions if provided
        if (request.PermissionIds != null && request.PermissionIds.Any())
        {
            foreach (var permissionId in request.PermissionIds)
            {
                var permission = await _permissionRepository.GetByIdAsync(permissionId, cancellationToken);
                if (permission != null)
                {
                    role.AddPermission(permission);
                }
            }
            role = await _roleRepository.UpdateAsync(role, cancellationToken);
        }

        // Log audit trail
        await _auditService.LogAsync(
            "RoleCreated",
            nameof(UserRole),
            role.Id,
            Guid.Empty, // System action
            request.TenantId ?? Guid.Empty,
            null,
            new { role.Name, role.Description, role.Type },
            cancellationToken);

        _logger.LogInformation("Successfully created role: {RoleId}", role.Id);

        return _mapper.Map<UserRoleDto>(role);
    }
}

/// <summary>
/// Create permission command
/// </summary>
public record CreatePermissionCommand(
    string Name,
    string Description,
    string Module,
    string Action,
    string Resource
) : IRequest<PermissionDto>;

public class CreatePermissionCommandValidator : AbstractValidator<CreatePermissionCommand>
{
    public CreatePermissionCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Permission name is required and must not exceed 100 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(500)
            .WithMessage("Description is required and must not exceed 500 characters");

        RuleFor(x => x.Module)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Module is required and must not exceed 50 characters");

        RuleFor(x => x.Action)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Action is required and must not exceed 50 characters");

        RuleFor(x => x.Resource)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Resource is required and must not exceed 50 characters");
    }
}

public class CreatePermissionCommandHandler : IRequestHandler<CreatePermissionCommand, PermissionDto>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IAuditService _auditService;
    private readonly IMapper _mapper;
    private readonly ILogger<CreatePermissionCommandHandler> _logger;

    public CreatePermissionCommandHandler(
        IPermissionRepository permissionRepository,
        IAuditService auditService,
        IMapper mapper,
        ILogger<CreatePermissionCommandHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _auditService = auditService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PermissionDto> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating permission: {PermissionName}", request.Name);

        // Validate permission name uniqueness
        if (await _permissionRepository.ExistsByNameAsync(request.Name, cancellationToken))
        {
            throw new ValidationException($"Permission with name '{request.Name}' already exists");
        }

        // Create permission
        var permission = Permission.Create(
            request.Name,
            request.Description,
            request.Module,
            request.Action,
            request.Resource);

        permission = await _permissionRepository.AddAsync(permission, cancellationToken);

        // Log audit trail
        await _auditService.LogSystemActionAsync(
            "PermissionCreated",
            $"Created permission: {permission.Name} for module: {permission.Module}",
            cancellationToken);

        _logger.LogInformation("Successfully created permission: {PermissionId}", permission.Id);

        return _mapper.Map<PermissionDto>(permission);
    }
}

/// <summary>
/// Assign permission to role command
/// </summary>
public record AssignPermissionToRoleCommand(
    Guid RoleId,
    Guid PermissionId
) : IRequest<UserRoleDto>;

public class AssignPermissionToRoleCommandValidator : AbstractValidator<AssignPermissionToRoleCommand>
{
    public AssignPermissionToRoleCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage("Role ID is required");

        RuleFor(x => x.PermissionId)
            .NotEmpty()
            .WithMessage("Permission ID is required");
    }
}

public class AssignPermissionToRoleCommandHandler : IRequestHandler<AssignPermissionToRoleCommand, UserRoleDto>
{
    private readonly IUserRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IAuditService _auditService;
    private readonly IMapper _mapper;
    private readonly ILogger<AssignPermissionToRoleCommandHandler> _logger;

    public AssignPermissionToRoleCommandHandler(
        IUserRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IAuditService auditService,
        IMapper mapper,
        ILogger<AssignPermissionToRoleCommandHandler> logger)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _auditService = auditService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserRoleDto> Handle(AssignPermissionToRoleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning permission {PermissionId} to role {RoleId}", 
            request.PermissionId, request.RoleId);

        // Get role
        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken)
            ?? throw new NotFoundException($"Role with ID {request.RoleId} not found");

        // Get permission
        var permission = await _permissionRepository.GetByIdAsync(request.PermissionId, cancellationToken)
            ?? throw new NotFoundException($"Permission with ID {request.PermissionId} not found");

        // Assign permission to role
        role.AddPermission(permission);
        role = await _roleRepository.UpdateAsync(role, cancellationToken);

        // Log audit trail
        await _auditService.LogAsync(
            "PermissionAssignedToRole",
            nameof(UserRole),
            role.Id,
            Guid.Empty, // System action
            role.TenantId ?? Guid.Empty,
            null,
            new { PermissionId = request.PermissionId, PermissionName = permission.Name },
            cancellationToken);

        _logger.LogInformation("Successfully assigned permission to role: {RoleId}", role.Id);

        return _mapper.Map<UserRoleDto>(role);
    }
}
