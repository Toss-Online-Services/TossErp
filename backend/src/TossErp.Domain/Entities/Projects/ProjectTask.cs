using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.Projects;

/// <summary>
/// Task within a project
/// </summary>
public class ProjectTask : BaseEntity
{
    public int ProjectId { get; set; }
    public string TaskNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.NotStarted;
    public int Priority { get; set; } = 3;
    
    // Dates
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    
    // Assignment
    public int? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }
    
    // Time Tracking
    public decimal EstimatedHours { get; set; }
    public decimal ActualHours { get; set; }
    
    // Progress
    public decimal PercentComplete { get; set; }
    
    // Dependencies
    public int? ParentTaskId { get; set; }
    public string? Dependencies { get; set; } // JSON array of task IDs
    
    // Navigation
    public Project Project { get; set; } = null!;
}

public enum TaskStatus
{
    NotStarted,
    InProgress,
    OnHold,
    Completed,
    Cancelled
}
