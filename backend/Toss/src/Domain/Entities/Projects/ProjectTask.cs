using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;
using Toss.Domain.Enums;

namespace Toss.Domain.Entities.Projects;

/// <summary>
/// Represents a task within a project
/// </summary>
public class ProjectTask : BaseAuditableEntity, IBusinessScopedEntity
{
    public ProjectTask()
    {
        Title = string.Empty;
        Status = Enums.TaskStatus.ToDo;
    }

    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the project ID
    /// </summary>
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    /// <summary>
    /// Gets or sets the task title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the task description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the task status
    /// </summary>
    public Enums.TaskStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the assignee user ID (optional)
    /// </summary>
    public string? AssigneeId { get; set; }

    /// <summary>
    /// Gets or sets the due date (optional)
    /// </summary>
    public DateTimeOffset? DueDate { get; set; }

    /// <summary>
    /// Gets or sets the completion date
    /// </summary>
    public DateTimeOffset? CompletedDate { get; set; }

    /// <summary>
    /// Gets or sets the estimated hours
    /// </summary>
    public decimal? EstimatedHours { get; set; }

    /// <summary>
    /// Gets or sets the actual hours worked
    /// </summary>
    public decimal? ActualHours { get; set; }
}

