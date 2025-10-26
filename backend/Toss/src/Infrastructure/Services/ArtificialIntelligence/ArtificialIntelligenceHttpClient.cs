using Toss.Domain.Entities.ArtificialIntelligence;

namespace Toss.Infrastructure.Services.ArtificialIntelligence;

/// <summary>
/// Represents the HTTP client to request artificial intelligence
/// </summary>
public class ArtificialIntelligenceHttpClient
{
    private readonly IAISettingsService _aiSettingsService;
    private readonly HttpClient _httpClient;
    private IArtificialIntelligenceHttpClientHelper? _artificialIntelligenceHttpClientHelper;
    private AISettings? _artificialIntelligenceSettings;

    public ArtificialIntelligenceHttpClient(
        IAISettingsService aiSettingsService,
        HttpClient httpClient)
    {
        _aiSettingsService = aiSettingsService;
        _httpClient = httpClient;
    }

    protected virtual async Task InitializeAsync()
    {
        if (_artificialIntelligenceHttpClientHelper != null)
            return;

        _artificialIntelligenceSettings = await _aiSettingsService.GetSettingsAsync();

        // Configure client
        _httpClient.Timeout = TimeSpan.FromSeconds(_artificialIntelligenceSettings.RequestTimeout ?? 
            ArtificialIntelligenceDefaults.RequestTimeout);

        _artificialIntelligenceHttpClientHelper = _artificialIntelligenceSettings.ProviderType switch
        {
            AIProviderType.Gemini => new GeminiHttpClientHelper(),
            AIProviderType.ChatGpt => new ChatGptHttpClientHelper(),
            AIProviderType.DeepSeek => new DeepSeekHttpClientHelper(),
            _ => throw new ArgumentOutOfRangeException(nameof(_artificialIntelligenceSettings.ProviderType))
        };

        _artificialIntelligenceHttpClientHelper.ConfigureClient(_httpClient);
    }

    /// <summary>
    /// Send query to artificial intelligence host
    /// </summary>
    /// <param name="query">Query to artificial intelligence host</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the response from the artificial intelligence host
    /// </returns>
    public virtual async Task<string> SendQueryAsync(string query)
    {
        await InitializeAsync();

        if (_artificialIntelligenceHttpClientHelper == null || _artificialIntelligenceSettings == null)
            throw new InvalidOperationException("AI client not initialized");

        var request = _artificialIntelligenceHttpClientHelper.CreateRequest(_artificialIntelligenceSettings, query);

        var httpResponse = await _httpClient.SendAsync(request);
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new ApplicationException($"AI request failed: {httpResponse.ReasonPhrase}. Response: {response}");

        var result = _artificialIntelligenceHttpClientHelper.ParseResponse(response);

        return result;
    }
}

