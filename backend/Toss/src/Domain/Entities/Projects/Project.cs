using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;
using Toss.Domain.Entities.CRM;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Entities.Stores;
using Toss.Domain.Enums;

namespace Toss.Domain.Entities.Projects;

/// <summary>
/// Represents a project/job card for tracking work, materials, and labour
/// </summary>
public class Project : BaseAuditableEntity, IBusinessScopedEntity
{
    public Project()
    {
        Title = string.Empty;
        Tasks = new List<ProjectTask>();
        Materials = new List<ProjectMaterial>();
        LabourEntries = new List<LabourEntry>();
        Status = ProjectStatus.New;
    }

    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the project title/name
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the project description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the customer ID (optional, can be a general project)
    /// </summary>
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    /// <summary>
    /// Gets or sets the shop/store ID where the project is located
    /// </summary>
    public int? ShopId { get; set; }
    public Store? Shop { get; set; }

    /// <summary>
    /// Gets or sets the project status
    /// </summary>
    public ProjectStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the start date
    /// </summary>
    public DateTimeOffset? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the expected completion date
    /// </summary>
    public DateTimeOffset? ExpectedCompletionDate { get; set; }

    /// <summary>
    /// Gets or sets the actual completion date
    /// </summary>
    public DateTimeOffset? CompletedDate { get; set; }

    /// <summary>
    /// Gets or sets the linked sales invoice ID (when project is invoiced)
    /// </summary>
    public int? InvoiceId { get; set; }
    public SalesDocument? Invoice { get; set; }

    /// <summary>
    /// Gets or sets optional notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the collection of project tasks
    /// </summary>
    public ICollection<ProjectTask> Tasks { get; private set; }

    /// <summary>
    /// Gets or sets the collection of materials used
    /// </summary>
    public ICollection<ProjectMaterial> Materials { get; private set; }

    /// <summary>
    /// Gets or sets the collection of labour entries
    /// </summary>
    public ICollection<LabourEntry> LabourEntries { get; private set; }
}

