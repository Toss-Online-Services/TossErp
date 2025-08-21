namespace TossErp.Projects.Application.Queries;

/// <summary>
/// Query to get time tracking report
/// </summary>
public record GetTimeTrackingReportQuery(
    DateOnly FromDate,
    DateOnly ToDate,
    Guid? ProjectId = null,
    Guid? UserId = null
) : IRequest<TimeTrackingReportDto>;

/// <summary>
/// Handler for GetTimeTrackingReportQuery
/// </summary>
public class GetTimeTrackingReportQueryHandler : IRequestHandler<GetTimeTrackingReportQuery, TimeTrackingReportDto>
{
    private readonly IProjectReportingService _reportingService;
    private readonly ILogger<GetTimeTrackingReportQueryHandler> _logger;

    public GetTimeTrackingReportQueryHandler(
        IProjectReportingService reportingService,
        ILogger<GetTimeTrackingReportQueryHandler> logger)
    {
        _reportingService = reportingService;
        _logger = logger;
    }

    public async Task<TimeTrackingReportDto> Handle(GetTimeTrackingReportQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting time tracking report from {FromDate} to {ToDate}", 
            request.FromDate, request.ToDate);

        var report = await _reportingService.GetTimeTrackingReportAsync(
            request.FromDate, 
            request.ToDate, 
            request.ProjectId, 
            request.UserId,
            cancellationToken);
        
        return report;
    }
}

/// <summary>
/// Query to get resource utilization report
/// </summary>
public record GetResourceUtilizationReportQuery(
    DateOnly FromDate,
    DateOnly ToDate
) : IRequest<ResourceUtilizationReportDto>;

/// <summary>
/// Handler for GetResourceUtilizationReportQuery
/// </summary>
public class GetResourceUtilizationReportQueryHandler : IRequestHandler<GetResourceUtilizationReportQuery, ResourceUtilizationReportDto>
{
    private readonly IProjectReportingService _reportingService;
    private readonly ILogger<GetResourceUtilizationReportQueryHandler> _logger;

    public GetResourceUtilizationReportQueryHandler(
        IProjectReportingService reportingService,
        ILogger<GetResourceUtilizationReportQueryHandler> logger)
    {
        _reportingService = reportingService;
        _logger = logger;
    }

    public async Task<ResourceUtilizationReportDto> Handle(GetResourceUtilizationReportQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting resource utilization report from {FromDate} to {ToDate}", 
            request.FromDate, request.ToDate);

        var report = await _reportingService.GetResourceUtilizationReportAsync(
            request.FromDate, 
            request.ToDate,
            cancellationToken);
        
        return report;
    }
}

/// <summary>
/// Query to get project performance report
/// </summary>
public record GetProjectPerformanceReportQuery(
    DateOnly FromDate,
    DateOnly ToDate
) : IRequest<ProjectPerformanceReportDto>;

/// <summary>
/// Handler for GetProjectPerformanceReportQuery
/// </summary>
public class GetProjectPerformanceReportQueryHandler : IRequestHandler<GetProjectPerformanceReportQuery, ProjectPerformanceReportDto>
{
    private readonly IProjectReportingService _reportingService;
    private readonly ILogger<GetProjectPerformanceReportQueryHandler> _logger;

    public GetProjectPerformanceReportQueryHandler(
        IProjectReportingService reportingService,
        ILogger<GetProjectPerformanceReportQueryHandler> logger)
    {
        _reportingService = reportingService;
        _logger = logger;
    }

    public async Task<ProjectPerformanceReportDto> Handle(GetProjectPerformanceReportQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting project performance report from {FromDate} to {ToDate}", 
            request.FromDate, request.ToDate);

        var report = await _reportingService.GetProjectPerformanceReportAsync(
            request.FromDate, 
            request.ToDate,
            cancellationToken);
        
        return report;
    }
}

/// <summary>
/// Query to get project dashboard summary
/// </summary>
public record GetProjectDashboardQuery(
    Guid? ProjectManagerId = null,
    Guid? CustomerId = null
) : IRequest<ProjectDashboardDto>;

