using Toss.Domain.Common;
using Toss.Domain.Entities.ArtificialIntelligence;
using Toss.Domain.Entities.Localization;

namespace Toss.Application.Common.Interfaces;

/// <summary>
/// Service for AI-powered functionality
/// </summary>
public partial interface IArtificialIntelligenceService
{
    /// <summary>
    /// Create product description using AI
    /// </summary>
    Task<string> CreateProductDescriptionAsync(
        string productName, 
        string keywords, 
        ToneOfVoiceType toneOfVoice, 
        string instruction, 
        string? customToneOfVoice = null, 
        int languageId = 0);

    /// <summary>
    /// Create meta tags for a localized entity using AI
    /// </summary>
    Task<(string metaTitle, string metaKeywords, string metaDescription)> CreateMetaTagsForLocalizedEntityAsync<TEntity>(
        TEntity entity, 
        int languageId)
        where TEntity : BaseEntity, IMetaTagsSupported, ILocalizedEntity;

    /// <summary>
    /// Create meta tags for an entity using AI
    /// </summary>
    Task<(string metaTitle, string metaKeywords, string metaDescription)> CreateMetaTagsAsync<TEntity>(
        TEntity entity, 
        int languageId = 0)
        where TEntity : BaseEntity, IMetaTagsSupported;
}

