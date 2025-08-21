namespace TossErp.Setup.Application.Queries;

// ============================================================================
// USER QUERIES
// ============================================================================

/// <summary>
/// Get users query with filtering and pagination
/// </summary>
public record GetUsersQuery(
    Guid? TenantId = null,
    UserStatus? Status = null,
    Guid? RoleId = null,
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string? SortBy = null,
    bool SortDescending = false
) : IRequest<PagedResult<UserSummaryDto>>;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<UserSummaryDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUsersQueryHandler> _logger;

    public GetUsersQueryHandler(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<GetUsersQueryHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResult<UserSummaryDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting users with filters: TenantId={TenantId}, Status={Status}", 
            request.TenantId, request.Status);

        IEnumerable<User> users;

        if (request.TenantId.HasValue)
        {
            users = await _userRepository.GetByTenantAsync(request.TenantId.Value, cancellationToken);
        }
        else
        {
            // For system-wide queries (admin only)
            users = new List<User>(); // Would need a GetAllUsers method
        }

        // Apply filters
        if (request.Status.HasValue)
        {
            users = users.Where(u => u.Status == request.Status.Value);
        }

        if (request.RoleId.HasValue)
        {
            users = users.Where(u => u.Roles.Any(r => r.Id == request.RoleId.Value));
        }

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLowerInvariant();
            users = users.Where(u => 
                u.FirstName.ToLowerInvariant().Contains(searchTerm) ||
                u.LastName.ToLowerInvariant().Contains(searchTerm) ||
                u.Email.ToLowerInvariant().Contains(searchTerm) ||
                u.Username.ToLowerInvariant().Contains(searchTerm));
        }

        // Apply sorting
        users = request.SortBy?.ToLowerInvariant() switch
        {
            "firstname" => request.SortDescending ? users.OrderByDescending(u => u.FirstName) : users.OrderBy(u => u.FirstName),
            "lastname" => request.SortDescending ? users.OrderByDescending(u => u.LastName) : users.OrderBy(u => u.LastName),
            "email" => request.SortDescending ? users.OrderByDescending(u => u.Email) : users.OrderBy(u => u.Email),
            "createdat" => request.SortDescending ? users.OrderByDescending(u => u.CreatedAt) : users.OrderBy(u => u.CreatedAt),
            "lastloginat" => request.SortDescending ? users.OrderByDescending(u => u.LastLoginAt) : users.OrderBy(u => u.LastLoginAt),
            "status" => request.SortDescending ? users.OrderByDescending(u => u.Status) : users.OrderBy(u => u.Status),
            _ => users.OrderByDescending(u => u.CreatedAt)
        };

        // Apply pagination
        var totalCount = users.Count();
        var pagedUsers = users
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var userDtos = _mapper.Map<List<UserSummaryDto>>(pagedUsers);

        return new PagedResult<UserSummaryDto>
        {
            Items = userDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }
}

/// <summary>
/// Get user by ID query
/// </summary>
public record GetUserByIdQuery(Guid UserId) : IRequest<UserDto>;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;

    public GetUserByIdQueryHandler(
        IUserRepository userRepository,
        ITenantRepository tenantRepository,
        IUserService userService,
        IMapper mapper,
        ILogger<GetUserByIdQueryHandler> logger)
    {
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
        _userService = userService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting user by ID: {UserId}", request.UserId);

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException($"User with ID {request.UserId} not found");

        var userDto = _mapper.Map<UserDto>(user);

        // Get tenant information
        var tenant = await _tenantRepository.GetByIdAsync(user.TenantId, cancellationToken);
        if (tenant != null)
        {
            userDto.TenantName = tenant.Name;
        }

        // Get user permissions
        var permissions = await _userService.GetUserPermissionsAsync(user.Id, cancellationToken);
        userDto.Permissions = _mapper.Map<ICollection<PermissionDto>>(permissions);

        return userDto;
    }
}

/// <summary>
/// Get user by email query
/// </summary>
public record GetUserByEmailQuery(string Email) : IRequest<UserDto>;

public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUserByEmailQueryHandler> _logger;

    public GetUserByEmailQueryHandler(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<GetUserByEmailQueryHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserDto> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting user by email: {Email}", request.Email);

        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken)
            ?? throw new NotFoundException($"User with email '{request.Email}' not found");

        return _mapper.Map<UserDto>(user);
    }
}

/// <summary>
/// Get users by tenant query
/// </summary>
public record GetUsersByTenantQuery(
    Guid TenantId,
    UserStatus? Status = null,
    bool ActiveOnly = false
) : IRequest<IEnumerable<UserSummaryDto>>;

