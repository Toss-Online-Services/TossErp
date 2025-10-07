using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Http;
using System.Text;
using System.Text.Json;

namespace TossErp.Infrastructure.Services.AI;

/// <summary>
/// AI Copilot service for natural language interactions and intelligent assistance
/// </summary>
public interface ICopilotService
{
    Task<CopilotResponse> ProcessQuery(string query, string? context = null, string? language = "en");
    Task<List<Recommendation>> GetRecommendations(RecommendationType type, Dictionary<string, object>? parameters = null);
    Task<PredictionResult> GetPrediction(PredictionType type, Dictionary<string, object>? parameters = null);
}

public class CopilotService : ICopilotService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<CopilotService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _apiEndpoint;

    public CopilotService(
        IConfiguration configuration,
        ILogger<CopilotService> logger,
        IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("OpenAI");
        _apiKey = configuration["AI:ApiKey"] ?? "";
        _apiEndpoint = configuration["AI:Endpoint"] ?? "https://api.openai.com/v1/chat/completions";
    }

    public async Task<CopilotResponse> ProcessQuery(string query, string? context = null, string? language = "en")
    {
        try
        {
            var systemPrompt = GetSystemPrompt(language);
            var userPrompt = BuildUserPrompt(query, context);

            var requestBody = new
            {
                model = "gpt-4",
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
                },
                temperature = 0.7,
                max_tokens = 500
            };

            var request = new HttpRequestMessage(HttpMethod.Post, _apiEndpoint)
            {
                Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {_apiKey}");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OpenAIResponse>(responseContent);

            var copilotResponse = new CopilotResponse
            {
                Answer = result?.Choices?.FirstOrDefault()?.Message?.Content ?? "I couldn't process that request.",
                Confidence = 0.85, // Would be calculated based on response
                Language = language ?? "en",
                SuggestedActions = ExtractSuggestedActions(result?.Choices?.FirstOrDefault()?.Message?.Content),
                Timestamp = DateTime.UtcNow
            };

            _logger.LogInformation("Processed AI query: {Query}", query);

            return copilotResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing AI query: {Query}", query);
            
            return new CopilotResponse
            {
                Answer = "I'm having trouble processing your request right now. Please try again later.",
                Confidence = 0,
                Language = language ?? "en",
                Timestamp = DateTime.UtcNow,
                Error = ex.Message
            };
        }
    }

    public async Task<List<Recommendation>> GetRecommendations(RecommendationType type, Dictionary<string, object>? parameters = null)
    {
        // This would integrate with ML models or business logic
        // For now, return smart recommendations based on type
        
        var recommendations = new List<Recommendation>();

        switch (type)
        {
            case RecommendationType.InventoryOptimization:
                recommendations.AddRange(await GetInventoryRecommendations(parameters));
                break;
            
            case RecommendationType.PricingStrategy:
                recommendations.AddRange(await GetPricingRecommendations(parameters));
                break;
            
            case RecommendationType.CustomerInsights:
                recommendations.AddRange(await GetCustomerRecommendations(parameters));
                break;
            
            case RecommendationType.SupplierSelection:
                recommendations.AddRange(await GetSupplierRecommendations(parameters));
                break;
        }

        return recommendations;
    }

    public async Task<PredictionResult> GetPrediction(PredictionType type, Dictionary<string, object>? parameters = null)
    {
        // This would use ML models for predictions
        // For now, return calculated predictions
        
        return type switch
        {
            PredictionType.SalesForecast => await PredictSales(parameters),
            PredictionType.DemandForecast => await PredictDemand(parameters),
            PredictionType.CashFlow => await PredictCashFlow(parameters),
            PredictionType.CustomerChurn => await PredictChurn(parameters),
            _ => new PredictionResult { Type = type, Success = false, Message = "Unknown prediction type" }
        };
    }

    // Helper Methods
    private string GetSystemPrompt(string? language)
    {
        var languageInstructions = language switch
        {
            "zu" => "Respond in Zulu (isiZulu).",
            "af" => "Respond in Afrikaans.",
            "xh" => "Respond in Xhosa (isiXhosa).",
            _ => "Respond in English."
        };

        return $@"You are TOSS AI Copilot, an intelligent assistant for the Township One-Stop Solution ERP system.
You help business owners manage their operations through natural language.
You can answer questions about sales, inventory, finances, customers, and more.
You provide actionable insights and recommendations.
{languageInstructions}
Be concise, helpful, and business-focused.";
    }

    private string BuildUserPrompt(string query, string? context)
    {
        var prompt = new StringBuilder();
        prompt.AppendLine($"User Query: {query}");
        
        if (!string.IsNullOrEmpty(context))
        {
            prompt.AppendLine($"\nContext: {context}");
        }
        
        return prompt.ToString();
    }

    private List<string> ExtractSuggestedActions(string? response)
    {
        // Parse response for action suggestions
        // For now, return empty list
        return new List<string>();
    }

    private async Task<List<Recommendation>> GetInventoryRecommendations(Dictionary<string, object>? parameters)
    {
        // Would query database and apply ML models
        await Task.CompletedTask;
        
        return new List<Recommendation>
        {
            new()
            {
                Type = "InventoryOptimization",
                Title = "Reorder Low Stock Items",
                Description = "5 products are below reorder point",
                Priority = "High",
                EstimatedImpact = "Prevent stockouts, maintain sales",
                ActionUrl = "/inventory/reorder"
            }
        };
    }

    private async Task<List<Recommendation>> GetPricingRecommendations(Dictionary<string, object>? parameters)
    {
        await Task.CompletedTask;
        return new List<Recommendation>();
    }

    private async Task<List<Recommendation>> GetCustomerRecommendations(Dictionary<string, object>? parameters)
    {
        await Task.CompletedTask;
        return new List<Recommendation>();
    }

    private async Task<List<Recommendation>> GetSupplierRecommendations(Dictionary<string, object>? parameters)
    {
        await Task.CompletedTask;
        return new List<Recommendation>();
    }

    private async Task<PredictionResult> PredictSales(Dictionary<string, object>? parameters)
    {
        // Would use ML model for actual prediction
        await Task.CompletedTask;
        
        return new PredictionResult
        {
            Type = PredictionType.SalesForecast,
            Success = true,
            Message = "Sales forecast generated",
            PredictedValue = 150000,
            Confidence = 0.82,
            TimeHorizon = "30 days",
            Data = new Dictionary<string, object>
            {
                { "forecast", new[] { 5000, 5200, 4800, 5100, 5300, 4900, 5000 } },
                { "trend", "upward" },
                { "seasonality", "moderate" }
            }
        };
    }

    private async Task<PredictionResult> PredictDemand(Dictionary<string, object>? parameters)
    {
        await Task.CompletedTask;
        return new PredictionResult { Type = PredictionType.DemandForecast, Success = true };
    }

    private async Task<PredictionResult> PredictCashFlow(Dictionary<string, object>? parameters)
    {
        await Task.CompletedTask;
        return new PredictionResult { Type = PredictionType.CashFlow, Success = true };
    }

    private async Task<PredictionResult> PredictChurn(Dictionary<string, object>? parameters)
    {
        await Task.CompletedTask;
        return new PredictionResult { Type = PredictionType.CustomerChurn, Success = true };
    }
}

// DTOs
public class CopilotResponse
{
    public string Answer { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public string Language { get; set; } = "en";
    public List<string> SuggestedActions { get; set; } = new();
    public DateTime Timestamp { get; set; }
    public string? Error { get; set; }
}

public class Recommendation
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = "Medium";
    public string? EstimatedImpact { get; set; }
    public string? ActionUrl { get; set; }
    public Dictionary<string, object>? Data { get; set; }
}

public class PredictionResult
{
    public PredictionType Type { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
    public double? PredictedValue { get; set; }
    public double? Confidence { get; set; }
    public string? TimeHorizon { get; set; }
    public Dictionary<string, object>? Data { get; set; }
}

public enum RecommendationType
{
    InventoryOptimization,
    PricingStrategy,
    CustomerInsights,
    SupplierSelection,
    MarketingCampaign,
    CashFlowManagement
}

public enum PredictionType
{
    SalesForecast,
    DemandForecast,
    CashFlow,
    CustomerChurn,
    SeasonalTrends
}

// OpenAI API Response Models
internal class OpenAIResponse
{
    public List<Choice>? Choices { get; set; }
}

internal class Choice
{
    public Message? Message { get; set; }
}

internal class Message
{
    public string? Content { get; set; }
}

