namespace Toss.Domain.Entities.ArtificialIntelligence;

/// <summary>
/// Represents a message in an AI conversation
/// </summary>
public class AIMessage : BaseEntity
{
    public AIMessage()
    {
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets or sets the conversation ID
    /// </summary>
    public int ConversationId { get; set; }
    public AIConversation? Conversation { get; set; }

    /// <summary>
    /// Gets or sets the message role (user, assistant, system)
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the message content
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when the message was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the AI model used for this message (if assistant)
    /// </summary>
    public string? ModelUsed { get; set; }

    /// <summary>
    /// Gets or sets the token count for this message
    /// </summary>
    public int? TokenCount { get; set; }

    /// <summary>
    /// Gets or sets metadata (e.g., context used, query parameters)
    /// </summary>
    public string? Metadata { get; set; }
}

