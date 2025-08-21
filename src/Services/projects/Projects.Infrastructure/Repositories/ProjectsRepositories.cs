using Microsoft.EntityFrameworkCore;
using TossErp.Projects.Infrastructure.Data;
using TossErp.ServiceDefaults.Infrastructure.Repositories;

namespace TossErp.Projects.Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation for Projects service
/// </summary>
public class ProjectsRepository<T> : Repository<T>, IRepository<T> where T : class, IAggregateRoot
{
    public ProjectsRepository(ProjectsDbContext context) : base(context)
    {
    }
}

/// <summary>
/// Repository implementation for Project entities
/// </summary>
public class ProjectRepository : ProjectsRepository<Project>, IProjectRepository
{
    private readonly ProjectsDbContext _context;

    public ProjectRepository(ProjectsDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Project>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .Where(p => p.ClientId == customerId)
            .Include(p => p.Tasks)
            .Include(p => p.Milestones)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetByManagerIdAsync(Guid managerId, CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .Where(p => p.ProjectManagerId == managerId)
            .Include(p => p.Tasks)
            .Include(p => p.Milestones)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetByStatusAsync(ProjectStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .Where(p => p.Status == status)
            .Include(p => p.Tasks)
            .Include(p => p.Milestones)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetProjectsWithTasksAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .Include(p => p.Tasks)
            .Include(p => p.Milestones)
            .Include(p => p.ResourceAllocations)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetOverdueProjectsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .Where(p => p.Timeline != null && 
                       p.Timeline.EndDate < DateTime.Today && 
                       p.Status != ProjectStatus.Completed && 
                       p.Status != ProjectStatus.Cancelled)
            .Include(p => p.Tasks)
            .Include(p => p.Milestones)
            .ToListAsync(cancellationToken);
    }

    public async Task<PaginatedResult<Project>> GetProjectsAsync(
        int pageNumber, 
        int pageSize, 
        string? searchTerm = null,
        ProjectStatus? status = null,
        Guid? customerId = null,
        Guid? managerId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Projects.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm) || 
                                   p.Description!.Contains(searchTerm) ||
                                   p.ProjectCode.Value.Contains(searchTerm));
        }

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        if (customerId.HasValue)
        {
            query = query.Where(p => p.ClientId == customerId.Value);
        }

        if (managerId.HasValue)
        {
            query = query.Where(p => p.ProjectManagerId == managerId.Value);
        }

        // Include related data
        query = query.Include(p => p.Tasks)
                     .Include(p => p.Milestones);

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Project>(items, totalCount, pageNumber, pageSize);
    }
}

/// <summary>
/// Repository implementation for ProjectTask entities
/// </summary>
public class ProjectTaskRepository : ProjectsRepository<ProjectTask>, IProjectTaskRepository
{
    private readonly ProjectsDbContext _context;

