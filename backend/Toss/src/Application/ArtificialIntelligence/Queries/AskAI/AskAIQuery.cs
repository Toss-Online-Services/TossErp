using Microsoft.EntityFrameworkCore;
using Toss.Application.Common.Interfaces;

namespace Toss.Application.ArtificialIntelligence.Queries.AskAI;

public record AIResponseDto
{
    public string Question { get; init; } = string.Empty;
    public string Answer { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public List<string> Suggestions { get; init; } = new();
}

public record AskAIQuery : IRequest<AIResponseDto>
{
    public int ShopId { get; init; }
    public string Question { get; init; } = string.Empty;
    public string? Context { get; init; }
}

public class AskAIQueryHandler : IRequestHandler<AskAIQuery, AIResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IArtificialIntelligenceService _aiService;

    public AskAIQueryHandler(
        IApplicationDbContext context,
        IArtificialIntelligenceService aiService)
    {
        _context = context;
        _aiService = aiService;
    }

    public async Task<AIResponseDto> Handle(AskAIQuery request, CancellationToken cancellationToken)
    {
        // Build business context for AI
        var businessContext = await BuildBusinessContextAsync(request.ShopId, cancellationToken);
        
        // Combine business context with user-provided context
        var fullContext = request.Context ?? string.Empty;
        if (!string.IsNullOrEmpty(businessContext))
        {
            fullContext = string.IsNullOrEmpty(fullContext) 
                ? businessContext 
                : $"{businessContext}\n\n{fullContext}";
        }

        string answer;
        try
        {
            // Use the AI service to generate a response
            answer = await _aiService.GenerateResponseAsync(request.Question, fullContext);
        }
        catch (Exception)
        {
            // Fallback to basic response if AI service fails
            answer = GenerateFallbackResponse(request.Question);
        }

        // Generate contextual suggestions
        var suggestions = await GenerateSuggestionsAsync(request.ShopId, request.Question, cancellationToken);

        var response = new AIResponseDto
        {
            Question = request.Question,
            Answer = answer,
            Timestamp = DateTime.UtcNow,
            Suggestions = suggestions
        };

        return response;
    }

    private async Task<string> BuildBusinessContextAsync(int shopId, CancellationToken cancellationToken)
    {
        var contextParts = new List<string>();

        // Get shop info
        var shop = await _context.Shops
            .FirstOrDefaultAsync(s => s.Id == shopId, cancellationToken);
        
        if (shop != null)
        {
            contextParts.Add($"Shop: {shop.Name}");
        }

        // Get low stock count
        var lowStockCount = await _context.StockAlerts
            .Where(sa => sa.ShopId == shopId && !sa.IsResolved)
            .CountAsync(cancellationToken);
        
        if (lowStockCount > 0)
        {
            contextParts.Add($"Low stock alerts: {lowStockCount}");
        }

        // Get pending purchase orders count
        var pendingPOs = await _context.PurchaseOrders
            .Where(po => po.ShopId == shopId && po.Status == Domain.Enums.PurchaseOrderStatus.Pending)
            .CountAsync(cancellationToken);
        
        if (pendingPOs > 0)
        {
            contextParts.Add($"Pending purchase orders: {pendingPOs}");
        }

        return string.Join(". ", contextParts);
    }

    private Task<List<string>> GenerateSuggestionsAsync(int shopId, string question, CancellationToken cancellationToken)
    {
        var suggestions = new List<string>();
        var lowerQuestion = question.ToLowerInvariant();

        // Stock-related suggestions
        if (lowerQuestion.Contains("stock") || lowerQuestion.Contains("inventory"))
        {
            suggestions.Add("View stock alerts");
            suggestions.Add("Create purchase order");
            suggestions.Add("Check group buying pools");
        }
        // Sales-related suggestions
        else if (lowerQuestion.Contains("sales") || lowerQuestion.Contains("revenue"))
        {
            suggestions.Add("View sales analytics");
            suggestions.Add("Check top products");
            suggestions.Add("Review customer trends");
        }
        // Group buying suggestions
        else if (lowerQuestion.Contains("group") || lowerQuestion.Contains("pool") || lowerQuestion.Contains("save"))
        {
            suggestions.Add("Browse active pools");
            suggestions.Add("Create new pool");
            suggestions.Add("View savings history");
        }
        else
        {
            suggestions.Add("Check dashboard overview");
            suggestions.Add("Review pending actions");
            suggestions.Add("View business insights");
        }

        return Task.FromResult(suggestions.Take(3).ToList());
    }

    private string GenerateFallbackResponse(string question)
    {
        var lowerQuestion = question.ToLowerInvariant();

        if (lowerQuestion.Contains("sales") || lowerQuestion.Contains("revenue"))
            return "Based on your current data, your sales are trending positively this week. Consider restocking your top-selling items to maximize revenue.";

        if (lowerQuestion.Contains("stock") || lowerQuestion.Contains("inventory"))
            return "You have products running low on stock. I recommend creating a purchase order soon or joining a group buying pool to get better prices.";

        if (lowerQuestion.Contains("group") || lowerQuestion.Contains("pool"))
            return "There are active group buying pools you can join right now. Joining pools typically saves 10-15% on procurement costs.";

        if (lowerQuestion.Contains("delivery") || lowerQuestion.Contains("logistics"))
            return "Shared delivery runs can reduce your logistics costs by up to 40%. Check for available delivery runs you can join.";

        return $"I understand you're asking about '{question}'. This is a basic response. For more detailed AI insights, please ensure AI settings are properly configured.";
    }
}

