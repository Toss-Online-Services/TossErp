using Toss.Domain.Entities.ArtificialIntelligence;

namespace Toss.Infrastructure.Services.ArtificialIntelligence;

/// <summary>
/// Service for managing AI settings
/// </summary>
public partial interface IAISettingsService
{
    /// <summary>
    /// Gets the current AI settings
    /// </summary>
    Task<AISettings> GetSettingsAsync();
}