    public ProjectTaskRepository(ProjectsDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProjectTask>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _context.ProjectTasks
            .Where(t => t.ProjectId == projectId)
            .Include(t => t.Subtasks)
            .Include(t => t.Comments)
            .OrderBy(t => t.TaskNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProjectTask>> GetByAssigneeIdAsync(Guid assigneeId, CancellationToken cancellationToken = default)
    {
        return await _context.ProjectTasks
            .Where(t => t.AssigneeId == assigneeId)
            .Include(t => t.Subtasks)
            .OrderBy(t => t.Priority)
            .ThenBy(t => t.DateRange!.EndDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProjectTask>> GetByStatusAsync(TaskStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.ProjectTasks
            .Where(t => t.Status == status)
            .Include(t => t.Subtasks)
            .OrderBy(t => t.Priority)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProjectTask>> GetOverdueTasksAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ProjectTasks
            .Where(t => t.DateRange != null && 
                       t.DateRange.EndDate < DateTime.Today && 
                       t.Status != TaskStatus.Completed)
            .Include(t => t.Subtasks)
            .OrderBy(t => t.DateRange!.EndDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProjectTask>> GetTasksDueAsync(DateOnly dueDate, CancellationToken cancellationToken = default)
    {
        return await _context.ProjectTasks
            .Where(t => t.DateRange != null && 
                       t.DateRange.EndDate.Date == dueDate.ToDateTime(TimeOnly.MinValue) && 
                       t.Status != TaskStatus.Completed)
            .Include(t => t.Subtasks)
            .OrderBy(t => t.Priority)
            .ToListAsync(cancellationToken);
    }

    public async Task<PaginatedResult<ProjectTask>> GetTasksAsync(
        int pageNumber, 
        int pageSize, 
        string? searchTerm = null,
        TaskStatus? status = null,
        Guid? projectId = null,
        Guid? assigneeId = null,
        TaskPriority? priority = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ProjectTasks.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(t => t.Title.Contains(searchTerm) || 
                                   t.Description!.Contains(searchTerm) ||
                                   t.TaskNumber.Contains(searchTerm));
        }

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (projectId.HasValue)
        {
            query = query.Where(t => t.ProjectId == projectId.Value);
        }

        if (assigneeId.HasValue)
        {
            query = query.Where(t => t.AssigneeId == assigneeId.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(t => t.Priority == priority.Value);
        }

        // Include related data
        query = query.Include(t => t.Subtasks)
                     .Include(t => t.Comments);

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .OrderBy(t => t.Priority)
            .ThenBy(t => t.DateRange!.EndDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<ProjectTask>(items, totalCount, pageNumber, pageSize);
    }
}

/// <summary>
/// Repository implementation for TimeEntry entities
/// </summary>
public class TimeEntryRepository : ProjectsRepository<TimeEntry>, ITimeEntryRepository
{
    private readonly ProjectsDbContext _context;

    public TimeEntryRepository(ProjectsDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TimeEntry>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _context.TimeEntries
            .Where(te => te.ProjectId == projectId)
            .OrderByDescending(te => te.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TimeEntry>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await _context.TimeEntries
            .Where(te => te.TaskId == taskId)
            .OrderByDescending(te => te.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TimeEntry>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.TimeEntries
            .Where(te => te.UserId == userId)
            .OrderByDescending(te => te.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TimeEntry>> GetByDateRangeAsync(DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken = default)
    {
        var from = fromDate.ToDateTime(TimeOnly.MinValue);
        var to = toDate.ToDateTime(TimeOnly.MaxValue);

        return await _context.TimeEntries
            .Where(te => te.Date >= from && te.Date <= to)
            .OrderByDescending(te => te.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<PaginatedResult<TimeEntry>> GetTimeEntriesAsync(
        int pageNumber, 
        int pageSize, 
        Guid? projectId = null,
        Guid? taskId = null,
        Guid? userId = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.TimeEntries.AsQueryable();

        // Apply filters
        if (projectId.HasValue)
        {
            query = query.Where(te => te.ProjectId == projectId.Value);
        }

        if (taskId.HasValue)
        {
            query = query.Where(te => te.TaskId == taskId.Value);
        }

        if (userId.HasValue)
        {
            query = query.Where(te => te.UserId == userId.Value);
        }

        if (fromDate.HasValue)
        {
            var from = fromDate.Value.ToDateTime(TimeOnly.MinValue);
            query = query.Where(te => te.Date >= from);
        }

        if (toDate.HasValue)
        {
            var to = toDate.Value.ToDateTime(TimeOnly.MaxValue);
            query = query.Where(te => te.Date <= to);
        }

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .OrderByDescending(te => te.Date)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<TimeEntry>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<decimal> GetTotalHoursAsync(
        Guid? projectId = null,
        Guid? taskId = null,
        Guid? userId = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.TimeEntries.AsQueryable();

        // Apply filters
        if (projectId.HasValue)
        {
            query = query.Where(te => te.ProjectId == projectId.Value);
        }

        if (taskId.HasValue)
        {
            query = query.Where(te => te.TaskId == taskId.Value);
        }

        if (userId.HasValue)
        {
            query = query.Where(te => te.UserId == userId.Value);
        }

        if (fromDate.HasValue)
        {
            var from = fromDate.Value.ToDateTime(TimeOnly.MinValue);
            query = query.Where(te => te.Date >= from);
        }

        if (toDate.HasValue)
        {
            var to = toDate.Value.ToDateTime(TimeOnly.MaxValue);
            query = query.Where(te => te.Date <= to);
        }

        return await query.SumAsync(te => te.Duration.Hours, cancellationToken);
    }
}

/// <summary>
/// Repository implementation for ResourceAllocation entities (acting as Resource)
/// </summary>
public class ResourceRepository : ProjectsRepository<ResourceAllocation>, IResourceRepository
{
    private readonly ProjectsDbContext _context;

    public ResourceRepository(ProjectsDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ResourceAllocation>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _context.ResourceAllocations
            .Where(r => r.ProjectId == projectId)
            .OrderBy(r => r.ResourceName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ResourceAllocation>> GetByTypeAsync(ResourceType type, CancellationToken cancellationToken = default)
    {
        return await _context.ResourceAllocations
            .Where(r => r.ResourceType == type)
            .OrderBy(r => r.ResourceName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ResourceAllocation>> GetAvailableResourcesAsync(DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken = default)
    {
        var from = fromDate.ToDateTime(TimeOnly.MinValue);
        var to = toDate.ToDateTime(TimeOnly.MaxValue);

        return await _context.ResourceAllocations
            .Where(r => r.Status == AllocationStatus.Active &&
                       r.AllocationPeriod.StartDate <= to &&
                       r.AllocationPeriod.EndDate >= from)
            .OrderBy(r => r.ResourceName)
            .ToListAsync(cancellationToken);
    }

    public async Task<PaginatedResult<ResourceAllocation>> GetResourcesAsync(
        int pageNumber, 
        int pageSize, 
        string? searchTerm = null,
        ResourceType? type = null,
        bool? isAvailable = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ResourceAllocations.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(r => r.ResourceName.Contains(searchTerm) || 
                                   r.Notes!.Contains(searchTerm));
        }

        if (type.HasValue)
        {
            query = query.Where(r => r.ResourceType == type.Value);
        }

        if (isAvailable.HasValue)
        {
            if (isAvailable.Value)
            {
                query = query.Where(r => r.Status == AllocationStatus.Active);
            }
            else
            {
                query = query.Where(r => r.Status != AllocationStatus.Active);
            }
        }

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .OrderBy(r => r.ResourceName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<ResourceAllocation>(items, totalCount, pageNumber, pageSize);
    }
}

/// <summary>
/// Repository implementation for ProjectMilestone entities (acting as Milestone)
/// </summary>
public class MilestoneRepository : ProjectsRepository<ProjectMilestone>, IMilestoneRepository
{
    private readonly ProjectsDbContext _context;

    public MilestoneRepository(ProjectsDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProjectMilestone>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _context.ProjectMilestones
            .Where(m => m.ProjectId == projectId)
            .Include(m => m.Tasks)
            .OrderBy(m => m.PlannedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProjectMilestone>> GetUpcomingMilestonesAsync(DateOnly? beforeDate = null, CancellationToken cancellationToken = default)
    {
        var cutoffDate = beforeDate?.ToDateTime(TimeOnly.MaxValue) ?? DateTime.Today.AddDays(30);

        return await _context.ProjectMilestones
            .Where(m => m.PlannedDate <= cutoffDate && 
                       m.Status != MilestoneStatus.Completed &&
                       m.PlannedDate >= DateTime.Today)
            .Include(m => m.Tasks)
            .OrderBy(m => m.PlannedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProjectMilestone>> GetOverdueMilestonesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ProjectMilestones
            .Where(m => m.PlannedDate < DateTime.Today && 
                       m.Status != MilestoneStatus.Completed)
            .Include(m => m.Tasks)
            .OrderBy(m => m.PlannedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<PaginatedResult<ProjectMilestone>> GetMilestonesAsync(
        int pageNumber, 
        int pageSize, 
        Guid? projectId = null,
        MilestoneStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ProjectMilestones.AsQueryable();

        // Apply filters
        if (projectId.HasValue)
        {
            query = query.Where(m => m.ProjectId == projectId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(m => m.Status == status.Value);
        }

        // Include related data
        query = query.Include(m => m.Tasks);

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .OrderBy(m => m.PlannedDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<ProjectMilestone>(items, totalCount, pageNumber, pageSize);
    }
}
