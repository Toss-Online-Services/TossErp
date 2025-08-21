using Microsoft.Extensions.Hosting;
using Projects.Domain.Aggregates;
using Projects.Domain.Entities;
using Projects.Domain.Enums;

namespace Projects.Application.Common.Interfaces;

/// <summary>
/// Base repository interface for CRUD operations
/// </summary>
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}

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
    Task<IEnumerable<Project>> GetActiveProjectsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetCompletedProjectsOlderThanAsync(DateTime cutoffDate, CancellationToken cancellationToken = default);
    Task<int> GetProjectCountAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectTeamMember>> GetProjectTeamMembersAsync(Guid projectId, CancellationToken cancellationToken = default);
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
public interface ITaskRepository : IRepository<ProjectTask>
{
    Task<IEnumerable<ProjectTask>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectTask>> GetTasksByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectTask>> GetByAssigneeIdAsync(Guid assigneeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectTask>> GetByStatusAsync(TaskStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectTask>> GetOverdueTasksAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectTask>> GetTasksDueAsync(DateOnly dueDate, CancellationToken cancellationToken = default);
    Task<int> GetTaskCountAsync(CancellationToken cancellationToken = default);
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
    Task<IEnumerable<TimeEntry>> GetOverdueTimeEntriesAsync(DateTime overdueThreshold, CancellationToken cancellationToken = default);
    Task<bool> HasTimeEntryForDateAsync(string userId, Guid projectId, DateTime date, CancellationToken cancellationToken = default);
    Task<double> GetTotalHoursForPeriodAsync(string userId, Guid projectId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<int> DeleteOldDraftEntriesAsync(DateTime cutoffDate, CancellationToken cancellationToken = default);
    Task<int> GetTimeEntryCountAsync(CancellationToken cancellationToken = default);
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
/// Repository interface for Document entities
/// </summary>
public interface IDocumentRepository : IRepository<Document>
{
    Task<IEnumerable<Document>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Document>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Document>> GetByTypeAsync(DocumentType type, CancellationToken cancellationToken = default);
}

/// <summary>
/// Unit of Work pattern interface for managing transactions
/// </summary>
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    Task TestConnectionAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for getting current user information
/// </summary>
public interface ICurrentUserService
{
    string? UserId { get; }
    string? Email { get; }
    string? TenantId { get; }
    bool IsAuthenticated { get; }
}

/// <summary>
/// Service interface for project management operations
/// </summary>
public interface IProjectService
{
    Task<Project> CreateProjectAsync(CreateProjectRequest request, CancellationToken cancellationToken = default);
    Task<Project> UpdateProjectAsync(Guid id, UpdateProjectRequest request, CancellationToken cancellationToken = default);
    Task DeleteProjectAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Project?> GetProjectByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetProjectsAsync(ProjectFilterRequest? filter = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for task management operations
/// </summary>
public interface ITaskService
{
    Task<ProjectTask> CreateTaskAsync(CreateTaskRequest request, CancellationToken cancellationToken = default);
    Task<ProjectTask> UpdateTaskAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default);
    Task DeleteTaskAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProjectTask?> GetTaskByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectTask>> GetTasksAsync(TaskFilterRequest? filter = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for time tracking operations
/// </summary>
public interface ITimeTrackingService
{
    Task<TimeEntry> StartTimeTrackingAsync(StartTimeTrackingRequest request, CancellationToken cancellationToken = default);
    Task<TimeEntry> StopTimeTrackingAsync(Guid timeEntryId, CancellationToken cancellationToken = default);
    Task<TimeEntry> CreateTimeEntryAsync(CreateTimeEntryRequest request, CancellationToken cancellationToken = default);
    Task<TimeEntry> UpdateTimeEntryAsync(Guid id, UpdateTimeEntryRequest request, CancellationToken cancellationToken = default);
    Task DeleteTimeEntryAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TimeEntry?> GetTimeEntryByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TimeEntry>> GetTimeEntriesAsync(TimeEntryFilterRequest? filter = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for project reporting
/// </summary>
public interface IReportingService
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

// Additional types that may be referenced
public class ProjectTeamMember
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

public class Document
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? TaskId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DocumentType Type { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}

public class Milestone
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public MilestoneStatus Status { get; set; }
    public DateTime PlannedDate { get; set; }
    public DateTime? ActualDate { get; set; }
}

// Request/Response DTOs (placeholders - would be defined elsewhere)
public class CreateProjectRequest { }
public class UpdateProjectRequest { }
public class ProjectFilterRequest { }
public class CreateTaskRequest { }
public class UpdateTaskRequest { }
public class TaskFilterRequest { }
public class StartTimeTrackingRequest { }
public class CreateTimeEntryRequest { }
public class UpdateTimeEntryRequest { }
public class TimeEntryFilterRequest { }
public class ProjectSummaryDto { }
public class ProjectProgressReportDto { }
public class TimeTrackingReportDto { }
public class ResourceUtilizationReportDto { }
public class ProjectPerformanceReportDto { }
public class ProjectTemplateDto { }
