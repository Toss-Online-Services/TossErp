using TossErp.Projects.Domain.Enums;
using TossErp.Projects.Domain.SeedWork;
using TossErp.Projects.Domain.ValueObjects;

namespace TossErp.Projects.Domain.Entities;

/// <summary>
/// Project Task entity representing individual work items
/// </summary>
public class ProjectTask : Entity
{
    public Guid ProjectId { get; private set; }
    public string TaskNumber { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public TaskStatus Status { get; private set; }
    public TaskPriority Priority { get; private set; }
    public TaskType TaskType { get; private set; }
    public DateRange? DateRange { get; private set; }
    public Effort? Effort { get; private set; }
    public Progress Progress { get; private set; }
    public Guid? AssigneeId { get; private set; }
    public string? AssigneeName { get; private set; }
    public Guid? ParentTaskId { get; private set; }
    public Guid? MilestoneId { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public string? ModifiedBy { get; private set; }

    private readonly List<ProjectTask> _subtasks;
    public IReadOnlyList<ProjectTask> Subtasks => _subtasks.AsReadOnly();

    private readonly List<TaskComment> _comments;
    public IReadOnlyList<TaskComment> Comments => _comments.AsReadOnly();

    private readonly List<TaskAttachment> _attachments;
    public IReadOnlyList<TaskAttachment> Attachments => _attachments.AsReadOnly();

    private ProjectTask()
    {
        _subtasks = new List<ProjectTask>();
        _comments = new List<TaskComment>();
        _attachments = new List<TaskAttachment>();
        TaskNumber = null!;
        Title = null!;
        Progress = null!;
        CreatedBy = null!;
    } // EF Core

    public ProjectTask(
        Guid id,
        Guid projectId,
        string taskNumber,
        string title,
        TaskPriority priority,
        TaskType taskType,
        string createdBy,
        string? description = null,
        Guid? parentTaskId = null,
        Guid? milestoneId = null)
    {
        Id = id;
        ProjectId = projectId;
        TaskNumber = taskNumber?.Trim() ?? throw new ArgumentException("Task number cannot be empty");
        Title = title?.Trim() ?? throw new ArgumentException("Title cannot be empty");
        Description = description?.Trim();
        Status = TaskStatus.NotStarted;
        Priority = priority;
        TaskType = taskType;
        Progress = Progress.Zero;
        ParentTaskId = parentTaskId;
        MilestoneId = milestoneId;
        CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
        CreatedAt = DateTime.UtcNow;
        _subtasks = new List<ProjectTask>();
        _comments = new List<TaskComment>();
        _attachments = new List<TaskAttachment>();
    }

    public void UpdateDetails(string title, string? description, string modifiedBy)
    {
        Title = title?.Trim() ?? throw new ArgumentException("Title cannot be empty");
        Description = description?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void UpdateStatus(TaskStatus status, string modifiedBy)
    {
        Status = status;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));

        // Auto-update progress based on status
        Progress = status switch
        {
            TaskStatus.NotStarted => Progress.Zero,
            TaskStatus.Completed => Progress.Complete,
            _ => Progress
        };
    }

    public void UpdateProgress(Progress progress, string modifiedBy)
    {
        Progress = progress ?? throw new ArgumentNullException(nameof(progress));
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));

