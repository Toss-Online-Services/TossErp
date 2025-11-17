using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Toss.Application.Common.Interfaces;
using Toss.Domain.Common;
using Toss.Domain.Entities.ArtificialIntelligence;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Entities.Localization;
using Toss.Domain.Entities.Vendors;

namespace Toss.Infrastructure.Services.ArtificialIntelligence;

/// <summary>
/// Represent Artificial intelligence service
/// </summary>
public class ArtificialIntelligenceService : IArtificialIntelligenceService
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ArtificialIntelligenceService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IAISettingsService _aiSettingsService;

    public ArtificialIntelligenceService(
        IApplicationDbContext context,
        ILogger<ArtificialIntelligenceService> logger,
        IHttpClientFactory httpClientFactory,
        IAISettingsService aiSettingsService)
    {
        _context = context;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _aiSettingsService = aiSettingsService;
    }

    /// <summary>
    /// Create product description by artificial intelligence
    /// </summary>
    public virtual async Task<string> CreateProductDescriptionAsync(
        string productName, 
        string keywords, 
        ToneOfVoiceType toneOfVoice, 
        string instruction, 
        string? customToneOfVoice = null, 
        int languageId = 0)
    {
        // Get AI settings from database (assuming ShopId = 1 for now, should be passed as parameter in production)
        var settings = await _context.AISettings.FirstOrDefaultAsync() 
            ?? throw new InvalidOperationException("AI settings not configured");

        if (!settings.Enabled)
            throw new InvalidOperationException("AI service is not enabled");

        var language = "English"; // TODO: Get language from languageId

        var toneOfVoiceInstruction = toneOfVoice switch
        {
            ToneOfVoiceType.Expert => "expert and knowledgeable tone",
            ToneOfVoiceType.Supportive => "supportive and friendly tone",
            ToneOfVoiceType.Custom => customToneOfVoice ?? "professional tone",
            _ => "professional tone"
        };

        var query = string.Format(
            settings.ProductDescriptionQuery ?? ArtificialIntelligenceDefaults.ProductDescriptionQuery, 
            productName, 
            keywords, 
            toneOfVoiceInstruction, 
            instruction, 
            language);

        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var aiHttpClient = new ArtificialIntelligenceHttpClient(_aiSettingsService, httpClient);
            var result = await aiHttpClient.SendQueryAsync(query);
            
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create product description with AI: {Message}", e.Message);
            throw new ApplicationException($"Failed to create product description: {e.Message}", e);
        }
    }

    /// <summary>
    /// Create meta tags for localized entity by artificial intelligence
    /// </summary>
    public virtual async Task<(string metaTitle, string metaKeywords, string metaDescription)> CreateMetaTagsForLocalizedEntityAsync<TEntity>(
        TEntity entity, 
        int languageId)
        where TEntity : BaseEntity, IMetaTagsSupported, ILocalizedEntity
    {
        // For now, use the same implementation as CreateMetaTagsAsync
        // In the future, this could be enhanced with localization-specific logic
        return await CreateMetaTagsAsync(entity, languageId);
    }

    /// <summary>
    /// Create meta tags by artificial intelligence
    /// </summary>
    public virtual async Task<(string metaTitle, string metaKeywords, string metaDescription)> CreateMetaTagsAsync<TEntity>(
        TEntity entity, 
        int languageId = 0)
        where TEntity : BaseEntity, IMetaTagsSupported
    {
        // Get AI settings from database
        var settings = await _context.AISettings.FirstOrDefaultAsync() 
            ?? throw new InvalidOperationException("AI settings not configured");

        if (!settings.Enabled)
            throw new InvalidOperationException("AI service is not enabled");

        var language = "English"; // TODO: Get language from languageId
        var title = string.Empty;
        var text = string.Empty;
        var metaTitle = entity.MetaTitle ?? string.Empty;
        var metaKeywords = entity.MetaKeywords ?? string.Empty;
        var metaDescription = entity.MetaDescription ?? string.Empty;

        // Extract title and text based on entity type
        switch (entity)
        {
            case Product product:
                title = product.Name;
                text = product.Description ?? string.Empty;
                if (string.IsNullOrEmpty(text))
                    throw new ApplicationException("Product description is required for meta tag generation");
                break;

            case ProductCategory category:
                title = category.Name;
                text = category.Description ?? string.Empty;
                if (string.IsNullOrEmpty(text))
                    throw new ApplicationException("Category description is required for meta tag generation");
                break;

            case Vendor vendor:
                title = vendor.Name;
                text = vendor.Description ?? string.Empty;
                if (string.IsNullOrEmpty(text))
                    throw new ApplicationException("Vendor description is required for meta tag generation");
                break;

            default:
                throw new NotSupportedException($"Entity type {typeof(TEntity).Name} is not supported for meta tag generation");
        }

        // Strip HTML tags from text
        if (!string.IsNullOrEmpty(text))
            text = Regex.Replace(text, "<.*?>", string.Empty);

        if (string.IsNullOrEmpty(title))
            throw new ApplicationException("Title is required for meta tag generation");

        if (string.IsNullOrEmpty(text))
            throw new ApplicationException("Description/text is required for meta tag generation");

        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var aiHttpClient = new ArtificialIntelligenceHttpClient(_aiSettingsService, httpClient);

            // Generate meta title
            if (settings.AllowMetaTitleGeneration && string.IsNullOrEmpty(entity.MetaTitle))
            {
                var metaTitleQuery = string.Format(
                    settings.MetaTitleQuery ?? ArtificialIntelligenceDefaults.MetaTitleQuery,
                    title,
                    text,
                    language);
                var result = await aiHttpClient.SendQueryAsync(metaTitleQuery);
                metaTitle = result.Trim('"');
            }

            // Generate meta keywords
            if (settings.AllowMetaKeywordsGeneration && string.IsNullOrEmpty(entity.MetaKeywords))
            {
                var metaKeywordsQuery = string.Format(
                    settings.MetaKeywordsQuery ?? ArtificialIntelligenceDefaults.MetaKeywordsQuery,
                    title,
                    text,
                    language);
                metaKeywords = await aiHttpClient.SendQueryAsync(metaKeywordsQuery);
            }

            // Generate meta description
            if (settings.AllowMetaDescriptionGeneration && string.IsNullOrEmpty(entity.MetaDescription))
            {
                var metaDescriptionQuery = string.Format(
                    settings.MetaDescriptionQuery ?? ArtificialIntelligenceDefaults.MetaDescriptionQuery,
                    title,
                    text,
                    language);
                var result = await aiHttpClient.SendQueryAsync(metaDescriptionQuery);
                metaDescription = result.Trim('"');
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create meta tags with AI: {Message}", e.Message);
            throw new ApplicationException($"Failed to create meta tags: {e.Message}", e);
        }

        return (metaTitle, metaKeywords, metaDescription);
    }

    /// <summary>
    /// Generate AI response for a question
    /// </summary>
    public virtual async Task<string> GenerateResponseAsync(string question, string? context = null)
    {
        // Get AI settings from database
        var settings = await _context.AISettings.FirstOrDefaultAsync() 
            ?? throw new InvalidOperationException("AI settings not configured");

        if (!settings.Enabled)
            throw new InvalidOperationException("AI service is not enabled");

        var fullQuery = question;
        if (!string.IsNullOrEmpty(context))
            fullQuery = $"{context}\n\nQuestion: {question}";

        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var aiHttpClient = new ArtificialIntelligenceHttpClient(_aiSettingsService, httpClient);
            var result = await aiHttpClient.SendQueryAsync(fullQuery);
            
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to generate AI response: {Message}", e.Message);
            throw new ApplicationException($"Failed to generate AI response: {e.Message}", e);
        }
    }
}