/// <summary>
/// Project dashboard DTO
/// </summary>
public class ProjectDashboardDto
{
    public int TotalProjects { get; set; }
    public int ActiveProjects { get; set; }
    public int CompletedProjects { get; set; }
    public int OverdueProjects { get; set; }
    public int TotalTasks { get; set; }
    public int OverdueTasks { get; set; }
    public int TasksDueToday { get; set; }
    public int TasksDueThisWeek { get; set; }
    public decimal TotalBudget { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal TotalHours { get; set; }
    public decimal BillableHours { get; set; }
    public string Currency { get; set; } = "USD";
    public List<ProjectSummaryDto> RecentProjects { get; set; } = new();
    public List<ProjectTaskDto> UpcomingTasks { get; set; } = new();
    public List<MilestoneDto> UpcomingMilestones { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// Handler for GetProjectDashboardQuery
/// </summary>
public class GetProjectDashboardQueryHandler : IRequestHandler<GetProjectDashboardQuery, ProjectDashboardDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectTaskRepository _taskRepository;
    private readonly IMilestoneRepository _milestoneRepository;
    private readonly ITimeEntryRepository _timeEntryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProjectDashboardQueryHandler> _logger;

    public GetProjectDashboardQueryHandler(
        IProjectRepository projectRepository,
        IProjectTaskRepository taskRepository,
        IMilestoneRepository milestoneRepository,
        ITimeEntryRepository timeEntryRepository,
        IMapper mapper,
        ILogger<GetProjectDashboardQueryHandler> logger)
    {
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
        _milestoneRepository = milestoneRepository;
        _timeEntryRepository = timeEntryRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProjectDashboardDto> Handle(GetProjectDashboardQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting project dashboard");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var weekFromNow = today.AddDays(7);

        // Get all projects (filtered if needed)
        var allProjects = await _projectRepository.GetProjectsAsync(1, int.MaxValue, null, null, 
            request.CustomerId, request.ProjectManagerId, cancellationToken);

        // Get all tasks
        var allTasks = await _taskRepository.GetTasksAsync(1, int.MaxValue, null, null, null, null, null, cancellationToken);

        // Get overdue projects and tasks
        var overdueProjects = await _projectRepository.GetOverdueProjectsAsync(cancellationToken);
        var overdueTasks = await _taskRepository.GetOverdueTasksAsync(cancellationToken);

        // Get tasks due today and this week
        var tasksDueToday = await _taskRepository.GetTasksDueAsync(today, cancellationToken);
        var tasksDueThisWeek = allTasks.Items.Where(t => 
            t.DueDate.HasValue && t.DueDate >= today && t.DueDate <= weekFromNow);

        // Get upcoming milestones
        var upcomingMilestones = await _milestoneRepository.GetUpcomingMilestonesAsync(weekFromNow, cancellationToken);

        // Get time tracking data
        var totalHours = await _timeEntryRepository.GetTotalHoursAsync(null, null, null, null, null, cancellationToken);

        // Calculate totals
        var totalBudget = allProjects.Items.Sum(p => p.Budget.Amount);
        var totalSpent = allProjects.Items.Sum(p => p.ActualCost);

        var dashboard = new ProjectDashboardDto
        {
            TotalProjects = allProjects.TotalCount,
            ActiveProjects = allProjects.Items.Count(p => p.Status == ProjectStatus.Active),
            CompletedProjects = allProjects.Items.Count(p => p.Status == ProjectStatus.Completed),
            OverdueProjects = overdueProjects.Count(),
            TotalTasks = allTasks.TotalCount,
            OverdueTasks = overdueTasks.Count(),
            TasksDueToday = tasksDueToday.Count(),
            TasksDueThisWeek = tasksDueThisWeek.Count(),
            TotalBudget = totalBudget,
            TotalSpent = totalSpent,
            TotalHours = totalHours,
            BillableHours = 0, // Calculate based on billable time entries
            Currency = "USD", // Could be configurable
            RecentProjects = _mapper.Map<List<ProjectSummaryDto>>(
                allProjects.Items.OrderByDescending(p => p.CreatedAt).Take(5)),
            UpcomingTasks = _mapper.Map<List<ProjectTaskDto>>(
                tasksDueThisWeek.OrderBy(t => t.DueDate).Take(10)),
            UpcomingMilestones = _mapper.Map<List<MilestoneDto>>(
                upcomingMilestones.OrderBy(m => m.DueDate).Take(5)),
            LastUpdated = DateTime.UtcNow
        };

        return dashboard;
    }
}

/// <summary>
/// Query to get team workload report
/// </summary>
public record GetTeamWorkloadReportQuery(
    DateOnly FromDate,
    DateOnly ToDate,
    List<Guid>? TeamMemberIds = null
) : IRequest<TeamWorkloadReportDto>;

/// <summary>
/// Team workload report DTO
/// </summary>
public class TeamWorkloadReportDto
{
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public List<TeamMemberWorkloadDto> TeamMembers { get; set; } = new();
    public decimal AverageUtilization { get; set; }
    public DateTime ReportDate { get; set; }
}

/// <summary>
/// Team member workload DTO
/// </summary>
public class TeamMemberWorkloadDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int AssignedTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int OverdueTasks { get; set; }
    public decimal TotalHours { get; set; }
    public decimal BillableHours { get; set; }
    public decimal UtilizationPercentage { get; set; }
    public List<ProjectWorkloadDto> ProjectWorkload { get; set; } = new();
}

/// <summary>
/// Project workload DTO
/// </summary>
public class ProjectWorkloadDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int AssignedTasks { get; set; }
    public decimal Hours { get; set; }
    public decimal AllocationPercentage { get; set; }
}

/// <summary>
/// Handler for GetTeamWorkloadReportQuery
/// </summary>
public class GetTeamWorkloadReportQueryHandler : IRequestHandler<GetTeamWorkloadReportQuery, TeamWorkloadReportDto>
{
    private readonly IProjectTaskRepository _taskRepository;
    private readonly ITimeEntryRepository _timeEntryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTeamWorkloadReportQueryHandler> _logger;

