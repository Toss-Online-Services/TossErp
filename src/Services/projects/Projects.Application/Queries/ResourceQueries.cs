namespace TossErp.Projects.Application.Queries;

/// <summary>
/// Query to get paginated list of resources
/// </summary>
public record GetResourcesQuery(
    int PageNumber = 1,
    int PageSize = 50,
    string? SearchTerm = null,
    ResourceType? Type = null,
    bool? IsAvailable = null
) : IRequest<PaginatedResult<ResourceDto>>;

/// <summary>
/// Handler for GetResourcesQuery
/// </summary>
public class GetResourcesQueryHandler : IRequestHandler<GetResourcesQuery, PaginatedResult<ResourceDto>>
{
    private readonly IResourceRepository _resourceRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetResourcesQueryHandler> _logger;

    public GetResourcesQueryHandler(
        IResourceRepository resourceRepository,
        IMapper mapper,
        ILogger<GetResourcesQueryHandler> logger)
    {
        _resourceRepository = resourceRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PaginatedResult<ResourceDto>> Handle(GetResourcesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting resources - Page: {PageNumber}, Size: {PageSize}", 
            request.PageNumber, request.PageSize);

        var resources = await _resourceRepository.GetResourcesAsync(
            request.PageNumber,
            request.PageSize,
            request.SearchTerm,
            request.Type,
            request.IsAvailable,
            cancellationToken);

        var resourceDtos = _mapper.Map<IEnumerable<ResourceDto>>(resources.Items);

        return new PaginatedResult<ResourceDto>
        {
            Items = resourceDtos,
            TotalCount = resources.TotalCount,
            PageNumber = resources.PageNumber,
            PageSize = resources.PageSize,
            TotalPages = resources.TotalPages
        };
    }
}

/// <summary>
/// Query to get resource by ID
/// </summary>
public record GetResourceByIdQuery(Guid Id) : IRequest<ResourceDto?>;

/// <summary>
/// Handler for GetResourceByIdQuery
/// </summary>
public class GetResourceByIdQueryHandler : IRequestHandler<GetResourceByIdQuery, ResourceDto?>
{
    private readonly IResourceRepository _resourceRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetResourceByIdQueryHandler> _logger;

    public GetResourceByIdQueryHandler(
        IResourceRepository resourceRepository,
        IMapper mapper,
        ILogger<GetResourceByIdQueryHandler> logger)
    {
        _resourceRepository = resourceRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ResourceDto?> Handle(GetResourceByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting resource by ID: {ResourceId}", request.Id);

        var resource = await _resourceRepository.GetByIdAsync(request.Id, cancellationToken);
        
        return resource != null ? _mapper.Map<ResourceDto>(resource) : null;
    }
}

/// <summary>
/// Query to get resources by project
/// </summary>
public record GetResourcesByProjectQuery(Guid ProjectId) : IRequest<IEnumerable<ResourceDto>>;

/// <summary>
/// Handler for GetResourcesByProjectQuery
/// </summary>
public class GetResourcesByProjectQueryHandler : IRequestHandler<GetResourcesByProjectQuery, IEnumerable<ResourceDto>>
{
    private readonly IResourceRepository _resourceRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetResourcesByProjectQueryHandler> _logger;

    public GetResourcesByProjectQueryHandler(
        IResourceRepository resourceRepository,
        IMapper mapper,
        ILogger<GetResourcesByProjectQueryHandler> logger)
    {
        _resourceRepository = resourceRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<ResourceDto>> Handle(GetResourcesByProjectQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting resources by project: {ProjectId}", request.ProjectId);

        var resources = await _resourceRepository.GetByProjectIdAsync(request.ProjectId, cancellationToken);
        
        return _mapper.Map<IEnumerable<ResourceDto>>(resources);
    }
}

/// <summary>
/// Query to get available resources
/// </summary>
public record GetAvailableResourcesQuery(
    DateOnly FromDate,
    DateOnly ToDate
) : IRequest<IEnumerable<ResourceDto>>;

/// <summary>
/// Handler for GetAvailableResourcesQuery
/// </summary>
public class GetAvailableResourcesQueryHandler : IRequestHandler<GetAvailableResourcesQuery, IEnumerable<ResourceDto>>
{
    private readonly IResourceRepository _resourceRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAvailableResourcesQueryHandler> _logger;

    public GetAvailableResourcesQueryHandler(
        IResourceRepository resourceRepository,
        IMapper mapper,
        ILogger<GetAvailableResourcesQueryHandler> logger)
    {
        _resourceRepository = resourceRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<ResourceDto>> Handle(GetAvailableResourcesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting available resources from {FromDate} to {ToDate}", 
            request.FromDate, request.ToDate);

        var resources = await _resourceRepository.GetAvailableResourcesAsync(request.FromDate, request.ToDate, cancellationToken);
        
        return _mapper.Map<IEnumerable<ResourceDto>>(resources);
    }
}

/// <summary>
/// Query to get paginated list of milestones
/// </summary>
public record GetMilestonesQuery(
    int PageNumber = 1,
    int PageSize = 50,
    Guid? ProjectId = null,
    MilestoneStatus? Status = null
) : IRequest<PaginatedResult<MilestoneDto>>;

/// <summary>
/// Handler for GetMilestonesQuery
/// </summary>
public class GetMilestonesQueryHandler : IRequestHandler<GetMilestonesQuery, PaginatedResult<MilestoneDto>>
{
    private readonly IMilestoneRepository _milestoneRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetMilestonesQueryHandler> _logger;

