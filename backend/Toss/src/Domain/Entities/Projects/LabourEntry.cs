using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;

namespace Toss.Domain.Entities.Projects;

/// <summary>
/// Represents a labour/time entry for a project
/// </summary>
public class LabourEntry : BaseAuditableEntity, IBusinessScopedEntity
{
    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the project ID
    /// </summary>
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user ID who performed the work
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Gets or sets the date of work
    /// </summary>
    public DateTimeOffset WorkDate { get; set; }

    /// <summary>
    /// Gets or sets the hours worked
    /// </summary>
    public decimal Hours { get; set; }

    /// <summary>
    /// Gets or sets the hourly rate
    /// </summary>
    public decimal Rate { get; set; }

    /// <summary>
    /// Gets or sets the total cost (Hours * Rate)
    /// </summary>
    public decimal TotalCost { get; set; }

    /// <summary>
    /// Gets or sets optional description of work performed
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the linked project task ID (optional)
    /// </summary>
    public int? ProjectTaskId { get; set; }
    public ProjectTask? ProjectTask { get; set; }
}

