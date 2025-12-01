using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;
using Toss.Domain.Enums;

namespace Toss.Domain.Entities.Support;

/// <summary>
/// Represents a support ticket
/// </summary>
public class Ticket : BaseAuditableEntity, IBusinessScopedEntity
{
    public Ticket()
    {
        Title = string.Empty;
        Description = string.Empty;
        Status = TicketStatus.New;
        Type = TicketType.Issue;
    }

    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ticket type
    /// </summary>
    public TicketType Type { get; set; }

    /// <summary>
    /// Gets or sets the ticket status
    /// </summary>
    public TicketStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the ticket title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the ticket description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the type of entity this ticket is linked to (e.g., "Sale", "Product", "Customer")
    /// </summary>
    public string? LinkedEntityType { get; set; }

    /// <summary>
    /// Gets or sets the ID of the entity this ticket is linked to
    /// </summary>
    public int? LinkedEntityId { get; set; }

    /// <summary>
    /// Gets or sets the user ID who created the ticket
    /// </summary>
    public string CreatedById { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user ID who is assigned to the ticket (optional)
    /// </summary>
    public string? AssignedToId { get; set; }

    /// <summary>
    /// Gets or sets when the ticket was closed (if applicable)
    /// </summary>
    public DateTimeOffset? ClosedAt { get; set; }

    /// <summary>
    /// Gets or sets optional priority (1 = Low, 2 = Medium, 3 = High)
    /// </summary>
    public int Priority { get; set; } = 2;

    /// <summary>
    /// Gets or sets the collection of ticket notes
    /// </summary>
    public ICollection<TicketNote> Notes { get; private set; } = new List<TicketNote>();
}

