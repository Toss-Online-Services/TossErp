namespace TossErp.Projects.Application.Queries;

/// <summary>
/// Query to get paginated list of project tasks
/// </summary>
public record GetProjectTasksQuery(
    int PageNumber = 1,
    int PageSize = 50,
    string? SearchTerm = null,
    TaskStatus? Status = null,
    Guid? ProjectId = null,
    Guid? AssigneeId = null,
    TaskPriority? Priority = null,
    DateOnly? DueDateFrom = null,
    DateOnly? DueDateTo = null,
    bool IncludeSubTasks = false,
    string? SortBy = null,
    bool SortDescending = false
) : IRequest<PaginatedResult<ProjectTaskDto>>;

/// <summary>
/// Handler for GetProjectTasksQuery
/// </summary>
public class GetProjectTasksQueryHandler : IRequestHandler<GetProjectTasksQuery, PaginatedResult<ProjectTaskDto>>
{
    private readonly IProjectTaskRepository _taskRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProjectTasksQueryHandler> _logger;

    public GetProjectTasksQueryHandler(
        IProjectTaskRepository taskRepository,
        IMapper mapper,
        ILogger<GetProjectTasksQueryHandler> logger)
    {
        _taskRepository = taskRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PaginatedResult<ProjectTaskDto>> Handle(GetProjectTasksQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting project tasks - Page: {PageNumber}, Size: {PageSize}", 
            request.PageNumber, request.PageSize);

        var tasks = await _taskRepository.GetTasksAsync(
            request.PageNumber,
            request.PageSize,
            request.SearchTerm,
            request.Status,
            request.ProjectId,
            request.AssigneeId,
            request.Priority,
            cancellationToken);

        var taskDtos = _mapper.Map<IEnumerable<ProjectTaskDto>>(tasks.Items);

        return new PaginatedResult<ProjectTaskDto>
        {
            Items = taskDtos,
            TotalCount = tasks.TotalCount,
            PageNumber = tasks.PageNumber,
            PageSize = tasks.PageSize,
            TotalPages = tasks.TotalPages
        };
    }
}

/// <summary>
/// Query to get project task by ID
/// </summary>
public record GetProjectTaskByIdQuery(Guid Id) : IRequest<ProjectTaskDto?>;

/// <summary>
/// Handler for GetProjectTaskByIdQuery
/// </summary>
public class GetProjectTaskByIdQueryHandler : IRequestHandler<GetProjectTaskByIdQuery, ProjectTaskDto?>
{
    private readonly IProjectTaskRepository _taskRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProjectTaskByIdQueryHandler> _logger;

    public GetProjectTaskByIdQueryHandler(
        IProjectTaskRepository taskRepository,
        IMapper mapper,
        ILogger<GetProjectTaskByIdQueryHandler> logger)
    {
        _taskRepository = taskRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProjectTaskDto?> Handle(GetProjectTaskByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting project task by ID: {TaskId}", request.Id);

        var task = await _taskRepository.GetByIdAsync(request.Id, cancellationToken);
        
        return task != null ? _mapper.Map<ProjectTaskDto>(task) : null;
    }
}

/// <summary>
/// Query to get tasks by project
/// </summary>
public record GetTasksByProjectQuery(Guid ProjectId) : IRequest<IEnumerable<ProjectTaskDto>>;

/// <summary>
/// Handler for GetTasksByProjectQuery
/// </summary>
public class GetTasksByProjectQueryHandler : IRequestHandler<GetTasksByProjectQuery, IEnumerable<ProjectTaskDto>>
{
    private readonly IProjectTaskRepository _taskRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTasksByProjectQueryHandler> _logger;

    public GetTasksByProjectQueryHandler(
        IProjectTaskRepository taskRepository,
        IMapper mapper,
        ILogger<GetTasksByProjectQueryHandler> logger)
    {
        _taskRepository = taskRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<ProjectTaskDto>> Handle(GetTasksByProjectQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting tasks by project: {ProjectId}", request.ProjectId);

        var tasks = await _taskRepository.GetByProjectIdAsync(request.ProjectId, cancellationToken);
        
        return _mapper.Map<IEnumerable<ProjectTaskDto>>(tasks);
    }
}

/// <summary>
/// Query to get tasks by assignee
/// </summary>
public record GetTasksByAssigneeQuery(Guid AssigneeId) : IRequest<IEnumerable<ProjectTaskDto>>;

/// <summary>
/// Handler for GetTasksByAssigneeQuery
/// </summary>
public class GetTasksByAssigneeQueryHandler : IRequestHandler<GetTasksByAssigneeQuery, IEnumerable<ProjectTaskDto>>
{
    private readonly IProjectTaskRepository _taskRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTasksByAssigneeQueryHandler> _logger;

