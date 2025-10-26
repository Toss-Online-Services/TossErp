using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Toss.Domain.Entities.ArtificialIntelligence;

namespace Toss.Infrastructure.Services.ArtificialIntelligence;

/// <summary>
/// Represents the HTTP client helper to create DeepSeek requests
/// </summary>
public class DeepSeekHttpClientHelper : IArtificialIntelligenceHttpClientHelper
{
    /// <summary>
    /// Configure client
    /// </summary>
    /// <param name="httpClient">HTTP client for configuration</param>
    public virtual void ConfigureClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri(ArtificialIntelligenceDefaults.DeepSeekBaseApiUrl);
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
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", settings.DeepSeekApiKey);

        var data = JsonSerializer.Serialize(new
        {
            model = ArtificialIntelligenceDefaults.DeepSeekApiModel,
            messages = new[]
            {
                new { role = "user", content = query }
            }
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
        var response = JsonSerializer.Deserialize<DeepSeekResponse>(responseText);

        var result = response?.choices?.FirstOrDefault()?.message?.content;

        return result ?? string.Empty;
    }

    private record DeepSeekResponse(
        DeepSeekChoice[]? choices
    );

    private record DeepSeekChoice(
        DeepSeekMessage? message
    );

    private record DeepSeekMessage(
        string? content
    );
}