public class GetUsersByTenantQueryHandler : IRequestHandler<GetUsersByTenantQuery, IEnumerable<UserSummaryDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUsersByTenantQueryHandler> _logger;

    public GetUsersByTenantQueryHandler(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<GetUsersByTenantQueryHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<UserSummaryDto>> Handle(GetUsersByTenantQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting users for tenant: {TenantId}", request.TenantId);

        IEnumerable<User> users;

        if (request.ActiveOnly)
        {
            users = await _userRepository.GetActiveUsersAsync(request.TenantId, cancellationToken);
        }
        else
        {
            users = await _userRepository.GetByTenantAsync(request.TenantId, cancellationToken);
        }

        if (request.Status.HasValue)
        {
            users = users.Where(u => u.Status == request.Status.Value);
        }

        return _mapper.Map<IEnumerable<UserSummaryDto>>(users.OrderBy(u => u.FirstName).ThenBy(u => u.LastName));
    }
}

/// <summary>
/// Get users by role query
/// </summary>
public record GetUsersByRoleQuery(Guid RoleId) : IRequest<IEnumerable<UserSummaryDto>>;

public class GetUsersByRoleQueryHandler : IRequestHandler<GetUsersByRoleQuery, IEnumerable<UserSummaryDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUsersByRoleQueryHandler> _logger;

    public GetUsersByRoleQueryHandler(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<GetUsersByRoleQueryHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<UserSummaryDto>> Handle(GetUsersByRoleQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting users by role: {RoleId}", request.RoleId);

        // This would need to be implemented in the repository
        var users = new List<User>(); // await _userRepository.GetByRoleAsync(request.RoleId, cancellationToken);

        return _mapper.Map<IEnumerable<UserSummaryDto>>(users);
    }
}

/// <summary>
/// Get inactive users query
/// </summary>
public record GetInactiveUsersQuery(
    Guid TenantId,
    int DaysInactive = 30
) : IRequest<IEnumerable<UserSummaryDto>>;

public class GetInactiveUsersQueryHandler : IRequestHandler<GetInactiveUsersQuery, IEnumerable<UserSummaryDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetInactiveUsersQueryHandler> _logger;

    public GetInactiveUsersQueryHandler(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<GetInactiveUsersQueryHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<UserSummaryDto>> Handle(GetInactiveUsersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting inactive users for tenant: {TenantId}, DaysInactive: {Days}", 
            request.TenantId, request.DaysInactive);

        var users = await _userRepository.GetByTenantAsync(request.TenantId, cancellationToken);
        var cutoffDate = DateTime.UtcNow.AddDays(-request.DaysInactive);

        var inactiveUsers = users.Where(u => 
            u.Status == UserStatus.Active && 
            (u.LastLoginAt == null || u.LastLoginAt < cutoffDate));

        return _mapper.Map<IEnumerable<UserSummaryDto>>(inactiveUsers.OrderBy(u => u.LastLoginAt));
    }
}

/// <summary>
/// Get user permissions query
/// </summary>
public record GetUserPermissionsQuery(Guid UserId) : IRequest<IEnumerable<PermissionDto>>;

public class GetUserPermissionsQueryHandler : IRequestHandler<GetUserPermissionsQuery, IEnumerable<PermissionDto>>
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUserPermissionsQueryHandler> _logger;

    public GetUserPermissionsQueryHandler(
        IUserService userService,
        IMapper mapper,
        ILogger<GetUserPermissionsQueryHandler> logger)
    {
        _userService = userService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<PermissionDto>> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting permissions for user: {UserId}", request.UserId);

        var permissions = await _userService.GetUserPermissionsAsync(request.UserId, cancellationToken);
        return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
    }
}

/// <summary>
/// Validate user permission query
/// </summary>
public record ValidateUserPermissionQuery(
    Guid UserId,
    string Permission
) : IRequest<bool>;

public class ValidateUserPermissionQueryHandler : IRequestHandler<ValidateUserPermissionQuery, bool>
{
    private readonly IUserService _userService;
    private readonly ILogger<ValidateUserPermissionQueryHandler> _logger;

    public ValidateUserPermissionQueryHandler(
        IUserService userService,
        ILogger<ValidateUserPermissionQueryHandler> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task<bool> Handle(ValidateUserPermissionQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Validating permission {Permission} for user: {UserId}", 
            request.Permission, request.UserId);

        return await _userService.ValidateUserPermissionAsync(
            request.UserId,
            request.Permission,
            cancellationToken);
    }
}
