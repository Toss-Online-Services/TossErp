namespace TossErp.Setup.Application.Queries;

// ============================================================================
// TENANT QUERIES
// ============================================================================

/// <summary>
/// Get all tenants query
/// </summary>
public record GetTenantsQuery(
    TenantStatus? Status = null,
    SubscriptionPlan? Plan = null,
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string? SortBy = null,
    bool SortDescending = false
) : IRequest<PagedResult<TenantSummaryDto>>;

public class GetTenantsQueryHandler : IRequestHandler<GetTenantsQuery, PagedResult<TenantSummaryDto>>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTenantsQueryHandler> _logger;

    public GetTenantsQueryHandler(
        ITenantRepository tenantRepository,
        IMapper mapper,
        ILogger<GetTenantsQueryHandler> logger)
    {
        _tenantRepository = tenantRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResult<TenantSummaryDto>> Handle(GetTenantsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting tenants with filters: Status={Status}, Plan={Plan}", 
            request.Status, request.Plan);

        var tenants = await _tenantRepository.GetAllAsync(cancellationToken);

        // Apply filters
        if (request.Status.HasValue)
        {
            tenants = tenants.Where(t => t.Status == request.Status.Value);
        }

        if (request.Plan.HasValue)
        {
            tenants = tenants.Where(t => t.SubscriptionPlan == request.Plan.Value);
        }

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLowerInvariant();
            tenants = tenants.Where(t => 
                t.Name.ToLowerInvariant().Contains(searchTerm) ||
                t.Domain.ToLowerInvariant().Contains(searchTerm));
        }

        // Apply sorting
        tenants = request.SortBy?.ToLowerInvariant() switch
        {
            "name" => request.SortDescending ? tenants.OrderByDescending(t => t.Name) : tenants.OrderBy(t => t.Name),
            "domain" => request.SortDescending ? tenants.OrderByDescending(t => t.Domain) : tenants.OrderBy(t => t.Domain),
            "createdat" => request.SortDescending ? tenants.OrderByDescending(t => t.CreatedAt) : tenants.OrderBy(t => t.CreatedAt),
            "status" => request.SortDescending ? tenants.OrderByDescending(t => t.Status) : tenants.OrderBy(t => t.Status),
            _ => tenants.OrderByDescending(t => t.CreatedAt)
        };

        // Apply pagination
        var totalCount = tenants.Count();
        var pagedTenants = tenants
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var tenantDtos = _mapper.Map<List<TenantSummaryDto>>(pagedTenants);

        return new PagedResult<TenantSummaryDto>
        {
            Items = tenantDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }
}

/// <summary>
/// Get tenant by ID query
/// </summary>
public record GetTenantByIdQuery(Guid TenantId) : IRequest<TenantDto>;

public class GetTenantByIdQueryHandler : IRequestHandler<GetTenantByIdQuery, TenantDto>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITenantService _tenantService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTenantByIdQueryHandler> _logger;

    public GetTenantByIdQueryHandler(
        ITenantRepository tenantRepository,
        IUserRepository userRepository,
        ITenantService tenantService,
        IMapper mapper,
        ILogger<GetTenantByIdQueryHandler> logger)
    {
        _tenantRepository = tenantRepository;
        _userRepository = userRepository;
        _tenantService = tenantService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<TenantDto> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting tenant by ID: {TenantId}", request.TenantId);

        var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken)
            ?? throw new NotFoundException($"Tenant with ID {request.TenantId} not found");

        var tenantDto = _mapper.Map<TenantDto>(tenant);

        // Get additional statistics
        var users = await _userRepository.GetByTenantAsync(tenant.Id, cancellationToken);
        tenantDto.UserCount = users.Count();
        tenantDto.ActiveUserCount = users.Count(u => u.Status == UserStatus.Active);

        // Get usage statistics
        var usageStats = await _tenantService.GetTenantUsageStatsAsync(tenant.Id, cancellationToken);
        tenantDto.StorageUsedBytes = usageStats.StorageUsedBytes;

        return tenantDto;
    }
}

/// <summary>
/// Get tenant by domain query
/// </summary>
public record GetTenantByDomainQuery(string Domain) : IRequest<TenantDto>;

