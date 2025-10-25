namespace Toss.Domain.Entities.ArtificialIntelligence;

/// <summary>
/// Represents an AI conversation session with the TOSS co-pilot
/// </summary>
public class AIConversation : BaseAuditableEntity
{
    public AIConversation()
    {
        Messages = new List<AIMessage>();
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    /// <summary>
    /// Gets or sets the shop ID
    /// </summary>
    public int ShopId { get; set; }
    public Shop? Shop { get; set; }

    /// <summary>
    /// Gets or sets the user ID who initiated the conversation
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Gets or sets the conversation title/summary
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets when the conversation was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets when the conversation was last updated
    /// </summary>
    public DateTime? LastMessageAt { get; set; }

    /// <summary>
    /// Gets or sets whether the conversation is still active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the preferred language for this conversation
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// Gets or sets the conversation messages
    /// </summary>
    public ICollection<AIMessage> Messages { get; set; }
}

