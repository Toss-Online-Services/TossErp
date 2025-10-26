namespace Toss.Domain.Entities.ArtificialIntelligence;

/// <summary>
/// AI settings for TOSS business intelligence and assistant features
/// </summary>
public class AISettings : BaseEntity
{
    public AISettings()
    {
        Enabled = true;
        ProviderType = AIProviderType.Gemini;
        AllowSalesForecasting = true;
        AllowInventoryPrediction = true;
        AllowBusinessInsights = true;
        AllowPriceSuggestions = true;
        AllowMetaTitleGeneration = true;
        AllowMetaKeywordsGeneration = true;
        AllowMetaDescriptionGeneration = true;
        SupportedLanguages = new List<string> { "en", "zu", "xh", "st", "tn", "af" };
        RequestTimeout = 30;
    }

    /// <summary>
    /// Gets or sets whether AI features are enabled
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the AI provider type
    /// </summary>
    public AIProviderType ProviderType { get; set; }

    /// <summary>
    /// Gets or sets the Gemini API key
    /// </summary>
    public string? GeminiApiKey { get; set; }

    /// <summary>
    /// Gets or sets the ChatGPT API key
    /// </summary>
    public string? ChatGptApiKey { get; set; }

    /// <summary>
    /// Gets or sets the DeepSeek API key
    /// </summary>
    public string? DeepSeekApiKey { get; set; }

    /// <summary>
    /// Gets or sets the API endpoint URL (for custom providers)
    /// </summary>
    public string? ApiEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the request timeout in seconds
    /// </summary>
    public int? RequestTimeout { get; set; }

    /// <summary>
    /// Gets or sets whether meta title generation is allowed
    /// </summary>
    public bool AllowMetaTitleGeneration { get; set; }

    /// <summary>
    /// Gets or sets whether meta keywords generation is allowed
    /// </summary>
    public bool AllowMetaKeywordsGeneration { get; set; }

    /// <summary>
    /// Gets or sets whether meta description generation is allowed
    /// </summary>
    public bool AllowMetaDescriptionGeneration { get; set; }

    /// <summary>
    /// Gets or sets the product description query template
    /// </summary>
    public string? ProductDescriptionQuery { get; set; }

    /// <summary>
    /// Gets or sets the meta title query template
    /// </summary>
    public string? MetaTitleQuery { get; set; }

    /// <summary>
    /// Gets or sets the meta keywords query template
    /// </summary>
    public string? MetaKeywordsQuery { get; set; }

    /// <summary>
    /// Gets or sets the meta description query template
    /// </summary>
    public string? MetaDescriptionQuery { get; set; }

    /// <summary>
    /// Gets or sets whether sales forecasting is allowed
    /// </summary>
    public bool AllowSalesForecasting { get; set; }

    /// <summary>
    /// Gets or sets whether inventory prediction is allowed
    /// </summary>
    public bool AllowInventoryPrediction { get; set; }

    /// <summary>
    /// Gets or sets whether business insights generation is allowed
    /// </summary>
    public bool AllowBusinessInsights { get; set; }

    /// <summary>
    /// Gets or sets whether price suggestions are allowed
    /// </summary>
    public bool AllowPriceSuggestions { get; set; }

    /// <summary>
    /// Gets or sets whether product description generation is allowed
    /// </summary>
    public bool AllowProductDescriptionGeneration { get; set; }

    /// <summary>
    /// Gets or sets supported languages for AI interactions (ISO codes)
    /// </summary>
    public List<string> SupportedLanguages { get; set; }

    /// <summary>
    /// Gets or sets the shop ID this setting belongs to
    /// </summary>
    public int ShopId { get; set; }
    public Store? Shop { get; set; }
}