    public GetTasksByAssigneeQueryHandler(
        IProjectTaskRepository taskRepository,
        IMapper mapper,
        ILogger<GetTasksByAssigneeQueryHandler> logger)
    {
        _taskRepository = taskRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<ProjectTaskDto>> Handle(GetTasksByAssigneeQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting tasks by assignee: {AssigneeId}", request.AssigneeId);

        var tasks = await _taskRepository.GetByAssigneeIdAsync(request.AssigneeId, cancellationToken);
        
        return _mapper.Map<IEnumerable<ProjectTaskDto>>(tasks);
    }
}

/// <summary>
/// Query to get overdue tasks
/// </summary>
public record GetOverdueTasksQuery() : IRequest<IEnumerable<ProjectTaskDto>>;

/// <summary>
/// Handler for GetOverdueTasksQuery
/// </summary>
public class GetOverdueTasksQueryHandler : IRequestHandler<GetOverdueTasksQuery, IEnumerable<ProjectTaskDto>>
{
    private readonly IProjectTaskRepository _taskRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetOverdueTasksQueryHandler> _logger;

    public GetOverdueTasksQueryHandler(
        IProjectTaskRepository taskRepository,
        IMapper mapper,
        ILogger<GetOverdueTasksQueryHandler> logger)
    {
        _taskRepository = taskRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<ProjectTaskDto>> Handle(GetOverdueTasksQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting overdue tasks");

        var tasks = await _taskRepository.GetOverdueTasksAsync(cancellationToken);
        
        return _mapper.Map<IEnumerable<ProjectTaskDto>>(tasks);
    }
}

/// <summary>
/// Query to get tasks due on a specific date
/// </summary>
public record GetTasksDueQuery(DateOnly DueDate) : IRequest<IEnumerable<ProjectTaskDto>>;

/// <summary>
/// Handler for GetTasksDueQuery
/// </summary>
public class GetTasksDueQueryHandler : IRequestHandler<GetTasksDueQuery, IEnumerable<ProjectTaskDto>>
{
    private readonly IProjectTaskRepository _taskRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTasksDueQueryHandler> _logger;

    public GetTasksDueQueryHandler(
        IProjectTaskRepository taskRepository,
        IMapper mapper,
        ILogger<GetTasksDueQueryHandler> logger)
    {
        _taskRepository = taskRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<ProjectTaskDto>> Handle(GetTasksDueQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting tasks due on: {DueDate}", request.DueDate);

        var tasks = await _taskRepository.GetTasksDueAsync(request.DueDate, cancellationToken);
        
        return _mapper.Map<IEnumerable<ProjectTaskDto>>(tasks);
    }
}

/// <summary>
/// Query to get paginated list of time entries
/// </summary>
public record GetTimeEntriesQuery(
    int PageNumber = 1,
    int PageSize = 50,
    Guid? ProjectId = null,
    Guid? TaskId = null,
    Guid? UserId = null,
    DateOnly? FromDate = null,
    DateOnly? ToDate = null,
    bool? IsBillable = null
) : IRequest<PaginatedResult<TimeEntryDto>>;

/// <summary>
/// Handler for GetTimeEntriesQuery
/// </summary>
public class GetTimeEntriesQueryHandler : IRequestHandler<GetTimeEntriesQuery, PaginatedResult<TimeEntryDto>>
{
    private readonly ITimeEntryRepository _timeEntryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTimeEntriesQueryHandler> _logger;

    public GetTimeEntriesQueryHandler(
        ITimeEntryRepository timeEntryRepository,
        IMapper mapper,
        ILogger<GetTimeEntriesQueryHandler> logger)
    {
        _timeEntryRepository = timeEntryRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PaginatedResult<TimeEntryDto>> Handle(GetTimeEntriesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting time entries - Page: {PageNumber}, Size: {PageSize}", 
            request.PageNumber, request.PageSize);

        var timeEntries = await _timeEntryRepository.GetTimeEntriesAsync(
            request.PageNumber,
            request.PageSize,
            request.ProjectId,
            request.TaskId,
            request.UserId,
            request.FromDate,
            request.ToDate,
            cancellationToken);

        var timeEntryDtos = _mapper.Map<IEnumerable<TimeEntryDto>>(timeEntries.Items);

        return new PaginatedResult<TimeEntryDto>
        {
            Items = timeEntryDtos,
            TotalCount = timeEntries.TotalCount,
            PageNumber = timeEntries.PageNumber,
            PageSize = timeEntries.PageSize,
            TotalPages = timeEntries.TotalPages
        };
    }
}

/// <summary>
/// Query to get time entries by project
/// </summary>
public record GetTimeEntriesByProjectQuery(Guid ProjectId) : IRequest<IEnumerable<TimeEntryDto>>;

/// <summary>
/// Handler for GetTimeEntriesByProjectQuery
/// </summary>
public class GetTimeEntriesByProjectQueryHandler : IRequestHandler<GetTimeEntriesByProjectQuery, IEnumerable<TimeEntryDto>>
{
    private readonly ITimeEntryRepository _timeEntryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTimeEntriesByProjectQueryHandler> _logger;