        // Auto-update status based on progress
        if (progress.IsComplete && Status != TaskStatus.Completed)
        {
            Status = TaskStatus.Completed;
        }
        else if (progress.IsStarted && Status == TaskStatus.NotStarted)
        {
            Status = TaskStatus.InProgress;
        }
    }

    public void AssignTo(Guid assigneeId, string assigneeName, string modifiedBy)
    {
        AssigneeId = assigneeId;
        AssigneeName = assigneeName?.Trim() ?? throw new ArgumentException("Assignee name cannot be empty");
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void Unassign(string modifiedBy)
    {
        AssigneeId = null;
        AssigneeName = null;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void SetDateRange(DateRange dateRange, string modifiedBy)
    {
        DateRange = dateRange;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void SetEffort(Effort effort, string modifiedBy)
    {
        Effort = effort;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void UpdatePriority(TaskPriority priority, string modifiedBy)
    {
        Priority = priority;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void AddComment(TaskComment comment)
    {
        if (comment == null)
            throw new ArgumentNullException(nameof(comment));
        _comments.Add(comment);
    }

    public void AddAttachment(TaskAttachment attachment)
    {
        if (attachment == null)
            throw new ArgumentNullException(nameof(attachment));
        _attachments.Add(attachment);
    }

    public void AddSubtask(ProjectTask subtask)
    {
        if (subtask == null)
            throw new ArgumentNullException(nameof(subtask));
        if (subtask.ProjectId != ProjectId)
            throw new InvalidOperationException("Subtask must belong to the same project");
        _subtasks.Add(subtask);
    }

    public bool IsOverdue => DateRange?.EndDate < DateTime.Today && Status != TaskStatus.Completed;
    public bool IsActive => Status == TaskStatus.InProgress;
    public bool IsCompleted => Status == TaskStatus.Completed;
    public bool HasSubtasks => _subtasks.Any();
    public bool IsAssigned => AssigneeId.HasValue;
}

/// <summary>
/// Task Comment entity for communication
/// </summary>
public class TaskComment : Entity
{
    public Guid TaskId { get; private set; }
    public string Content { get; private set; }
    public string AuthorId { get; private set; }
    public string AuthorName { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public bool IsEdited => ModifiedAt.HasValue;

    private TaskComment()
    {
        Content = null!;
        AuthorId = null!;
        AuthorName = null!;
    } // EF Core

    public TaskComment(
        Guid id,
        Guid taskId,
        string content,
        string authorId,
        string authorName)
    {
        Id = id;
        TaskId = taskId;
        Content = content?.Trim() ?? throw new ArgumentException("Content cannot be empty");
        AuthorId = authorId ?? throw new ArgumentNullException(nameof(authorId));
        AuthorName = authorName?.Trim() ?? throw new ArgumentException("Author name cannot be empty");
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateContent(string content)
    {
        Content = content?.Trim() ?? throw new ArgumentException("Content cannot be empty");
        ModifiedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Task Attachment entity for file management
/// </summary>
public class TaskAttachment : Entity
{
    public Guid TaskId { get; private set; }
    public string FileName { get; private set; }
    public string? Description { get; private set; }
    public string FilePath { get; private set; }
    public string ContentType { get; private set; }
    public long FileSize { get; private set; }
    public string UploadedBy { get; private set; }
    public DateTime UploadedAt { get; private set; }

    private TaskAttachment()
    {
        FileName = null!;
        FilePath = null!;
        ContentType = null!;
        UploadedBy = null!;
    } // EF Core

    public TaskAttachment(
        Guid id,
        Guid taskId,
        string fileName,
        string filePath,
        string contentType,
        long fileSize,
        string uploadedBy,
        string? description = null)
    {
        Id = id;
        TaskId = taskId;
        FileName = fileName?.Trim() ?? throw new ArgumentException("File name cannot be empty");
        Description = description?.Trim();
        FilePath = filePath?.Trim() ?? throw new ArgumentException("File path cannot be empty");
        ContentType = contentType?.Trim() ?? throw new ArgumentException("Content type cannot be empty");
        FileSize = fileSize > 0 ? fileSize : throw new ArgumentException("File size must be positive");
        UploadedBy = uploadedBy ?? throw new ArgumentNullException(nameof(uploadedBy));
        UploadedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string? description)
    {
        Description = description?.Trim();
    }
}

/// <summary>
/// Project Milestone entity for key deliverables
/// </summary>
public class ProjectMilestone : Entity
{
    public Guid ProjectId { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public MilestoneStatus Status { get; private set; }
    public MilestoneType Type { get; private set; }
    public DateTime PlannedDate { get; private set; }
    public DateTime? ActualDate { get; private set; }
    public Progress Progress { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public string? ModifiedBy { get; private set; }

    private readonly List<ProjectTask> _tasks;
    public IReadOnlyList<ProjectTask> Tasks => _tasks.AsReadOnly();

    private ProjectMilestone()
    {
        _tasks = new List<ProjectTask>();
        Name = null!;
        Progress = null!;
        CreatedBy = null!;
    } // EF Core

    public ProjectMilestone(
        Guid id,
        Guid projectId,
        string name,
        MilestoneType type,
        DateTime plannedDate,
        string createdBy,
        string? description = null)
    {
        Id = id;
        ProjectId = projectId;
        Name = name?.Trim() ?? throw new ArgumentException("Name cannot be empty");
        Description = description?.Trim();
        Status = MilestoneStatus.Planned;
        Type = type;
        PlannedDate = plannedDate.Date;
        Progress = Progress.Zero;
        CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
        CreatedAt = DateTime.UtcNow;
        _tasks = new List<ProjectTask>();
    }

    public void UpdateDetails(string name, string? description, string modifiedBy)
    {
        Name = name?.Trim() ?? throw new ArgumentException("Name cannot be empty");
        Description = description?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void UpdateStatus(MilestoneStatus status, string modifiedBy)
    {
        Status = status;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));

        if (status == MilestoneStatus.Completed && !ActualDate.HasValue)
        {
            ActualDate = DateTime.UtcNow.Date;
            Progress = Progress.Complete;
        }
    }

    public void UpdatePlannedDate(DateTime plannedDate, string modifiedBy)
    {
        PlannedDate = plannedDate.Date;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void Complete(DateTime? completionDate = null)
    {
        Status = MilestoneStatus.Completed;
        ActualDate = completionDate?.Date ?? DateTime.UtcNow.Date;
        Progress = Progress.Complete;
        ModifiedAt = DateTime.UtcNow;
    }

    public void AddTask(ProjectTask task)
    {
        if (task == null)
            throw new ArgumentNullException(nameof(task));
        if (task.ProjectId != ProjectId)
            throw new InvalidOperationException("Task must belong to the same project");
        _tasks.Add(task);
    }

    public bool IsOverdue => PlannedDate < DateTime.Today && Status != MilestoneStatus.Completed;
    public bool IsCompleted => Status == MilestoneStatus.Completed;
    public bool IsDelayed => ActualDate > PlannedDate;
    public int DelayInDays => IsDelayed && ActualDate.HasValue ? (ActualDate.Value - PlannedDate).Days : 0;
}

/// <summary>
/// Resource Allocation entity for project resource management
/// </summary>
public class ResourceAllocation : Entity
{
    public Guid ProjectId { get; private set; }
    public Guid ResourceId { get; private set; }
    public string ResourceName { get; private set; }
    public ResourceType ResourceType { get; private set; }
    public AllocationStatus Status { get; private set; }
    public DateRange AllocationPeriod { get; private set; }
    public decimal AllocationPercentage { get; private set; }
    public Money? Cost { get; private set; }
    public string AllocatedBy { get; private set; }
    public DateTime AllocatedAt { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public string? ModifiedBy { get; private set; }
    public string? Notes { get; private set; }

    private ResourceAllocation()
    {
        ResourceName = null!;
        AllocationPeriod = null!;
        AllocatedBy = null!;
    } // EF Core

    public ResourceAllocation(
        Guid id,
        Guid projectId,
        Guid resourceId,
        string resourceName,
        ResourceType resourceType,
        DateRange allocationPeriod,
        decimal allocationPercentage,
        string allocatedBy,
        Money? cost = null,
        string? notes = null)
    {
        Id = id;
        ProjectId = projectId;
        ResourceId = resourceId;
        ResourceName = resourceName?.Trim() ?? throw new ArgumentException("Resource name cannot be empty");
        ResourceType = resourceType;
        AllocationPeriod = allocationPeriod ?? throw new ArgumentNullException(nameof(allocationPeriod));
        AllocationPercentage = allocationPercentage >= 0 && allocationPercentage <= 100 ? 
            allocationPercentage : throw new ArgumentException("Allocation percentage must be between 0 and 100");
        Cost = cost;
        Notes = notes?.Trim();
        Status = AllocationStatus.Planned;
        AllocatedBy = allocatedBy ?? throw new ArgumentNullException(nameof(allocatedBy));
        AllocatedAt = DateTime.UtcNow;
    }

    public void UpdateAllocation(decimal allocationPercentage, string modifiedBy)
    {
        AllocationPercentage = allocationPercentage >= 0 && allocationPercentage <= 100 ? 
            allocationPercentage : throw new ArgumentException("Allocation percentage must be between 0 and 100");
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void UpdatePeriod(DateRange allocationPeriod, string modifiedBy)
    {
        AllocationPeriod = allocationPeriod ?? throw new ArgumentNullException(nameof(allocationPeriod));
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void UpdateStatus(AllocationStatus status, string modifiedBy)
    {
        Status = status;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void UpdateCost(Money cost, string modifiedBy)
    {
        Cost = cost;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void UpdateNotes(string? notes, string modifiedBy)
    {
        Notes = notes?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public bool IsActive => AllocationPeriod.IsActive && Status == AllocationStatus.Active;
    public bool IsFullTime => AllocationPercentage >= 100;
    public bool IsPartTime => AllocationPercentage < 100 && AllocationPercentage > 0;
}

/// <summary>
/// Time Tracking Entry entity for project time management
/// </summary>
public class TimeEntry : Entity
{
    public Guid ProjectId { get; private set; }
    public Guid? TaskId { get; private set; }
    public Guid UserId { get; private set; }
    public string UserName { get; private set; }
    public DateTime Date { get; private set; }
    public Duration Duration { get; private set; }
    public TimeEntryType Type { get; private set; }
    public TimeEntryStatus Status { get; private set; }
    public string? Description { get; private set; }
    public Money? BillableAmount { get; private set; }
    public bool IsBillable { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public string? ModifiedBy { get; private set; }

    private TimeEntry()
    {
        UserName = null!;
        Duration = null!;
    } // EF Core

    public TimeEntry(
        Guid id,
        Guid projectId,
        Guid userId,
        string userName,
        DateTime date,
        Duration duration,
        TimeEntryType type,
        bool isBillable = true,
        Guid? taskId = null,
        string? description = null,
        Money? billableAmount = null)
    {
        Id = id;
        ProjectId = projectId;
        TaskId = taskId;
        UserId = userId;
        UserName = userName?.Trim() ?? throw new ArgumentException("User name cannot be empty");
        Date = date.Date;
        Duration = duration ?? throw new ArgumentNullException(nameof(duration));
        Type = type;
        Status = TimeEntryStatus.Draft;
        Description = description?.Trim();
        IsBillable = isBillable;
        BillableAmount = billableAmount;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDuration(Duration duration, string modifiedBy)
    {
        Duration = duration ?? throw new ArgumentNullException(nameof(duration));
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void UpdateDescription(string? description, string modifiedBy)
    {
        Description = description?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void Submit(string modifiedBy)
    {
        if (Status != TimeEntryStatus.Draft)
            throw new InvalidOperationException("Only draft entries can be submitted");

        Status = TimeEntryStatus.Submitted;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void Approve(string approvedBy)
    {
        if (Status != TimeEntryStatus.Submitted)
            throw new InvalidOperationException("Only submitted entries can be approved");

        Status = TimeEntryStatus.Approved;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = approvedBy ?? throw new ArgumentNullException(nameof(approvedBy));
    }

    public void Reject(string rejectedBy)
    {
        if (Status != TimeEntryStatus.Submitted)
            throw new InvalidOperationException("Only submitted entries can be rejected");

        Status = TimeEntryStatus.Rejected;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = rejectedBy ?? throw new ArgumentNullException(nameof(rejectedBy));
    }

    public void MarkAsBilled(string modifiedBy)
    {
        if (Status != TimeEntryStatus.Approved)
            throw new InvalidOperationException("Only approved entries can be marked as billed");

        Status = TimeEntryStatus.Billed;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void UpdateBillableAmount(Money billableAmount, string modifiedBy)
    {
        BillableAmount = billableAmount;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public bool CanBeModified => Status == TimeEntryStatus.Draft || Status == TimeEntryStatus.Rejected;
    public bool IsApproved => Status == TimeEntryStatus.Approved;
    public bool IsBilled => Status == TimeEntryStatus.Billed;
}
