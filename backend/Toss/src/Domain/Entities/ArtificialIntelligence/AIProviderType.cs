namespace Toss.Domain.Entities.ArtificialIntelligence;

/// <summary>
/// Represents the artificial intelligence provider enumeration for TOSS
/// </summary>
public enum AIProviderType
{
    /// <summary>
    /// OpenAI (ChatGPT)
    /// </summary>
    OpenAI = 1,
    
    /// <summary>
    /// Google Gemini
    /// </summary>
    Gemini = 2,
    
    /// <summary>
    /// Anthropic Claude
    /// </summary>
    Claude = 3,
    
    /// <summary>
    /// Local/Self-hosted AI (for offline support)
    /// </summary>
    LocalAI = 4
}

