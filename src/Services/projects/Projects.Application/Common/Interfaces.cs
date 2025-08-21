namespace TossErp.Projects.Application.Common.Interfaces;

/// <summary>
/// Repository interface for Project entities
/// </summary>
public interface IProjectRepository : IRepository<Project>
{
    Task<IEnumerable<Project>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetByManagerIdAsync(Guid managerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetByStatusAsync(ProjectStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetProjectsWithTasksAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetOverdueProjectsAsync(CancellationToken cancellationToken = default);
    Task<PaginatedResult<Project>> GetProjectsAsync(
        int pageNumber, 
        int pageSize, 
        string? searchTerm = null,
        ProjectStatus? status = null,
        Guid? customerId = null,
        Guid? managerId = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for ProjectTask entities
/// </summary>
public interface IProjectTaskRepository : IRepository<ProjectTask>
{
    Task<IEnumerable<ProjectTask>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectTask>> GetByAssigneeIdAsync(Guid assigneeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectTask>> GetByStatusAsync(TaskStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectTask>> GetOverdueTasksAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectTask>> GetTasksDueAsync(DateOnly dueDate, CancellationToken cancellationToken = default);
    Task<PaginatedResult<ProjectTask>> GetTasksAsync(
        int pageNumber, 
        int pageSize, 
        string? searchTerm = null,
        TaskStatus? status = null,
        Guid? projectId = null,
        Guid? assigneeId = null,
        TaskPriority? priority = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for TimeEntry entities
/// </summary>
public interface ITimeEntryRepository : IRepository<TimeEntry>
{
    Task<IEnumerable<TimeEntry>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TimeEntry>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TimeEntry>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TimeEntry>> GetByDateRangeAsync(DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken = default);
    Task<PaginatedResult<TimeEntry>> GetTimeEntriesAsync(
        int pageNumber, 
        int pageSize, 
        Guid? projectId = null,
        Guid? taskId = null,
        Guid? userId = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        CancellationToken cancellationToken = default);
    Task<decimal> GetTotalHoursAsync(
        Guid? projectId = null,
        Guid? taskId = null,
        Guid? userId = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for Resource entities
/// </summary>
public interface IResourceRepository : IRepository<ResourceAllocation>
{
    Task<IEnumerable<ResourceAllocation>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ResourceAllocation>> GetByTypeAsync(ResourceType type, CancellationToken cancellationToken = default);
    Task<IEnumerable<ResourceAllocation>> GetAvailableResourcesAsync(DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken = default);
    Task<PaginatedResult<ResourceAllocation>> GetResourcesAsync(
        int pageNumber, 
        int pageSize, 
        string? searchTerm = null,
        ResourceType? type = null,
        bool? isAvailable = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for Milestone entities
/// </summary>
public interface IMilestoneRepository : IRepository<ProjectMilestone>
{
    Task<IEnumerable<ProjectMilestone>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectMilestone>> GetUpcomingMilestonesAsync(DateOnly? beforeDate = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectMilestone>> GetOverdueMilestonesAsync(CancellationToken cancellationToken = default);
    Task<PaginatedResult<ProjectMilestone>> GetMilestonesAsync(
        int pageNumber, 
        int pageSize, 
        Guid? projectId = null,
        MilestoneStatus? status = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for project reporting
/// </summary>
public interface IProjectReportingService
{
    Task<ProjectSummaryDto> GetProjectSummaryAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<ProjectProgressReportDto> GetProjectProgressReportAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<TimeTrackingReportDto> GetTimeTrackingReportAsync(
        DateOnly fromDate, 
        DateOnly toDate, 
        Guid? projectId = null, 
        Guid? userId = null,
        CancellationToken cancellationToken = default);
    Task<ResourceUtilizationReportDto> GetResourceUtilizationReportAsync(
        DateOnly fromDate, 
        DateOnly toDate,
        CancellationToken cancellationToken = default);
    Task<ProjectPerformanceReportDto> GetProjectPerformanceReportAsync(
        DateOnly fromDate, 
        DateOnly toDate,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for project notifications
/// </summary>
public interface IProjectNotificationService
{
    Task SendProjectCreatedNotificationAsync(Project project, CancellationToken cancellationToken = default);
    Task SendProjectStatusChangedNotificationAsync(Project project, ProjectStatus oldStatus, CancellationToken cancellationToken = default);
    Task SendTaskAssignedNotificationAsync(ProjectTask task, CancellationToken cancellationToken = default);
    Task SendTaskDueNotificationAsync(ProjectTask task, CancellationToken cancellationToken = default);
    Task SendMilestoneReachedNotificationAsync(Milestone milestone, CancellationToken cancellationToken = default);
    Task SendProjectOverdueNotificationAsync(Project project, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for file management
/// </summary>
public interface IFileService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<Stream> DownloadFileAsync(string fileUrl, CancellationToken cancellationToken = default);
    Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default);
    Task<bool> FileExistsAsync(string fileUrl, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for project templates
/// </summary>
public interface IProjectTemplateService
{
    Task<Project> CreateProjectFromTemplateAsync(Guid templateId, CreateProjectFromTemplateRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectTemplateDto>> GetAvailableTemplatesAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Request for creating project from template
/// </summary>
public class CreateProjectFromTemplateRequest
{
    public string ProjectName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public Guid ProjectManagerId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal Budget { get; set; }
    public string Currency { get; set; } = "USD";
}
