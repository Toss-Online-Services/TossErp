namespace TossErp.Application.Services;

public interface ICopilotService
{
    Task<string> ProcessQueryAsync(string query, Guid businessId, Guid? userId = null);
    Task<string> GetSalesInsightsAsync(Guid businessId, DateTime fromDate, DateTime toDate);
    Task<string> GetInventoryAlertsAsync(Guid businessId);
    Task<string> GetGroupPurchaseSuggestionsAsync(Guid businessId);
    Task<string> GetPromotionSuggestionsAsync(Guid businessId);
    Task<string> GetBusinessRecommendationsAsync(Guid businessId);
}

public class CopilotQuery
{
    public string Query { get; set; } = string.Empty;
    public Guid BusinessId { get; set; }
    public Guid? UserId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Context { get; set; }
}

public class CopilotResponse
{
    public string Response { get; set; } = string.Empty;
    public string Type { get; set; } = "text"; // text, action, suggestion
    public List<string> Actions { get; set; } = new();
    public List<string> Suggestions { get; set; } = new();
    public bool RequiresAction { get; set; } = false;
} 
