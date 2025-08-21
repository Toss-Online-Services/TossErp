namespace TossErp.Projects.Application.Queries;

/// <summary>
/// Query to get paginated list of projects
/// </summary>
public record GetProjectsQuery(
    int PageNumber = 1,
    int PageSize = 50,
    string? SearchTerm = null,
    ProjectStatus? Status = null,
    Guid? CustomerId = null,
    Guid? ProjectManagerId = null,
    DateOnly? StartDateFrom = null,
    DateOnly? StartDateTo = null,
    ProjectPriority? Priority = null,
    string? SortBy = null,
    bool SortDescending = false
) : IRequest<PaginatedResult<ProjectDto>>;

/// <summary>
/// Handler for GetProjectsQuery
/// </summary>
public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, PaginatedResult<ProjectDto>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProjectsQueryHandler> _logger;

    public GetProjectsQueryHandler(
        IProjectRepository projectRepository,
        IMapper mapper,
        ILogger<GetProjectsQueryHandler> logger)
    {
        _projectRepository = projectRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PaginatedResult<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting projects - Page: {PageNumber}, Size: {PageSize}", 
            request.PageNumber, request.PageSize);

        var projects = await _projectRepository.GetProjectsAsync(
            request.PageNumber,
            request.PageSize,
            request.SearchTerm,
            request.Status,
            request.CustomerId,
            request.ProjectManagerId,
            cancellationToken);

        var projectDtos = _mapper.Map<IEnumerable<ProjectDto>>(projects.Items);

        return new PaginatedResult<ProjectDto>
        {
            Items = projectDtos,
            TotalCount = projects.TotalCount,
            PageNumber = projects.PageNumber,
            PageSize = projects.PageSize,
            TotalPages = projects.TotalPages
        };
    }
}

/// <summary>
/// Query to get project by ID
/// </summary>
public record GetProjectByIdQuery(Guid Id) : IRequest<ProjectDto?>;

/// <summary>
/// Handler for GetProjectByIdQuery
/// </summary>
public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto?>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProjectByIdQueryHandler> _logger;

    public GetProjectByIdQueryHandler(
        IProjectRepository projectRepository,
        IMapper mapper,
        ILogger<GetProjectByIdQueryHandler> logger)
    {
        _projectRepository = projectRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProjectDto?> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting project by ID: {ProjectId}", request.Id);

        var project = await _projectRepository.GetByIdAsync(request.Id, cancellationToken);
        
        return project != null ? _mapper.Map<ProjectDto>(project) : null;
    }
}

/// <summary>
/// Query to get projects by customer
/// </summary>
public record GetProjectsByCustomerQuery(Guid CustomerId) : IRequest<IEnumerable<ProjectDto>>;

/// <summary>
/// Handler for GetProjectsByCustomerQuery
/// </summary>
public class GetProjectsByCustomerQueryHandler : IRequestHandler<GetProjectsByCustomerQuery, IEnumerable<ProjectDto>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProjectsByCustomerQueryHandler> _logger;

    public GetProjectsByCustomerQueryHandler(
        IProjectRepository projectRepository,
        IMapper mapper,
        ILogger<GetProjectsByCustomerQueryHandler> logger)
    {
        _projectRepository = projectRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<ProjectDto>> Handle(GetProjectsByCustomerQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting projects by customer: {CustomerId}", request.CustomerId);

        var projects = await _projectRepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);
        
        return _mapper.Map<IEnumerable<ProjectDto>>(projects);
    }
}

/// <summary>
/// Query to get projects by manager
/// </summary>
public record GetProjectsByManagerQuery(Guid ManagerId) : IRequest<IEnumerable<ProjectDto>>;

/// <summary>
/// Handler for GetProjectsByManagerQuery
/// </summary>
public class GetProjectsByManagerQueryHandler : IRequestHandler<GetProjectsByManagerQuery, IEnumerable<ProjectDto>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProjectsByManagerQueryHandler> _logger;

    public GetProjectsByManagerQueryHandler(
        IProjectRepository projectRepository,
        IMapper mapper,
        ILogger<GetProjectsByManagerQueryHandler> logger)
    {
        _projectRepository = projectRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<ProjectDto>> Handle(GetProjectsByManagerQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting projects by manager: {ManagerId}", request.ManagerId);

        var projects = await _projectRepository.GetByManagerIdAsync(request.ManagerId, cancellationToken);
        
        return _mapper.Map<IEnumerable<ProjectDto>>(projects);
    }
}

