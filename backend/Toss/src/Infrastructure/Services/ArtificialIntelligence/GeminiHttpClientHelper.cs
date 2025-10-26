using System.Text;
using System.Text.Json;
using Toss.Domain.Entities.ArtificialIntelligence;

namespace Toss.Infrastructure.Services.ArtificialIntelligence;

/// <summary>
/// Represents the HTTP client helper to create Gemini requests
/// </summary>
public class GeminiHttpClientHelper : IArtificialIntelligenceHttpClientHelper
{
    /// <summary>
    /// Configure client
    /// </summary>
    /// <param name="httpClient">HTTP client for configuration</param>
    public virtual void ConfigureClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri(ArtificialIntelligenceDefaults.GeminiBaseApiUrl);
    }

    /// <summary>
    /// Create HTTP request
    /// </summary>
    /// <param name="settings">Artificial intelligence settings</param>
    /// <param name="query">query to send into artificial intelligence host</param>
    /// <returns>Created HttpRequestMessage</returns>
    public virtual HttpRequestMessage CreateRequest(AISettings settings, string query)
    {
        var request = new HttpRequestMessage { Method = HttpMethod.Post };
        request.Headers.TryAddWithoutValidation(ArtificialIntelligenceDefaults.GeminiApiKeyHeader, settings.GeminiApiKey);

        var data = JsonSerializer.Serialize(new
        {
            contents = new { parts = new[] { new { text = query } } }
        });

        request.Content = new StringContent(data, Encoding.UTF8, "application/json");

        return request;
    }

    /// <summary>
    /// Parse response
    /// </summary>
    /// <param name="responseText">Response text to parse</param>
    /// <returns>Generated text from artificial intelligence host</returns>
    public virtual string ParseResponse(string responseText)
    {
        var response = JsonSerializer.Deserialize<GeminiResponse>(responseText);
        
        var result = response?.candidates?.FirstOrDefault()?.content?.parts?.FirstOrDefault()?.text;

        return result ?? string.Empty;
    }

    private record GeminiResponse(
        GeminiCandidate[]? candidates
    );

    private record GeminiCandidate(
        GeminiContent? content
    );

    private record GeminiContent(
        GeminiPart[]? parts
    );

    private record GeminiPart(
        string? text
    );
}

