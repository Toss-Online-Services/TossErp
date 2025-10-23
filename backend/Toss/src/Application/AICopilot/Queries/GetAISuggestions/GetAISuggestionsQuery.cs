using Toss.Application.Common.Interfaces;

namespace Toss.Application.AICopilot.Queries.GetAISuggestions;

public record AISuggestionDto
{
    public string Type { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string ActionUrl { get; init; } = string.Empty;
    public int Priority { get; init; } = 1;
}

public record GetAISuggestionsQuery : IRequest<List<AISuggestionDto>>
{
    public int ShopId { get; init; }
    public int MaxSuggestions { get; init; } = 5;
}

public class GetAISuggestionsQueryHandler : IRequestHandler<GetAISuggestionsQuery, List<AISuggestionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAISuggestionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AISuggestionDto>> Handle(GetAISuggestionsQuery request, CancellationToken cancellationToken)
    {
        // TODO: Integrate with actual AI service for intelligent suggestions
        // For now, return rule-based suggestions

        var suggestions = new List<AISuggestionDto>();

        // Check for low stock
        var lowStockCount = await _context.StockAlerts
            .Where(sa => sa.ShopId == request.ShopId && !sa.IsResolved)
            .CountAsync(cancellationToken);

        if (lowStockCount > 0)
        {
            suggestions.Add(new AISuggestionDto
            {
                Type = "StockAlert",
                Title = $"{lowStockCount} Low Stock Alerts",
                Description = "You have products running low. Consider restocking soon.",
                ActionUrl = "/inventory/alerts",
                Priority = 3
            });
        }

        // Check for open group buy pools
        var openPools = await _context.GroupBuyPools
            .Where(p => p.Status == Domain.Enums.PoolStatus.Open && p.CreatorShopId != request.ShopId)
            .CountAsync(cancellationToken);

        if (openPools > 0)
        {
            suggestions.Add(new AISuggestionDto
            {
                Type = "GroupBuyOpportunity",
                Title = $"{openPools} Group Buying Opportunities",
                Description = "Join pools to save up to 15% on procurement costs.",
                ActionUrl = "/group-buying/pools",
                Priority = 2
            });
        }

        // Check for pending purchase orders
        var pendingPOs = await _context.PurchaseOrders
            .Where(po => po.ShopId == request.ShopId && po.Status == Domain.Enums.PurchaseOrderStatus.Pending)
            .CountAsync(cancellationToken);

        if (pendingPOs > 0)
        {
            suggestions.Add(new AISuggestionDto
            {
                Type = "PendingAction",
                Title = $"{pendingPOs} Pending Purchase Orders",
                Description = "Review and approve pending purchase orders.",
                ActionUrl = "/buying/purchase-orders",
                Priority = 2
            });
        }

        // Add generic suggestions
        suggestions.Add(new AISuggestionDto
        {
            Type = "Analytics",
            Title = "Check Your Sales Trends",
            Description = "View your sales performance over the last 30 days.",
            ActionUrl = "/dashboard/analytics",
            Priority = 1
        });

        return suggestions
            .OrderByDescending(s => s.Priority)
            .Take(request.MaxSuggestions)
            .ToList();
    }
}