    public GetTimeEntriesByProjectQueryHandler(
        ITimeEntryRepository timeEntryRepository,
        IMapper mapper,
        ILogger<GetTimeEntriesByProjectQueryHandler> logger)
    {
        _timeEntryRepository = timeEntryRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<TimeEntryDto>> Handle(GetTimeEntriesByProjectQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting time entries by project: {ProjectId}", request.ProjectId);

        var timeEntries = await _timeEntryRepository.GetByProjectIdAsync(request.ProjectId, cancellationToken);
        
        return _mapper.Map<IEnumerable<TimeEntryDto>>(timeEntries);
    }
}

/// <summary>
/// Query to get time entries by task
/// </summary>
public record GetTimeEntriesByTaskQuery(Guid TaskId) : IRequest<IEnumerable<TimeEntryDto>>;

/// <summary>
/// Handler for GetTimeEntriesByTaskQuery
/// </summary>
public class GetTimeEntriesByTaskQueryHandler : IRequestHandler<GetTimeEntriesByTaskQuery, IEnumerable<TimeEntryDto>>
{
    private readonly ITimeEntryRepository _timeEntryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTimeEntriesByTaskQueryHandler> _logger;

    public GetTimeEntriesByTaskQueryHandler(
        ITimeEntryRepository timeEntryRepository,
        IMapper mapper,
        ILogger<GetTimeEntriesByTaskQueryHandler> logger)
    {
        _timeEntryRepository = timeEntryRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<TimeEntryDto>> Handle(GetTimeEntriesByTaskQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting time entries by task: {TaskId}", request.TaskId);

        var timeEntries = await _timeEntryRepository.GetByTaskIdAsync(request.TaskId, cancellationToken);
        
        return _mapper.Map<IEnumerable<TimeEntryDto>>(timeEntries);
    }
}

/// <summary>
/// Query to get time entries by user
/// </summary>
public record GetTimeEntriesByUserQuery(
    Guid UserId,
    DateOnly? FromDate = null,
    DateOnly? ToDate = null
) : IRequest<IEnumerable<TimeEntryDto>>;

/// <summary>
/// Handler for GetTimeEntriesByUserQuery
/// </summary>
public class GetTimeEntriesByUserQueryHandler : IRequestHandler<GetTimeEntriesByUserQuery, IEnumerable<TimeEntryDto>>
{
    private readonly ITimeEntryRepository _timeEntryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTimeEntriesByUserQueryHandler> _logger;

    public GetTimeEntriesByUserQueryHandler(
        ITimeEntryRepository timeEntryRepository,
        IMapper mapper,
        ILogger<GetTimeEntriesByUserQueryHandler> logger)
    {
        _timeEntryRepository = timeEntryRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<TimeEntryDto>> Handle(GetTimeEntriesByUserQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting time entries by user: {UserId}", request.UserId);

        var timeEntries = await _timeEntryRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        
        if (request.FromDate.HasValue || request.ToDate.HasValue)
        {
            var fromDate = request.FromDate ?? DateOnly.MinValue;
            var toDate = request.ToDate ?? DateOnly.MaxValue;
            
            timeEntries = timeEntries.Where(te => te.Date >= fromDate && te.Date <= toDate);
        }
        
        return _mapper.Map<IEnumerable<TimeEntryDto>>(timeEntries);
    }
}

/// <summary>
/// Query to get total hours
/// </summary>
public record GetTotalHoursQuery(
    Guid? ProjectId = null,
    Guid? TaskId = null,
    Guid? UserId = null,
    DateOnly? FromDate = null,
    DateOnly? ToDate = null
) : IRequest<decimal>;

/// <summary>
/// Handler for GetTotalHoursQuery
/// </summary>
public class GetTotalHoursQueryHandler : IRequestHandler<GetTotalHoursQuery, decimal>
{
    private readonly ITimeEntryRepository _timeEntryRepository;
    private readonly ILogger<GetTotalHoursQueryHandler> _logger;

    public GetTotalHoursQueryHandler(
        ITimeEntryRepository timeEntryRepository,
        ILogger<GetTotalHoursQueryHandler> logger)
    {
        _timeEntryRepository = timeEntryRepository;
        _logger = logger;
    }

    public async Task<decimal> Handle(GetTotalHoursQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting total hours");

        var totalHours = await _timeEntryRepository.GetTotalHoursAsync(
            request.ProjectId,
            request.TaskId,
            request.UserId,
            request.FromDate,
            request.ToDate,
            cancellationToken);
        
        return totalHours;
    }
}