public class GetTenantByDomainQueryHandler : IRequestHandler<GetTenantByDomainQuery, TenantDto>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTenantByDomainQueryHandler> _logger;

    public GetTenantByDomainQueryHandler(
        ITenantRepository tenantRepository,
        IMapper mapper,
        ILogger<GetTenantByDomainQueryHandler> logger)
    {
        _tenantRepository = tenantRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<TenantDto> Handle(GetTenantByDomainQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting tenant by domain: {Domain}", request.Domain);

        var tenant = await _tenantRepository.GetByDomainAsync(request.Domain, cancellationToken)
            ?? throw new NotFoundException($"Tenant with domain '{request.Domain}' not found");

        return _mapper.Map<TenantDto>(tenant);
    }
}

/// <summary>
/// Get active tenants query
/// </summary>
public record GetActiveTenantsQuery() : IRequest<IEnumerable<TenantSummaryDto>>;

public class GetActiveTenantsQueryHandler : IRequestHandler<GetActiveTenantsQuery, IEnumerable<TenantSummaryDto>>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetActiveTenantsQueryHandler> _logger;

    public GetActiveTenantsQueryHandler(
        ITenantRepository tenantRepository,
        IMapper mapper,
        ILogger<GetActiveTenantsQueryHandler> logger)
    {
        _tenantRepository = tenantRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<TenantSummaryDto>> Handle(GetActiveTenantsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting active tenants");

        var tenants = await _tenantRepository.GetActiveTenantsAsync(cancellationToken);
        return _mapper.Map<IEnumerable<TenantSummaryDto>>(tenants);
    }
}

/// <summary>
/// Get tenants by subscription plan query
/// </summary>
public record GetTenantsByPlanQuery(SubscriptionPlan Plan) : IRequest<IEnumerable<TenantSummaryDto>>;

public class GetTenantsByPlanQueryHandler : IRequestHandler<GetTenantsByPlanQuery, IEnumerable<TenantSummaryDto>>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTenantsByPlanQueryHandler> _logger;

    public GetTenantsByPlanQueryHandler(
        ITenantRepository tenantRepository,
        IMapper mapper,
        ILogger<GetTenantsByPlanQueryHandler> logger)
    {
        _tenantRepository = tenantRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<TenantSummaryDto>> Handle(GetTenantsByPlanQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting tenants by plan: {Plan}", request.Plan);

        var tenants = await _tenantRepository.GetTenantsByPlanAsync(request.Plan, cancellationToken);
        return _mapper.Map<IEnumerable<TenantSummaryDto>>(tenants);
    }
}

/// <summary>
/// Get tenant usage statistics query
/// </summary>
public record GetTenantUsageStatsQuery(Guid TenantId) : IRequest<TenantUsageStatsDto>;

public class GetTenantUsageStatsQueryHandler : IRequestHandler<GetTenantUsageStatsQuery, TenantUsageStatsDto>
{
    private readonly ITenantService _tenantService;
    private readonly ITenantRepository _tenantRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTenantUsageStatsQueryHandler> _logger;

    public GetTenantUsageStatsQueryHandler(
        ITenantService tenantService,
        ITenantRepository tenantRepository,
        IMapper mapper,
        ILogger<GetTenantUsageStatsQueryHandler> logger)
    {
        _tenantService = tenantService;
        _tenantRepository = tenantRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<TenantUsageStatsDto> Handle(GetTenantUsageStatsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting usage stats for tenant: {TenantId}", request.TenantId);

        var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken)
            ?? throw new NotFoundException($"Tenant with ID {request.TenantId} not found");

        var usageStats = await _tenantService.GetTenantUsageStatsAsync(request.TenantId, cancellationToken);
        
        var usageStatsDto = _mapper.Map<TenantUsageStatsDto>(usageStats);
        usageStatsDto.TenantName = tenant.Name;
        usageStatsDto.SubscriptionPlan = tenant.SubscriptionPlan;
        usageStatsDto.SubscriptionPlanName = tenant.SubscriptionPlan.ToString();

        // Calculate formatted storage
        usageStatsDto.StorageUsedFormatted = FormatBytes(usageStats.StorageUsedBytes);

        // Calculate utilization percentage (simplified)
        var maxUsers = tenant.SubscriptionPlan switch
        {
            SubscriptionPlan.Trial => 5,
            SubscriptionPlan.Basic => 25,
            SubscriptionPlan.Professional => 100,
            SubscriptionPlan.Enterprise => 1000,
            _ => 10
        };

        usageStatsDto.UtilizationPercentage = maxUsers > 0 ? 
            (decimal)usageStats.ActiveUserCount / maxUsers * 100 : 0;

        // Check limits
        usageStatsDto.IsOverLimit = usageStats.ActiveUserCount > maxUsers;
        if (usageStatsDto.IsOverLimit)
        {
            usageStatsDto.LimitExceeded.Add($"User limit exceeded: {usageStats.ActiveUserCount}/{maxUsers}");
        }

        return usageStatsDto;
    }