    public GetMilestonesQueryHandler(
        IMilestoneRepository milestoneRepository,
        IMapper mapper,
        ILogger<GetMilestonesQueryHandler> logger)
    {
        _milestoneRepository = milestoneRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PaginatedResult<MilestoneDto>> Handle(GetMilestonesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting milestones - Page: {PageNumber}, Size: {PageSize}", 
            request.PageNumber, request.PageSize);

        var milestones = await _milestoneRepository.GetMilestonesAsync(
            request.PageNumber,
            request.PageSize,
            request.ProjectId,
            request.Status,
            cancellationToken);

        var milestoneDtos = _mapper.Map<IEnumerable<MilestoneDto>>(milestones.Items);

        return new PaginatedResult<MilestoneDto>
        {
            Items = milestoneDtos,
            TotalCount = milestones.TotalCount,
            PageNumber = milestones.PageNumber,
            PageSize = milestones.PageSize,
            TotalPages = milestones.TotalPages
        };
    }
}

/// <summary>
/// Query to get milestone by ID
/// </summary>
public record GetMilestoneByIdQuery(Guid Id) : IRequest<MilestoneDto?>;

/// <summary>
/// Handler for GetMilestoneByIdQuery
/// </summary>
public class GetMilestoneByIdQueryHandler : IRequestHandler<GetMilestoneByIdQuery, MilestoneDto?>
{
    private readonly IMilestoneRepository _milestoneRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetMilestoneByIdQueryHandler> _logger;

    public GetMilestoneByIdQueryHandler(
        IMilestoneRepository milestoneRepository,
        IMapper mapper,
        ILogger<GetMilestoneByIdQueryHandler> logger)
    {
        _milestoneRepository = milestoneRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<MilestoneDto?> Handle(GetMilestoneByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting milestone by ID: {MilestoneId}", request.Id);

        var milestone = await _milestoneRepository.GetByIdAsync(request.Id, cancellationToken);
        
        return milestone != null ? _mapper.Map<MilestoneDto>(milestone) : null;
    }
}

/// <summary>
/// Query to get milestones by project
/// </summary>
public record GetMilestonesByProjectQuery(Guid ProjectId) : IRequest<IEnumerable<MilestoneDto>>;

/// <summary>
/// Handler for GetMilestonesByProjectQuery
/// </summary>
public class GetMilestonesByProjectQueryHandler : IRequestHandler<GetMilestonesByProjectQuery, IEnumerable<MilestoneDto>>
{
    private readonly IMilestoneRepository _milestoneRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetMilestonesByProjectQueryHandler> _logger;

    public GetMilestonesByProjectQueryHandler(
        IMilestoneRepository milestoneRepository,
        IMapper mapper,
        ILogger<GetMilestonesByProjectQueryHandler> logger)
    {
        _milestoneRepository = milestoneRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<MilestoneDto>> Handle(GetMilestonesByProjectQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting milestones by project: {ProjectId}", request.ProjectId);

        var milestones = await _milestoneRepository.GetByProjectIdAsync(request.ProjectId, cancellationToken);
        
        return _mapper.Map<IEnumerable<MilestoneDto>>(milestones);
    }
}

/// <summary>
/// Query to get upcoming milestones
/// </summary>
public record GetUpcomingMilestonesQuery(DateOnly? BeforeDate = null) : IRequest<IEnumerable<MilestoneDto>>;

/// <summary>
/// Handler for GetUpcomingMilestonesQuery
/// </summary>
public class GetUpcomingMilestonesQueryHandler : IRequestHandler<GetUpcomingMilestonesQuery, IEnumerable<MilestoneDto>>
{
    private readonly IMilestoneRepository _milestoneRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUpcomingMilestonesQueryHandler> _logger;

    public GetUpcomingMilestonesQueryHandler(
        IMilestoneRepository milestoneRepository,
        IMapper mapper,
        ILogger<GetUpcomingMilestonesQueryHandler> logger)
    {
        _milestoneRepository = milestoneRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<MilestoneDto>> Handle(GetUpcomingMilestonesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting upcoming milestones");

        var milestones = await _milestoneRepository.GetUpcomingMilestonesAsync(request.BeforeDate, cancellationToken);
        
        return _mapper.Map<IEnumerable<MilestoneDto>>(milestones);
    }
}

/// <summary>
/// Query to get overdue milestones
/// </summary>
public record GetOverdueMilestonesQuery() : IRequest<IEnumerable<MilestoneDto>>;

/// <summary>
/// Handler for GetOverdueMilestonesQuery
/// </summary>
public class GetOverdueMilestonesQueryHandler : IRequestHandler<GetOverdueMilestonesQuery, IEnumerable<MilestoneDto>>
{
    private readonly IMilestoneRepository _milestoneRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetOverdueMilestonesQueryHandler> _logger;

    public GetOverdueMilestonesQueryHandler(
        IMilestoneRepository milestoneRepository,
        IMapper mapper,
        ILogger<GetOverdueMilestonesQueryHandler> logger)
    {
        _milestoneRepository = milestoneRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<MilestoneDto>> Handle(GetOverdueMilestonesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting overdue milestones");

        var milestones = await _milestoneRepository.GetOverdueMilestonesAsync(cancellationToken);
        
        return _mapper.Map<IEnumerable<MilestoneDto>>(milestones);
    }
}
