namespace TossErp.Setup.Application.Queries;

// ============================================================================
// SYSTEM CONFIGURATION QUERIES
// ============================================================================

/// <summary>
/// Get system configurations query
/// </summary>
public record GetSystemConfigsQuery(
    Guid? TenantId = null,
    string? Category = null,
    bool GlobalOnly = false
) : IRequest<IEnumerable<SystemConfigDto>>;

public class GetSystemConfigsQueryHandler : IRequestHandler<GetSystemConfigsQuery, IEnumerable<SystemConfigDto>>
{
    private readonly ISystemConfigRepository _configRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetSystemConfigsQueryHandler> _logger;

    public GetSystemConfigsQueryHandler(
        ISystemConfigRepository configRepository,
        IMapper mapper,
        ILogger<GetSystemConfigsQueryHandler> logger)
    {
        _configRepository = configRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<SystemConfigDto>> Handle(GetSystemConfigsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting system configs: TenantId={TenantId}, Category={Category}", 
            request.TenantId, request.Category);

        IEnumerable<SystemConfig> configs;

        if (request.GlobalOnly)
        {
            configs = await _configRepository.GetGlobalConfigsAsync(cancellationToken);
        }
        else if (request.TenantId.HasValue)
        {
            configs = await _configRepository.GetByTenantAsync(request.TenantId.Value, cancellationToken);
        }
        else if (!string.IsNullOrEmpty(request.Category))
        {
            configs = await _configRepository.GetByCategoryAsync(request.Category, cancellationToken);
        }
        else
        {
            configs = await _configRepository.GetAllAsync(cancellationToken);
        }

        if (!string.IsNullOrEmpty(request.Category) && !request.GlobalOnly && !request.TenantId.HasValue)
        {
            configs = configs.Where(c => c.Category == request.Category);
        }

        return _mapper.Map<IEnumerable<SystemConfigDto>>(configs.OrderBy(c => c.Category).ThenBy(c => c.Key));
    }
}

/// <summary>
/// Get system configuration by key query
/// </summary>
public record GetSystemConfigByKeyQuery(
    string Key,
    Guid? TenantId = null
) : IRequest<SystemConfigDto>;

public class GetSystemConfigByKeyQueryHandler : IRequestHandler<GetSystemConfigByKeyQuery, SystemConfigDto>
{
    private readonly ISystemConfigRepository _configRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetSystemConfigByKeyQueryHandler> _logger;

    public GetSystemConfigByKeyQueryHandler(
        ISystemConfigRepository configRepository,
        IMapper mapper,
        ILogger<GetSystemConfigByKeyQueryHandler> logger)
    {
        _configRepository = configRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<SystemConfigDto> Handle(GetSystemConfigByKeyQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting system config by key: {Key}, TenantId: {TenantId}", 
            request.Key, request.TenantId);

        var config = await _configRepository.GetByKeyAndTenantAsync(request.Key, request.TenantId, cancellationToken)
            ?? throw new NotFoundException($"System configuration with key '{request.Key}' not found");

        return _mapper.Map<SystemConfigDto>(config);
    }
}

/// <summary>
/// Get configuration value query
/// </summary>
public record GetConfigValueQuery(
    string Key,
    Guid? TenantId = null
) : IRequest<string?>;

public class GetConfigValueQueryHandler : IRequestHandler<GetConfigValueQuery, string?>
{
    private readonly ISystemConfigService _configService;
    private readonly ILogger<GetConfigValueQueryHandler> _logger;

    public GetConfigValueQueryHandler(
        ISystemConfigService configService,
        ILogger<GetConfigValueQueryHandler> logger)
    {
        _configService = configService;
        _logger = logger;
    }

    public async Task<string?> Handle(GetConfigValueQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting config value for key: {Key}, TenantId: {TenantId}", 
            request.Key, request.TenantId);

        return await _configService.GetConfigValueAsync(request.Key, request.TenantId, cancellationToken);
    }
}

/// <summary>
/// Get configurations by category query
/// </summary>
public record GetConfigsByCategoryQuery(
    string Category,
    Guid? TenantId = null
) : IRequest<Dictionary<string, string>>;

public class GetConfigsByCategoryQueryHandler : IRequestHandler<GetConfigsByCategoryQuery, Dictionary<string, string>>
{
    private readonly ISystemConfigService _configService;
    private readonly ILogger<GetConfigsByCategoryQueryHandler> _logger;

    public GetConfigsByCategoryQueryHandler(
        ISystemConfigService configService,
        ILogger<GetConfigsByCategoryQueryHandler> logger)
    {
        _configService = configService;
        _logger = logger;
    }

    public async Task<Dictionary<string, string>> Handle(GetConfigsByCategoryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting configs by category: {Category}, TenantId: {TenantId}", 
            request.Category, request.TenantId);

        return await _configService.GetConfigByCategoryAsync(request.Category, request.TenantId, cancellationToken);
    }
}

// ============================================================================
// ROLE AND PERMISSION QUERIES
// ============================================================================