    private static string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}

/// <summary>
/// Get tenant dashboard query
/// </summary>
public record GetTenantDashboardQuery(Guid TenantId) : IRequest<TenantDashboardDto>;

public class GetTenantDashboardQueryHandler : IRequestHandler<GetTenantDashboardQuery, TenantDashboardDto>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAuditLogRepository _auditRepository;
    private readonly ISystemConfigRepository _configRepository;
    private readonly ITenantService _tenantService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTenantDashboardQueryHandler> _logger;

    public GetTenantDashboardQueryHandler(
        ITenantRepository tenantRepository,
        IUserRepository userRepository,
        IAuditLogRepository auditRepository,
        ISystemConfigRepository configRepository,
        ITenantService tenantService,
        IMapper mapper,
        ILogger<GetTenantDashboardQueryHandler> logger)
    {
        _tenantRepository = tenantRepository;
        _userRepository = userRepository;
        _auditRepository = auditRepository;
        _configRepository = configRepository;
        _tenantService = tenantService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<TenantDashboardDto> Handle(GetTenantDashboardQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting dashboard for tenant: {TenantId}", request.TenantId);

        var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken)
            ?? throw new NotFoundException($"Tenant with ID {request.TenantId} not found");

        var dashboard = new TenantDashboardDto
        {
            Tenant = _mapper.Map<TenantDto>(tenant)
        };

        // Get usage statistics
        var usageStats = await _tenantService.GetTenantUsageStatsAsync(request.TenantId, cancellationToken);
        dashboard.UsageStats = _mapper.Map<TenantUsageStatsDto>(usageStats);

        // Get recent users (last 10)
        var users = await _userRepository.GetByTenantAsync(request.TenantId, cancellationToken);
        dashboard.RecentUsers = _mapper.Map<List<UserSummaryDto>>(
            users.OrderByDescending(u => u.CreatedAt).Take(10));

        // Get recent activity (last 20)
        var auditLogs = await _auditRepository.GetByTenantAsync(request.TenantId, cancellationToken);
        dashboard.RecentActivity = _mapper.Map<List<AuditLogDto>>(
            auditLogs.OrderByDescending(a => a.Timestamp).Take(20));

        // Get tenant configurations
        var configs = await _configRepository.GetByTenantAsync(request.TenantId, cancellationToken);
        dashboard.TenantConfigs = _mapper.Map<List<SystemConfigDto>>(configs);

        // Calculate module usage stats (simplified)
        dashboard.ModuleUsageStats = new Dictionary<string, int>
        {
            ["Assets"] = auditLogs.Count(a => a.EntityType.Contains("Asset")),
            ["Accounts"] = auditLogs.Count(a => a.EntityType.Contains("Account") || a.EntityType.Contains("Invoice")),
            ["Projects"] = auditLogs.Count(a => a.EntityType.Contains("Project")),
            ["Setup"] = auditLogs.Count(a => a.EntityType.Contains("User") || a.EntityType.Contains("Tenant"))
        };

        // Check trial status
        if (tenant.SubscriptionPlan == SubscriptionPlan.Trial)
        {
            var trialStart = tenant.CreatedAt;
            var trialEnd = trialStart.AddDays(30); // 30-day trial
            var daysRemaining = (int)(trialEnd - DateTime.UtcNow).TotalDays;
            
            dashboard.IsTrialExpiringSoon = daysRemaining <= 7 && daysRemaining > 0;
            dashboard.DaysUntilTrialExpiry = Math.Max(0, daysRemaining);
        }

        return dashboard;
    }
}

/// <summary>
/// Paged result wrapper
/// </summary>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
