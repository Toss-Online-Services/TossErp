namespace Toss.Domain.Entities.ArtificialIntelligence;

/// <summary>
/// Represents the artificial intelligence provider enumeration for TOSS
/// </summary>
public enum AIProviderType
{
    /// <summary>
    /// Google Gemini
    /// </summary>
    Gemini = 1,
    
    /// <summary>
    /// OpenAI (ChatGPT)
    /// </summary>
    ChatGpt = 2,
    
    /// <summary>
    /// DeepSeek AI
    /// </summary>
    DeepSeek = 3,
    
    /// <summary>
    /// Anthropic Claude
    /// </summary>
    Claude = 4,
    
    /// <summary>
    /// Local/Self-hosted AI (for offline support)
    /// </summary>
    LocalAI = 5
}

