namespace TossErp.Setup.Application.Queries;

// ============================================================================
// AUDIT LOG QUERIES
// ============================================================================

/// <summary>
/// Get audit logs query with filtering and pagination
/// </summary>
public record GetAuditLogsQuery(
    Guid? TenantId = null,
    Guid? UserId = null,
    string? Action = null,
    string? EntityType = null,
    Guid? EntityId = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    int PageNumber = 1,
    int PageSize = 50,
    string? SortBy = null,
    bool SortDescending = true
) : IRequest<PagedResult<AuditLogDto>>;

public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, PagedResult<AuditLogDto>>
{
    private readonly IAuditLogRepository _auditRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAuditLogsQueryHandler> _logger;

    public GetAuditLogsQueryHandler(
        IAuditLogRepository auditRepository,
        IUserRepository userRepository,
        ITenantRepository tenantRepository,
        IMapper mapper,
        ILogger<GetAuditLogsQueryHandler> logger)
    {
        _auditRepository = auditRepository;
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResult<AuditLogDto>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting audit logs with filters: TenantId={TenantId}, UserId={UserId}", 
            request.TenantId, request.UserId);

        IEnumerable<AuditLog> auditLogs;

        // Get base query result
        if (request.TenantId.HasValue)
        {
            auditLogs = await _auditRepository.GetByTenantAsync(request.TenantId.Value, cancellationToken);
        }
        else if (request.UserId.HasValue)
        {
            auditLogs = await _auditRepository.GetByUserAsync(request.UserId.Value, cancellationToken);
        }
        else if (request.EntityId.HasValue && !string.IsNullOrEmpty(request.EntityType))
        {
            auditLogs = await _auditRepository.GetByEntityAsync(request.EntityType, request.EntityId.Value, cancellationToken);
        }
        else if (!string.IsNullOrEmpty(request.Action))
        {
            auditLogs = await _auditRepository.GetByActionAsync(request.Action, cancellationToken);
        }
        else
        {
            auditLogs = await _auditRepository.GetAllAsync(cancellationToken);
        }

        // Apply additional filters
        if (request.StartDate.HasValue || request.EndDate.HasValue)
        {
            var startDate = request.StartDate ?? DateTime.MinValue;
            var endDate = request.EndDate ?? DateTime.MaxValue;
            auditLogs = auditLogs.Where(a => a.Timestamp >= startDate && a.Timestamp <= endDate);
        }

        if (!string.IsNullOrEmpty(request.Action) && request.UserId.HasValue)
        {
            auditLogs = auditLogs.Where(a => a.Action.Contains(request.Action, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(request.EntityType) && !request.EntityId.HasValue)
        {
            auditLogs = auditLogs.Where(a => a.EntityType.Equals(request.EntityType, StringComparison.OrdinalIgnoreCase));
        }

        // Apply sorting
        auditLogs = request.SortBy?.ToLowerInvariant() switch
        {
            "timestamp" => request.SortDescending ? auditLogs.OrderByDescending(a => a.Timestamp) : auditLogs.OrderBy(a => a.Timestamp),
            "action" => request.SortDescending ? auditLogs.OrderByDescending(a => a.Action) : auditLogs.OrderBy(a => a.Action),
            "entitytype" => request.SortDescending ? auditLogs.OrderByDescending(a => a.EntityType) : auditLogs.OrderBy(a => a.EntityType),
            "userid" => request.SortDescending ? auditLogs.OrderByDescending(a => a.UserId) : auditLogs.OrderBy(a => a.UserId),
            _ => auditLogs.OrderByDescending(a => a.Timestamp)
        };

        // Apply pagination
        var totalCount = auditLogs.Count();
        var pagedAuditLogs = auditLogs
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Map to DTOs and enrich with user/tenant names
        var auditLogDtos = new List<AuditLogDto>();
        
        foreach (var auditLog in pagedAuditLogs)
        {
            var dto = _mapper.Map<AuditLogDto>(auditLog);
            
            // Get user name
            if (auditLog.UserId != Guid.Empty)
            {
                var user = await _userRepository.GetByIdAsync(auditLog.UserId, cancellationToken);
                dto.UserName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown User";
            }
            else
            {
                dto.UserName = "System";
            }

            // Get tenant name
            if (auditLog.TenantId != Guid.Empty)
            {
                var tenant = await _tenantRepository.GetByIdAsync(auditLog.TenantId, cancellationToken);
                dto.TenantName = tenant?.Name ?? "Unknown Tenant";
            }
            else
            {
                dto.TenantName = "System";
            }

            auditLogDtos.Add(dto);
        }

        return new PagedResult<AuditLogDto>
        {
            Items = auditLogDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }
}

/// <summary>
/// Get audit trail for entity query
/// </summary>
public record GetAuditTrailQuery(
    string EntityType,
    Guid EntityId
) : IRequest<IEnumerable<AuditLogDto>>;

public class GetAuditTrailQueryHandler : IRequestHandler<GetAuditTrailQuery, IEnumerable<AuditLogDto>>
{
    private readonly IAuditService _auditService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAuditTrailQueryHandler> _logger;

    public GetAuditTrailQueryHandler(
        IAuditService auditService,
        IMapper mapper,
        ILogger<GetAuditTrailQueryHandler> logger)
    {
        _auditService = auditService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<AuditLogDto>> Handle(GetAuditTrailQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting audit trail for entity: {EntityType} {EntityId}", 
            request.EntityType, request.EntityId);

        var auditLogs = await _auditService.GetAuditTrailAsync(request.EntityType, request.EntityId, cancellationToken);
        return _mapper.Map<IEnumerable<AuditLogDto>>(auditLogs.OrderByDescending(a => a.Timestamp));
    }
}

/// <summary>
/// Get recent activity query
/// </summary>
public record GetRecentActivityQuery(
    Guid? TenantId = null,
    int Count = 20
) : IRequest<IEnumerable<AuditLogDto>>;

public class GetRecentActivityQueryHandler : IRequestHandler<GetRecentActivityQuery, IEnumerable<AuditLogDto>>
{
    private readonly IAuditLogRepository _auditRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetRecentActivityQueryHandler> _logger;

    public GetRecentActivityQueryHandler(
        IAuditLogRepository auditRepository,
        IMapper mapper,
        ILogger<GetRecentActivityQueryHandler> logger)
    {
        _auditRepository = auditRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<AuditLogDto>> Handle(GetRecentActivityQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting recent activity: TenantId={TenantId}, Count={Count}", 
            request.TenantId, request.Count);

        IEnumerable<AuditLog> auditLogs;

        if (request.TenantId.HasValue)
        {
            auditLogs = await _auditRepository.GetByTenantAsync(request.TenantId.Value, cancellationToken);
        }
        else
        {
            auditLogs = await _auditRepository.GetAllAsync(cancellationToken);
        }

        var recentActivity = auditLogs
            .OrderByDescending(a => a.Timestamp)
            .Take(request.Count);

        return _mapper.Map<IEnumerable<AuditLogDto>>(recentActivity);
    }
}

// ============================================================================
// SYSTEM HEALTH AND METRICS QUERIES
// ============================================================================

/// <summary>
/// Get system health query
/// </summary>
public record GetSystemHealthQuery() : IRequest<SystemHealthDto>;

public class GetSystemHealthQueryHandler : IRequestHandler<GetSystemHealthQuery, SystemHealthDto>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAuditLogRepository _auditRepository;
    private readonly ILogger<GetSystemHealthQueryHandler> _logger;

    public GetSystemHealthQueryHandler(
        ITenantRepository tenantRepository,
        IUserRepository userRepository,
        IAuditLogRepository auditRepository,
        ILogger<GetSystemHealthQueryHandler> logger)
    {
        _tenantRepository = tenantRepository;
        _userRepository = userRepository;
        _auditRepository = auditRepository;
        _logger = logger;
    }

    public async Task<SystemHealthDto> Handle(GetSystemHealthQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting system health");

        var health = new SystemHealthDto
        {
            CheckTime = DateTime.UtcNow,
            Status = "Healthy",
            IsHealthy = true,
            Uptime = TimeSpan.FromDays(1), // Would calculate actual uptime
            Components = new Dictionary<string, ComponentHealthDto>(),
            Issues = new List<string>()
        };

        // Check database connectivity
        try
        {
            var tenantCount = (await _tenantRepository.GetAllAsync(cancellationToken)).Count();
            health.Components["Database"] = new ComponentHealthDto
            {
                Name = "Database",
                Status = "Healthy",
                IsHealthy = true,
                ResponseTime = TimeSpan.FromMilliseconds(50),
                LastChecked = DateTime.UtcNow,
                Message = $"Connected successfully. {tenantCount} tenants in database."
            };
        }
        catch (Exception ex)
        {
            health.Components["Database"] = new ComponentHealthDto
            {
                Name = "Database",
                Status = "Unhealthy",
                IsHealthy = false,
                ResponseTime = TimeSpan.FromSeconds(5),
                LastChecked = DateTime.UtcNow,
                Message = ex.Message
            };
            health.IsHealthy = false;
            health.Status = "Unhealthy";
            health.Issues.Add("Database connectivity issues");
        }

        // Get system metrics
        var tenants = await _tenantRepository.GetAllAsync(cancellationToken);
        var activeTenants = await _tenantRepository.GetActiveTenantsAsync(cancellationToken);
        
        health.Metrics = new SystemMetricsDto
        {
            TotalTenants = tenants.Count(),
            ActiveTenants = activeTenants.Count(),
            TotalUsers = 0, // Would aggregate from all tenants
            ActiveUsers = 0,
            TotalStorageUsed = 0,
            ApiCallsToday = 0,
            ApiCallsThisMonth = 0,
            CpuUsagePercentage = 25.5, // Would get from system monitoring
            MemoryUsagePercentage = 45.2,
            DiskUsagePercentage = 67.8,
            DatabaseConnections = 5,
            AverageResponseTime = TimeSpan.FromMilliseconds(150),
            ErrorsInLast24Hours = 0
        };

        return health;
    }
}

/// <summary>
/// Get system dashboard query
/// </summary>
public record GetSystemDashboardQuery() : IRequest<SystemDashboardDto>;

public class GetSystemDashboardQueryHandler : IRequestHandler<GetSystemDashboardQuery, SystemDashboardDto>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAuditLogRepository _auditRepository;
    private readonly ITenantService _tenantService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetSystemDashboardQueryHandler> _logger;

    public GetSystemDashboardQueryHandler(
        ITenantRepository tenantRepository,
        IUserRepository userRepository,
        IAuditLogRepository auditRepository,
        ITenantService tenantService,
        IMapper mapper,
        ILogger<GetSystemDashboardQueryHandler> logger)
    {
        _tenantRepository = tenantRepository;
        _userRepository = userRepository;
        _auditRepository = auditRepository;
        _tenantService = tenantService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<SystemDashboardDto> Handle(GetSystemDashboardQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting system dashboard");

        var dashboard = new SystemDashboardDto();

        // Get system health
        var healthHandler = new GetSystemHealthQueryHandler(_tenantRepository, _userRepository, _auditRepository, _logger);
        dashboard.Health = await healthHandler.Handle(new GetSystemHealthQuery(), cancellationToken);
        dashboard.Metrics = dashboard.Health.Metrics;

        // Get recent tenants
        var allTenants = await _tenantRepository.GetAllAsync(cancellationToken);
        dashboard.RecentTenants = _mapper.Map<List<TenantSummaryDto>>(
            allTenants.OrderByDescending(t => t.CreatedAt).Take(10));

        // Get recent activity
        var auditLogs = await _auditRepository.GetAllAsync(cancellationToken);
        dashboard.RecentActivity = _mapper.Map<List<AuditLogDto>>(
            auditLogs.OrderByDescending(a => a.Timestamp).Take(20));

        // Calculate tenant distribution by plan
        dashboard.TenantsByPlan = allTenants
            .GroupBy(t => t.SubscriptionPlan.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        // Get top tenants by usage
        dashboard.TopTenantsByUsage = new List<TenantUsageStatsDto>();
        foreach (var tenant in allTenants.Take(10))
        {
            try
            {
                var usageStats = await _tenantService.GetTenantUsageStatsAsync(tenant.Id, cancellationToken);
                var dto = _mapper.Map<TenantUsageStatsDto>(usageStats);
                dto.TenantName = tenant.Name;
                dashboard.TopTenantsByUsage.Add(dto);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get usage stats for tenant {TenantId}", tenant.Id);
            }
        }

        dashboard.TopTenantsByUsage = dashboard.TopTenantsByUsage
            .OrderByDescending(t => t.ApiCallsThisMonth)
            .ToList();

        // System alerts (simplified)
        dashboard.SystemAlerts = new List<string>();
        if (dashboard.Health.Metrics.CpuUsagePercentage > 80)
        {
            dashboard.SystemAlerts.Add("High CPU usage detected");
        }
        if (dashboard.Health.Metrics.MemoryUsagePercentage > 85)
        {
            dashboard.SystemAlerts.Add("High memory usage detected");
        }
        if (dashboard.Health.Metrics.DiskUsagePercentage > 90)
        {
            dashboard.SystemAlerts.Add("Low disk space warning");
        }

        dashboard.MaintenanceMode = false;
        dashboard.ScheduledMaintenanceDate = null;

        return dashboard;
    }
}
