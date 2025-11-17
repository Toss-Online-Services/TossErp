using Microsoft.EntityFrameworkCore;
using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.ArtificialIntelligence;

namespace Toss.Infrastructure.Services.ArtificialIntelligence;

/// <summary>
/// Default implementation of AI settings service
/// </summary>
public partial class AISettingsService : IAISettingsService
{
    private readonly IApplicationDbContext _context;
    private AISettings? _cachedSettings;

    public AISettingsService(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets the current AI settings from database or returns defaults
    /// </summary>
    public async Task<AISettings> GetSettingsAsync()
    {
        if (_cachedSettings != null)
            return _cachedSettings;

        _cachedSettings = await _context.AISettings.FirstOrDefaultAsync();

        if (_cachedSettings == null)
        {
            // Return default settings
            _cachedSettings = new AISettings
            {
                ProviderType = AIProviderType.Gemini,
                RequestTimeout = ArtificialIntelligenceDefaults.RequestTimeout,
                AllowMetaTitleGeneration = true,
                AllowMetaKeywordsGeneration = true,
                AllowMetaDescriptionGeneration = true,
                ProductDescriptionQuery = ArtificialIntelligenceDefaults.ProductDescriptionQuery,
                MetaTitleQuery = ArtificialIntelligenceDefaults.MetaTitleQuery,
                MetaKeywordsQuery = ArtificialIntelligenceDefaults.MetaKeywordsQuery,
                MetaDescriptionQuery = ArtificialIntelligenceDefaults.MetaDescriptionQuery
            };
        }

        return _cachedSettings;
    }
}

