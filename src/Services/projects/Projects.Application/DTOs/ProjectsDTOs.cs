namespace TossErp.Projects.Application.DTOs;

/// <summary>
/// Project DTO
/// </summary>
public class ProjectDto
{
    public Guid Id { get; set; }
    public string ProjectNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ProjectStatus Status { get; set; }
    public ProjectPriority Priority { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid ProjectManagerId { get; set; }
    public string ProjectManagerName { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateOnly? ActualEndDate { get; set; }
    public decimal Budget { get; set; }
    public decimal ActualCost { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int ProgressPercentage { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public decimal TotalHours { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<ProjectTaskDto> Tasks { get; set; } = new();
    public List<MilestoneDto> Milestones { get; set; } = new();
}

/// <summary>
/// Project summary DTO
/// </summary>
public class ProjectSummaryDto
{
    public Guid Id { get; set; }
    public string ProjectNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ProjectStatus Status { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string ProjectManagerName { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal Budget { get; set; }
    public decimal ActualCost { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int ProgressPercentage { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int OverdueTasks { get; set; }
    public decimal TotalHours { get; set; }
    public decimal BillableHours { get; set; }
    public bool IsOverdue { get; set; }
    public bool IsOverBudget { get; set; }
    public int DaysUntilDeadline { get; set; }
}

/// <summary>
/// Project task DTO
/// </summary>
public class ProjectTaskDto
{
    public Guid Id { get; set; }
    public string TaskNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public Guid? AssigneeId { get; set; }
    public string AssigneeName { get; set; } = string.Empty;
    public Guid? ParentTaskId { get; set; }
    public string ParentTaskTitle { get; set; } = string.Empty;
    public DateOnly? StartDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public DateOnly? CompletedDate { get; set; }
    public decimal EstimatedHours { get; set; }
    public decimal ActualHours { get; set; }
    public int ProgressPercentage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<ProjectTaskDto> SubTasks { get; set; } = new();
    public List<TimeEntryDto> TimeEntries { get; set; } = new();
    public bool IsOverdue => DueDate.HasValue && DueDate < DateOnly.FromDateTime(DateTime.UtcNow) && Status != TaskStatus.Completed;
    public int? DaysUntilDue => DueDate?.DayNumber - DateOnly.FromDateTime(DateTime.UtcNow).DayNumber;
}

/// <summary>
/// Time entry DTO
/// </summary>
public class TimeEntryDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public Guid? TaskId { get; set; }
    public string TaskTitle { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public decimal Hours { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsBillable { get; set; }
    public decimal? HourlyRate { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public decimal? Cost => IsBillable && HourlyRate.HasValue ? Hours * HourlyRate.Value : null;
}

/// <summary>
/// Resource DTO
/// </summary>
public class ResourceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ResourceType Type { get; set; }
    public bool IsAvailable { get; set; }
    public decimal? CostPerHour { get; set; }
    public decimal? CostPerDay { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string ContactInfo { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<Guid> AssignedProjects { get; set; } = new();
}

/// <summary>
/// Milestone DTO
/// </summary>
public class MilestoneDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public DateOnly DueDate { get; set; }
    public DateOnly? CompletedDate { get; set; }
    public MilestoneStatus Status { get; set; }
    public int ProgressPercentage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<Guid> DependentTaskIds { get; set; } = new();
    public bool IsOverdue => DueDate < DateOnly.FromDateTime(DateTime.UtcNow) && Status != MilestoneStatus.Completed;
    public int DaysUntilDue => DueDate.DayNumber - DateOnly.FromDateTime(DateTime.UtcNow).DayNumber;
}

/// <summary>
/// Project template DTO
/// </summary>
public class ProjectTemplateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<TaskTemplateDto> TaskTemplates { get; set; } = new();
    public List<MilestoneTemplateDto> MilestoneTemplates { get; set; } = new();
}

/// <summary>
/// Task template DTO
/// </summary>
public class TaskTemplateDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskPriority Priority { get; set; }
    public decimal EstimatedHours { get; set; }
    public int DaysFromStart { get; set; }
    public List<string> Dependencies { get; set; } = new();
}

/// <summary>
/// Milestone template DTO
/// </summary>
public class MilestoneTemplateDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DaysFromStart { get; set; }
    public List<string> DependentTasks { get; set; } = new();
}

/// <summary>
/// Project progress report DTO
/// </summary>
public class ProjectProgressReportDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int OverallProgress { get; set; }
    public int TasksTotal { get; set; }
    public int TasksCompleted { get; set; }
    public int TasksInProgress { get; set; }
    public int TasksNotStarted { get; set; }
    public int TasksOverdue { get; set; }
    public decimal BudgetTotal { get; set; }
    public decimal BudgetSpent { get; set; }
    public decimal BudgetRemaining { get; set; }
    public decimal HoursEstimated { get; set; }
    public decimal HoursActual { get; set; }
    public decimal HoursRemaining { get; set; }
    public List<MilestoneProgressDto> Milestones { get; set; } = new();
    public List<TaskProgressDto> CriticalTasks { get; set; } = new();
    public string Currency { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; }
}

/// <summary>
/// Milestone progress DTO
/// </summary>
public class MilestoneProgressDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly DueDate { get; set; }
    public MilestoneStatus Status { get; set; }
    public int ProgressPercentage { get; set; }
    public bool IsOverdue { get; set; }
    public int DaysUntilDue { get; set; }
}

/// <summary>
/// Task progress DTO
/// </summary>
public class TaskProgressDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public string AssigneeName { get; set; } = string.Empty;
    public DateOnly? DueDate { get; set; }
    public int ProgressPercentage { get; set; }
    public bool IsOverdue { get; set; }
    public int? DaysUntilDue { get; set; }
}

/// <summary>
/// Time tracking report DTO
/// </summary>
public class TimeTrackingReportDto
{
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public decimal TotalHours { get; set; }
    public decimal BillableHours { get; set; }
    public decimal NonBillableHours { get; set; }
    public decimal TotalCost { get; set; }
    public string Currency { get; set; } = string.Empty;
    public List<ProjectTimeReportDto> ProjectBreakdown { get; set; } = new();
    public List<UserTimeReportDto> UserBreakdown { get; set; } = new();
    public List<DailyTimeReportDto> DailyBreakdown { get; set; } = new();
}

/// <summary>
/// Project time report DTO
/// </summary>
public class ProjectTimeReportDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public decimal Hours { get; set; }
    public decimal BillableHours { get; set; }
    public decimal Cost { get; set; }
    public string Currency { get; set; } = string.Empty;
}

/// <summary>
/// User time report DTO
/// </summary>
public class UserTimeReportDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public decimal Hours { get; set; }
    public decimal BillableHours { get; set; }
    public decimal Cost { get; set; }
    public string Currency { get; set; } = string.Empty;
}

/// <summary>
/// Daily time report DTO
/// </summary>
public class DailyTimeReportDto
{
    public DateOnly Date { get; set; }
    public decimal Hours { get; set; }
    public decimal BillableHours { get; set; }
    public decimal Cost { get; set; }
    public string Currency { get; set; } = string.Empty;
}

/// <summary>
/// Resource utilization report DTO
/// </summary>
public class ResourceUtilizationReportDto
{
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public List<ResourceUtilizationDto> Resources { get; set; } = new();
    public decimal AverageUtilization { get; set; }
    public DateTime ReportDate { get; set; }
}

/// <summary>
/// Resource utilization DTO
/// </summary>
public class ResourceUtilizationDto
{
    public Guid ResourceId { get; set; }
    public string ResourceName { get; set; } = string.Empty;
    public ResourceType Type { get; set; }
    public decimal TotalAvailableHours { get; set; }
    public decimal TotalAllocatedHours { get; set; }
    public decimal UtilizationPercentage { get; set; }
    public decimal TotalCost { get; set; }
    public string Currency { get; set; } = string.Empty;
    public List<ProjectAllocationDto> ProjectAllocations { get; set; } = new();
}

/// <summary>
/// Project allocation DTO
/// </summary>
public class ProjectAllocationDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public decimal AllocatedHours { get; set; }
    public decimal Cost { get; set; }
    public string Currency { get; set; } = string.Empty;
}

/// <summary>
/// Project performance report DTO
/// </summary>
public class ProjectPerformanceReportDto
{
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public int TotalProjects { get; set; }
    public int CompletedProjects { get; set; }
    public int OnTimeProjects { get; set; }
    public int OverdueProjects { get; set; }
    public int OnBudgetProjects { get; set; }
    public int OverBudgetProjects { get; set; }
    public decimal AverageProjectDuration { get; set; }
    public decimal AverageBudgetVariance { get; set; }
    public decimal AverageScheduleVariance { get; set; }
    public List<ProjectPerformanceDto> ProjectDetails { get; set; } = new();
    public DateTime ReportDate { get; set; }
}

/// <summary>
/// Project performance DTO
/// </summary>
public class ProjectPerformanceDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public ProjectStatus Status { get; set; }
    public decimal BudgetVariance { get; set; }
    public int ScheduleVariance { get; set; }
    public decimal PerformanceIndex { get; set; }
    public bool IsOnTime { get; set; }
    public bool IsOnBudget { get; set; }
    public string Currency { get; set; } = string.Empty;
}
