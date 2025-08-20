using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Metrics;

namespace Shared.LLMAdapter;

/// <summary>
/// Minimal placeholder OpenAI adapter (non-functional until API key & model configured).
/// Replace endpoint/model as needed; intentionally simple and safe.
/// </summary>
public class OpenAIAdapter : ILLMAdapter
{
    private static readonly ActivitySource ActivitySource = new("LLMAdapter");
    private readonly HttpClient _http;
    private readonly OpenAIOptions _options;
    private readonly ILogger? _logger;

    public OpenAIAdapter(HttpClient http, OpenAIOptions options, ILogger? logger = null)
    {
        _http = http;
        _options = options;
        _logger = logger;
    }

    public async Task<LLMResponse> CompleteAsync(string prompt, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey) || string.IsNullOrWhiteSpace(_options.Model))
        {
            return new LLMResponse("[openai-disabled] Missing API key or model; using fallback stub semantics for prompt: " + prompt, 0.0);
        }

        using var req = new HttpRequestMessage(HttpMethod.Post, _options.Endpoint);
        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _options.ApiKey);
        var body = new
        {
            model = _options.Model,
            messages = new[] { new { role = "user", content = prompt } },
            max_tokens = _options.MaxTokens,
            temperature = _options.Temperature
        };
        req.Content = JsonContent.Create(body);

    using var activity = ActivitySource.StartActivity("OpenAIAdapter.Complete", ActivityKind.Client);
        activity?.SetTag("llm.vendor", "openai");
        activity?.SetTag("llm.model", _options.Model);
        activity?.SetTag("llm.prompt.length", prompt.Length);
    ResilienceMetrics.OpenAiRequestsTotal.Add(1);
        try
        {
            var res = await _http.SendAsync(req, ct);
            activity?.SetTag("http.status_code", (int)res.StatusCode);
            if (!res.IsSuccessStatusCode)
            {
                _logger?.LogWarning("OpenAI non-success status {Status} {Reason}", (int)res.StatusCode, res.ReasonPhrase);
        activity?.SetTag("llm.request.outcome", "error");
        ResilienceMetrics.OpenAiRequestsFailed.Add(1);
                return new LLMResponse($"[openai-error] HTTP {(int)res.StatusCode}: {res.ReasonPhrase}", 0.0);
            }
            using var stream = await res.Content.ReadAsStreamAsync(ct);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
            var root = doc.RootElement;
            string text = root.TryGetProperty("choices", out var choices) && choices.ValueKind == JsonValueKind.Array && choices.GetArrayLength() > 0
                ? choices[0].GetProperty("message").GetProperty("content").GetString() ?? string.Empty
                : string.Empty;
            activity?.SetTag("llm.response.length", text.Length);
        activity?.SetTag("llm.request.outcome", "success");
        ResilienceMetrics.OpenAiRequestsSuccess.Add(1);
            return new LLMResponse(text, string.IsNullOrEmpty(text) ? 0.0 : 0.7);
        }
        catch (TaskCanceledException ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "timeout");
        activity?.SetTag("llm.request.outcome", "timeout");
            _logger?.LogError(ex, "OpenAI request timeout");
        ResilienceMetrics.OpenAiRequestsTimeout.Add(1);
            return new LLMResponse("[openai-timeout]", 0.0);
        }
        catch (System.Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
        activity?.SetTag("llm.request.outcome", "exception");
            _logger?.LogError(ex, "OpenAI request exception");
        ResilienceMetrics.OpenAiRequestsFailed.Add(1);
            return new LLMResponse("[openai-exception] " + ex.Message, 0.0);
        }
    }
}

public record OpenAIOptions(
    string ApiKey,
    string Model = "gpt-4o-mini",
    string Endpoint = "https://api.openai.com/v1/chat/completions",
    int MaxTokens = 128,
    double Temperature = 0.2
);