    public GetTeamWorkloadReportQueryHandler(
        IProjectTaskRepository taskRepository,
        ITimeEntryRepository timeEntryRepository,
        IMapper mapper,
        ILogger<GetTeamWorkloadReportQueryHandler> logger)
    {
        _taskRepository = taskRepository;
        _timeEntryRepository = timeEntryRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<TeamWorkloadReportDto> Handle(GetTeamWorkloadReportQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting team workload report from {FromDate} to {ToDate}", 
            request.FromDate, request.ToDate);

        var teamMembers = new List<TeamMemberWorkloadDto>();

        // Get team member IDs (if not provided, get all users with tasks/time entries)
        var teamMemberIds = request.TeamMemberIds ?? new List<Guid>();
        
        if (!teamMemberIds.Any())
        {
            // Get all users who have tasks or time entries in the date range
            var timeEntries = await _timeEntryRepository.GetByDateRangeAsync(request.FromDate, request.ToDate, cancellationToken);
            teamMemberIds = timeEntries.Select(te => te.UserId).Distinct().ToList();
        }

        foreach (var userId in teamMemberIds)
        {
            // Get user's tasks
            var userTasks = await _taskRepository.GetByAssigneeIdAsync(userId, cancellationToken);
            var tasksInRange = userTasks.Where(t => 
                (t.StartDate.HasValue && t.StartDate >= request.FromDate && t.StartDate <= request.ToDate) ||
                (t.DueDate.HasValue && t.DueDate >= request.FromDate && t.DueDate <= request.ToDate));

            // Get user's time entries
            var userTimeEntries = await _timeEntryRepository.GetByUserIdAsync(userId, cancellationToken);
            var timeEntriesInRange = userTimeEntries.Where(te => 
                te.Date >= request.FromDate && te.Date <= request.ToDate);

            // Calculate workload
            var totalHours = timeEntriesInRange.Sum(te => te.Hours);
            var billableHours = timeEntriesInRange.Where(te => te.IsBillable).Sum(te => te.Hours);

            // Group by project
            var projectWorkload = timeEntriesInRange
                .GroupBy(te => te.ProjectId)
                .Select(g => new ProjectWorkloadDto
                {
                    ProjectId = g.Key,
                    ProjectName = "", // Would need to join with project data
                    AssignedTasks = tasksInRange.Count(t => t.ProjectId == g.Key),
                    Hours = g.Sum(te => te.Hours),
                    AllocationPercentage = totalHours > 0 ? (g.Sum(te => te.Hours) / totalHours) * 100 : 0
                }).ToList();

            var teamMember = new TeamMemberWorkloadDto
            {
                UserId = userId,
                UserName = "", // Would need to join with user data
                AssignedTasks = tasksInRange.Count(),
                CompletedTasks = tasksInRange.Count(t => t.Status == TaskStatus.Completed),
                OverdueTasks = tasksInRange.Count(t => t.DueDate.HasValue && 
                    t.DueDate < DateOnly.FromDateTime(DateTime.UtcNow) && 
                    t.Status != TaskStatus.Completed),
                TotalHours = totalHours,
                BillableHours = billableHours,
                UtilizationPercentage = 0, // Would need to calculate based on available hours
                ProjectWorkload = projectWorkload
            };

            teamMembers.Add(teamMember);
        }

        var report = new TeamWorkloadReportDto
        {
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            TeamMembers = teamMembers,
            AverageUtilization = teamMembers.Any() ? teamMembers.Average(tm => tm.UtilizationPercentage) : 0,
            ReportDate = DateTime.UtcNow
        };

        return report;
    }
}
