using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;

namespace Toss.Domain.Entities.Support;

/// <summary>
/// Represents a note/comment on a support ticket
/// </summary>
public class TicketNote : BaseAuditableEntity, IBusinessScopedEntity
{
    public TicketNote()
    {
        Note = string.Empty;
    }

    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ticket ID this note belongs to
    /// </summary>
    public int TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;

    /// <summary>
    /// Gets or sets the note content
    /// </summary>
    public string Note { get; set; }

    /// <summary>
    /// Gets or sets the user ID who created the note
    /// </summary>
    public string CreatedById { get; set; } = string.Empty;
}

