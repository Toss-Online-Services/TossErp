using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;
using Toss.Domain.Enums;

namespace Toss.Domain.Entities.Tasks;

/// <summary>
/// Represents a task item that can be linked to various entities (Sales, Purchase Orders, etc.)
/// </summary>
public class TaskItem : BaseAuditableEntity, IBusinessScopedEntity
{
    public TaskItem()
    {
        Title = string.Empty;
        Description = string.Empty;
        Status = Enums.TaskStatus.ToDo;
    }

    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

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
    /// Gets or sets the due date (optional)
    /// </summary>
    public DateTimeOffset? DueDate { get; set; }

    /// <summary>
    /// Gets or sets the type of entity this task is linked to (e.g., "Sale", "PurchaseOrder", "Customer")
    /// </summary>
    public string? LinkedType { get; set; }

    /// <summary>
    /// Gets or sets the ID of the linked entity
    /// </summary>
    public int? LinkedId { get; set; }

    /// <summary>
    /// Gets or sets the user ID of the assignee (optional)
    /// </summary>
    public string? AssigneeId { get; set; }

    /// <summary>
    /// Gets or sets the priority (1 = Low, 2 = Medium, 3 = High)
    /// </summary>
    public int Priority { get; set; } = 2; // Default to Medium

    /// <summary>
    /// Gets or sets optional tags for categorization
    /// </summary>
    public string? Tags { get; set; }

    /// <summary>
    /// Gets or sets the completion date (set when status changes to Done)
    /// </summary>
    public DateTimeOffset? CompletedDate { get; set; }
}

