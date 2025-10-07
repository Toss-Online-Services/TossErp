using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.Projects;

/// <summary>
/// Project tracking and management
/// </summary>
public class Project : BaseEntity
{
    public string ProjectNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Planning;
    public ProjectType Type { get; set; }
    
    // Dates
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    
    // Budget
    public decimal Budget { get; set; }
    public decimal ActualCost { get; set; }
    public decimal PercentComplete { get; set; }
    
    // People
    public int? ManagerId { get; set; }
    public string? ManagerName { get; set; }
    public int? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    
    // Metadata
    public string? Notes { get; set; }
    public int Priority { get; set; } = 5;
    
    // Navigation
    public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
}

public enum ProjectStatus
{
    Planning,
    Active,
    OnHold,
    Completed,
    Cancelled
}

public enum ProjectType
{
    Internal,
    CustomerProject,
    Research,
    Maintenance
}
