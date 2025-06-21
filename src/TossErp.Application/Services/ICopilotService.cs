using TossErp.Application.DTOs;

namespace TossErp.Application.Services
{
    /// <summary>
    /// AI Copilot service interface for business intelligence and decision support
    /// </summary>
    public interface ICopilotService
    {
        /// <summary>
        /// Process a natural language query and provide intelligent response
        /// </summary>
        Task<CopilotResponseDto> ProcessQueryAsync(CopilotQueryDto query, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get business insights and recommendations
        /// </summary>
        Task<CopilotResponseDto> GetBusinessInsightsAsync(Guid businessId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get inventory recommendations
        /// </summary>
        Task<CopilotResponseDto> GetInventoryRecommendationsAsync(Guid businessId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get sales insights and trends
        /// </summary>
        Task<CopilotResponseDto> GetSalesInsightsAsync(Guid businessId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get financial recommendations
        /// </summary>
        Task<CopilotResponseDto> GetFinancialRecommendationsAsync(Guid businessId, CancellationToken cancellationToken = default);
    }
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
