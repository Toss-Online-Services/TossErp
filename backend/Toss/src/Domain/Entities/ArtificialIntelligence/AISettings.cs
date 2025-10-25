namespace Toss.Domain.Entities.ArtificialIntelligence;

/// <summary>
/// AI settings for TOSS business intelligence and assistant features
/// </summary>
public class AISettings : BaseEntity
{
    public AISettings()
    {
        Enabled = true;
        ProviderType = AIProviderType.OpenAI;
        AllowSalesForecasting = true;
        AllowInventoryPrediction = true;
        AllowBusinessInsights = true;
        AllowPriceSuggestions = true;
        SupportedLanguages = new List<string> { "en", "zu", "xh", "st", "tn", "af" };
        RequestTimeoutSeconds = 30;
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
    /// Gets or sets the API key for the selected provider
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the API endpoint URL (for custom providers)
    /// </summary>
    public string? ApiEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the request timeout in seconds
    /// </summary>
    public int RequestTimeoutSeconds { get; set; }

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
    public Shop? Shop { get; set; }
}