/// <summary>
/// Get all roles query
/// </summary>
public record GetRolesQuery(
    Guid? TenantId = null,
    RoleType? Type = null,
    bool SystemRolesOnly = false
) : IRequest<IEnumerable<UserRoleDto>>;

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, IEnumerable<UserRoleDto>>
{
    private readonly IUserRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetRolesQueryHandler> _logger;

    public GetRolesQueryHandler(
        IUserRoleRepository roleRepository,
        IMapper mapper,
        ILogger<GetRolesQueryHandler> logger)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<UserRoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting roles: TenantId={TenantId}, Type={Type}", 
            request.TenantId, request.Type);

        IEnumerable<UserRole> roles;

        if (request.SystemRolesOnly)
        {
            roles = await _roleRepository.GetSystemRolesAsync(cancellationToken);
        }
        else if (request.TenantId.HasValue)
        {
            roles = await _roleRepository.GetByTenantAsync(request.TenantId.Value, cancellationToken);
        }
        else
        {
            roles = await _roleRepository.GetAllAsync(cancellationToken);
        }

        if (request.Type.HasValue)
        {
            roles = roles.Where(r => r.Type == request.Type.Value);
        }

        return _mapper.Map<IEnumerable<UserRoleDto>>(roles.OrderBy(r => r.Name));
    }
}

/// <summary>
/// Get role by ID query
/// </summary>
public record GetRoleByIdQuery(Guid RoleId) : IRequest<UserRoleDto>;

public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, UserRoleDto>
{
    private readonly IUserRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetRoleByIdQueryHandler> _logger;

    public GetRoleByIdQueryHandler(
        IUserRoleRepository roleRepository,
        IMapper mapper,
        ILogger<GetRoleByIdQueryHandler> logger)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserRoleDto> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting role by ID: {RoleId}", request.RoleId);

        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken)
            ?? throw new NotFoundException($"Role with ID {request.RoleId} not found");

        return _mapper.Map<UserRoleDto>(role);
    }
}

/// <summary>
/// Get all permissions query
/// </summary>
public record GetPermissionsQuery(
    string? Module = null,
    Guid? RoleId = null
) : IRequest<IEnumerable<PermissionDto>>;

public class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, IEnumerable<PermissionDto>>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetPermissionsQueryHandler> _logger;

    public GetPermissionsQueryHandler(
        IPermissionRepository permissionRepository,
        IMapper mapper,
        ILogger<GetPermissionsQueryHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<PermissionDto>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting permissions: Module={Module}, RoleId={RoleId}", 
            request.Module, request.RoleId);

        IEnumerable<Permission> permissions;

        if (request.RoleId.HasValue)
        {
            permissions = await _permissionRepository.GetByRoleAsync(request.RoleId.Value, cancellationToken);
        }
        else if (!string.IsNullOrEmpty(request.Module))
        {
            permissions = await _permissionRepository.GetByModuleAsync(request.Module, cancellationToken);
        }
        else
        {
            permissions = await _permissionRepository.GetAllAsync(cancellationToken);
        }

        return _mapper.Map<IEnumerable<PermissionDto>>(permissions.OrderBy(p => p.Module).ThenBy(p => p.Name));
    }
}

/// <summary>
/// Get permission by ID query
/// </summary>
public record GetPermissionByIdQuery(Guid PermissionId) : IRequest<PermissionDto>;

public class GetPermissionByIdQueryHandler : IRequestHandler<GetPermissionByIdQuery, PermissionDto>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetPermissionByIdQueryHandler> _logger;

    public GetPermissionByIdQueryHandler(
        IPermissionRepository permissionRepository,
        IMapper mapper,
        ILogger<GetPermissionByIdQueryHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PermissionDto> Handle(GetPermissionByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting permission by ID: {PermissionId}", request.PermissionId);

        var permission = await _permissionRepository.GetByIdAsync(request.PermissionId, cancellationToken)
            ?? throw new NotFoundException($"Permission with ID {request.PermissionId} not found");

        return _mapper.Map<PermissionDto>(permission);
    }
}

/// <summary>
/// Get permissions by module query
/// </summary>
public record GetPermissionsByModuleQuery(string Module) : IRequest<IEnumerable<PermissionDto>>;

public class GetPermissionsByModuleQueryHandler : IRequestHandler<GetPermissionsByModuleQuery, IEnumerable<PermissionDto>>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetPermissionsByModuleQueryHandler> _logger;

    public GetPermissionsByModuleQueryHandler(
        IPermissionRepository permissionRepository,
        IMapper mapper,
        ILogger<GetPermissionsByModuleQueryHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<PermissionDto>> Handle(GetPermissionsByModuleQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting permissions by module: {Module}", request.Module);

        var permissions = await _permissionRepository.GetByModuleAsync(request.Module, cancellationToken);
        return _mapper.Map<IEnumerable<PermissionDto>>(permissions.OrderBy(p => p.Name));
    }
}