/// <summary>
/// Query to get overdue projects
/// </summary>
public record GetOverdueProjectsQuery() : IRequest<IEnumerable<ProjectDto>>;

/// <summary>
/// Handler for GetOverdueProjectsQuery
/// </summary>
public class GetOverdueProjectsQueryHandler : IRequestHandler<GetOverdueProjectsQuery, IEnumerable<ProjectDto>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetOverdueProjectsQueryHandler> _logger;

    public GetOverdueProjectsQueryHandler(
        IProjectRepository projectRepository,
        IMapper mapper,
        ILogger<GetOverdueProjectsQueryHandler> logger)
    {
        _projectRepository = projectRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<ProjectDto>> Handle(GetOverdueProjectsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting overdue projects");

        var projects = await _projectRepository.GetOverdueProjectsAsync(cancellationToken);
        
        return _mapper.Map<IEnumerable<ProjectDto>>(projects);
    }
}

/// <summary>
/// Query to get project summary
/// </summary>
public record GetProjectSummaryQuery(Guid ProjectId) : IRequest<ProjectSummaryDto>;

/// <summary>
/// Handler for GetProjectSummaryQuery
/// </summary>
public class GetProjectSummaryQueryHandler : IRequestHandler<GetProjectSummaryQuery, ProjectSummaryDto>
{
    private readonly IProjectReportingService _reportingService;
    private readonly ILogger<GetProjectSummaryQueryHandler> _logger;

    public GetProjectSummaryQueryHandler(
        IProjectReportingService reportingService,
        ILogger<GetProjectSummaryQueryHandler> logger)
    {
        _reportingService = reportingService;
        _logger = logger;
    }

    public async Task<ProjectSummaryDto> Handle(GetProjectSummaryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting project summary: {ProjectId}", request.ProjectId);

        var summary = await _reportingService.GetProjectSummaryAsync(request.ProjectId, cancellationToken);
        
        return summary;
    }
}

/// <summary>
/// Query to get project progress report
/// </summary>
public record GetProjectProgressReportQuery(Guid ProjectId) : IRequest<ProjectProgressReportDto>;

/// <summary>
/// Handler for GetProjectProgressReportQuery
/// </summary>
public class GetProjectProgressReportQueryHandler : IRequestHandler<GetProjectProgressReportQuery, ProjectProgressReportDto>
{
    private readonly IProjectReportingService _reportingService;
    private readonly ILogger<GetProjectProgressReportQueryHandler> _logger;

    public GetProjectProgressReportQueryHandler(
        IProjectReportingService reportingService,
        ILogger<GetProjectProgressReportQueryHandler> logger)
    {
        _reportingService = reportingService;
        _logger = logger;
    }

    public async Task<ProjectProgressReportDto> Handle(GetProjectProgressReportQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting project progress report: {ProjectId}", request.ProjectId);

        var report = await _reportingService.GetProjectProgressReportAsync(request.ProjectId, cancellationToken);
        
        return report;
    }
}

/// <summary>
/// Query to get project templates
/// </summary>
public record GetProjectTemplatesQuery() : IRequest<IEnumerable<ProjectTemplateDto>>;

/// <summary>
/// Handler for GetProjectTemplatesQuery
/// </summary>
public class GetProjectTemplatesQueryHandler : IRequestHandler<GetProjectTemplatesQuery, IEnumerable<ProjectTemplateDto>>
{
    private readonly IProjectTemplateService _templateService;
    private readonly ILogger<GetProjectTemplatesQueryHandler> _logger;

    public GetProjectTemplatesQueryHandler(
        IProjectTemplateService templateService,
        ILogger<GetProjectTemplatesQueryHandler> logger)
    {
        _templateService = templateService;
        _logger = logger;
    }

    public async Task<IEnumerable<ProjectTemplateDto>> Handle(GetProjectTemplatesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting project templates");

        var templates = await _templateService.GetAvailableTemplatesAsync(cancellationToken);
        
        return templates;
    }
}
