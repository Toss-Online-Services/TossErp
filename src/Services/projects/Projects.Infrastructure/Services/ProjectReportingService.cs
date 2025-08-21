using TossErp.Projects.Infrastructure.Data;

namespace TossErp.Projects.Infrastructure.Services;

/// <summary>
/// Service implementation for project reporting
/// </summary>
public class ProjectReportingService : IProjectReportingService
{
    private readonly ProjectsDbContext _context;

    public ProjectReportingService(ProjectsDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ProjectSummaryDto> GetProjectSummaryAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var project = await _context.Projects
            .Include(p => p.Tasks)
            .Include(p => p.Milestones)
            .Include(p => p.TimeEntries)
            .Include(p => p.Issues)
            .Include(p => p.Risks)
            .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);

        if (project == null)
            throw new InvalidOperationException($"Project with ID {projectId} not found");

        return new ProjectSummaryDto
        {
            Id = project.Id,
            ProjectCode = project.ProjectCode.Value,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status.ToString(),
            Type = project.Type.ToString(),
            Priority = project.Priority.Level,
            Budget = project.Budget.Amount,
            Currency = project.Budget.Currency,
            ActualCost = project.ActualCost.Amount,
            BudgetUtilizationPercentage = project.BudgetUtilizationPercentage,
            OverallProgress = project.OverallProgress.Percentage,
            ProjectManagerId = project.ProjectManagerId,
            ProjectManagerName = project.ProjectManagerName,
            ClientId = project.ClientId,
            ClientName = project.ClientName,
            StartDate = project.Timeline?.StartDate,
            EndDate = project.Timeline?.EndDate,
            CreatedAt = project.CreatedAt,
            TasksTotal = project.Tasks.Count,
            TasksCompleted = project.TasksCompleted,
            TasksInProgress = project.TasksInProgress,
            TasksNotStarted = project.TasksNotStarted,
            MilestonesTotal = project.Milestones.Count,
            MilestonesCompleted = project.MilestonesCompleted,
            ActiveIssues = project.ActiveIssues,
            HighRisks = project.HighRisks,
            TotalTimeLogged = project.TotalTimeLogged.ToHours(),
            TotalBillableAmount = project.TotalBillableTime.Amount,
            IsOverBudget = project.IsOverBudget,
            IsOverdue = project.IsOverdue
        };
    }

    public async Task<ProjectProgressReportDto> GetProjectProgressReportAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var project = await _context.Projects
            .Include(p => p.Tasks)
            .Include(p => p.Milestones)
            .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);

        if (project == null)
            throw new InvalidOperationException($"Project with ID {projectId} not found");

        var taskBreakdown = project.Tasks
            .GroupBy(t => t.Status)
            .ToDictionary(g => g.Key.ToString(), g => g.Count());

        var milestoneBreakdown = project.Milestones
            .GroupBy(m => m.Status)
            .ToDictionary(g => g.Key.ToString(), g => g.Count());

        return new ProjectProgressReportDto
        {
            ProjectId = project.Id,
            ProjectName = project.Name,
            OverallProgress = project.OverallProgress.Percentage,
            TaskBreakdown = taskBreakdown,
            MilestoneBreakdown = milestoneBreakdown,
            CompletedTasks = project.TasksCompleted,
            TotalTasks = project.Tasks.Count,
            CompletedMilestones = project.MilestonesCompleted,
            TotalMilestones = project.Milestones.Count,
            ReportDate = DateTime.UtcNow
        };
    }

    public async Task<TimeTrackingReportDto> GetTimeTrackingReportAsync(
        DateOnly fromDate, 
        DateOnly toDate, 
        Guid? projectId = null, 
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        var from = fromDate.ToDateTime(TimeOnly.MinValue);
        var to = toDate.ToDateTime(TimeOnly.MaxValue);

        var query = _context.TimeEntries.AsQueryable();

        if (projectId.HasValue)
        {
            query = query.Where(te => te.ProjectId == projectId.Value);
        }

        if (userId.HasValue)
        {
            query = query.Where(te => te.UserId == userId.Value);
        }

        var timeEntries = await query
            .Where(te => te.Date >= from && te.Date <= to)
            .ToListAsync(cancellationToken);

        var totalHours = timeEntries.Sum(te => te.Duration.ToHours());
        var billableHours = timeEntries.Where(te => te.IsBillable).Sum(te => te.Duration.ToHours());
        var totalBillableAmount = timeEntries.Where(te => te.IsBillable && te.BillableAmount != null)
                                           .Sum(te => te.BillableAmount!.Amount);

        var dailyBreakdown = timeEntries
            .GroupBy(te => te.Date.Date)
            .ToDictionary(g => DateOnly.FromDateTime(g.Key), g => g.Sum(te => te.Duration.ToHours()));

        var userBreakdown = timeEntries
            .GroupBy(te => new { te.UserId, te.UserName })
            .ToDictionary(g => g.Key.UserName, g => g.Sum(te => te.Duration.ToHours()));

        return new TimeTrackingReportDto
        {
            FromDate = fromDate,
            ToDate = toDate,
            ProjectId = projectId,
            UserId = userId,
            TotalHours = totalHours,
            BillableHours = billableHours,
            NonBillableHours = totalHours - billableHours,
            TotalBillableAmount = totalBillableAmount,
            AverageHoursPerDay = totalHours / (toDate.DayNumber - fromDate.DayNumber + 1),
            DailyBreakdown = dailyBreakdown,
            UserBreakdown = userBreakdown,
            ReportDate = DateTime.UtcNow
        };
    }

    public async Task<ResourceUtilizationReportDto> GetResourceUtilizationReportAsync(
        DateOnly fromDate, 
        DateOnly toDate,
        CancellationToken cancellationToken = default)
    {
        var from = fromDate.ToDateTime(TimeOnly.MinValue);
        var to = toDate.ToDateTime(TimeOnly.MaxValue);

        var allocations = await _context.ResourceAllocations
            .Where(ra => ra.AllocationPeriod.StartDate <= to && ra.AllocationPeriod.EndDate >= from)
            .ToListAsync(cancellationToken);

        var utilizationByResource = allocations
            .GroupBy(ra => new { ra.ResourceId, ra.ResourceName, ra.ResourceType })
            .Select(g => new ResourceUtilizationDto
            {
                ResourceId = g.Key.ResourceId,
                ResourceName = g.Key.ResourceName,
                ResourceType = g.Key.ResourceType.ToString(),
                TotalAllocatedPercentage = g.Sum(ra => ra.AllocationPercentage),
                ProjectCount = g.Count(),
                AverageAllocation = g.Average(ra => ra.AllocationPercentage),
                IsOverAllocated = g.Sum(ra => ra.AllocationPercentage) > 100
            })
            .OrderByDescending(r => r.TotalAllocatedPercentage)
            .ToList();

        var typeBreakdown = allocations
            .GroupBy(ra => ra.ResourceType)
            .ToDictionary(g => g.Key.ToString(), g => new
            {
                Count = g.Select(ra => ra.ResourceId).Distinct().Count(),
                TotalAllocation = g.Sum(ra => ra.AllocationPercentage),
                AverageAllocation = g.Average(ra => ra.AllocationPercentage)
            });

        return new ResourceUtilizationReportDto
        {
            FromDate = fromDate,
            ToDate = toDate,
            ResourceUtilizations = utilizationByResource,
            TotalResources = utilizationByResource.Count,
            OverAllocatedResources = utilizationByResource.Count(r => r.IsOverAllocated),
            AverageUtilization = utilizationByResource.Average(r => r.AverageAllocation),
            TypeBreakdown = typeBreakdown.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.AverageAllocation),
            ReportDate = DateTime.UtcNow
        };
    }

    public async Task<ProjectPerformanceReportDto> GetProjectPerformanceReportAsync(
        DateOnly fromDate, 
        DateOnly toDate,
        CancellationToken cancellationToken = default)
    {
        var from = fromDate.ToDateTime(TimeOnly.MinValue);
        var to = toDate.ToDateTime(TimeOnly.MaxValue);

        var projects = await _context.Projects
            .Include(p => p.Tasks)
            .Include(p => p.Milestones)
            .Include(p => p.TimeEntries)
            .Where(p => p.CreatedAt >= from && p.CreatedAt <= to)
            .ToListAsync(cancellationToken);

        var performances = projects.Select(p => new ProjectPerformanceDto
        {
            ProjectId = p.Id,
            ProjectCode = p.ProjectCode.Value,
            ProjectName = p.Name,
            Status = p.Status.ToString(),
            Progress = p.OverallProgress.Percentage,
            BudgetUtilization = p.BudgetUtilizationPercentage,
            IsOverBudget = p.IsOverBudget,
            IsOverdue = p.IsOverdue,
            TaskCompletionRate = p.Tasks.Any() ? (decimal)p.TasksCompleted / p.Tasks.Count * 100 : 0,
            MilestoneCompletionRate = p.Milestones.Any() ? (decimal)p.MilestonesCompleted / p.Milestones.Count * 100 : 0,
            TotalTimeLogged = p.TotalTimeLogged.ToHours(),
            ActiveIssuesCount = p.ActiveIssues,
            HighRisksCount = p.HighRisks
        }).ToList();

        return new ProjectPerformanceReportDto
        {
            FromDate = fromDate,
            ToDate = toDate,
            ProjectPerformances = performances,
            TotalProjects = performances.Count,
            CompletedProjects = performances.Count(p => p.Status == ProjectStatus.Completed.ToString()),
            OverdueProjects = performances.Count(p => p.IsOverdue),
            OverBudgetProjects = performances.Count(p => p.IsOverBudget),
            AverageProgress = performances.Average(p => p.Progress),
            AverageBudgetUtilization = performances.Average(p => p.BudgetUtilization),
            ReportDate = DateTime.UtcNow
        };
    }
}
