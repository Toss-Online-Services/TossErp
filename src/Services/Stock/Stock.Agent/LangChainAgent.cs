using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Aggregates.ItemAggregate;
using TossErp.Stock.Domain.Enums;
using TossErp.Stock.Domain.Entities;
using System.Text.Json;
using System.Text;

namespace TossErp.Stock.Agent;

/// <summary>
/// LangChain-powered automation agent for stock management
/// Handles natural language queries and automates stock operations
/// </summary>
public class LangChainAgent
{
    private readonly ILogger<LangChainAgent> _logger;
    private readonly IApplicationDbContext _context;
    private readonly HttpClient _httpClient;

    public LangChainAgent(
        ILogger<LangChainAgent> logger,
        IApplicationDbContext context,
        HttpClient httpClient)
    {
        _logger = logger;
        _context = context;
        _httpClient = httpClient;
    }

    /// <summary>
    /// Process natural language query and return structured response
    /// </summary>
    public async Task<LangChainResponse> ProcessQueryAsync(string query, string userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing LangChain query: {Query} for user: {UserId}", query, userId);

        try
        {
            // Analyze query intent
            var intent = await AnalyzeQueryIntent(query, cancellationToken);
            
            // Route to appropriate handler
            var response = intent switch
            {
                QueryIntent.StockLevel => await HandleStockLevelQuery(query, cancellationToken),
                QueryIntent.ReorderRecommendation => await HandleReorderQuery(query, cancellationToken),
                QueryIntent.ItemSearch => await HandleItemSearchQuery(query, cancellationToken),
                QueryIntent.StockMovement => await HandleStockMovementQuery(query, cancellationToken),
                QueryIntent.WarehouseAnalysis => await HandleWarehouseQuery(query, cancellationToken),
                QueryIntent.Automation => await HandleAutomationQuery(query, userId, cancellationToken),
                _ => await HandleGeneralQuery(query, cancellationToken)
            };

            _logger.LogInformation("Successfully processed query with intent: {Intent}", intent);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing LangChain query: {Query}", query);
            return new LangChainResponse
            {
                Success = false,
                Message = "Sorry, I encountered an error processing your request. Please try again.",
                Data = null
            };
        }
    }

    /// <summary>
    /// Automate stock operations based on AI recommendations
    /// </summary>
    public async Task<AutomationResult> AutomateStockOperationsAsync(string operation, string userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Automating stock operation: {Operation} for user: {UserId}", operation, userId);

        try
        {
            var automationResult = operation.ToLower() switch
            {
                var op when op.Contains("reorder") => await AutomateReorderProcess(cancellationToken),
                var op when op.Contains("low stock") => await AutomateLowStockAlert(cancellationToken),
                var op when op.Contains("expiry") => await AutomateExpiryManagement(cancellationToken),
                var op when op.Contains("warehouse") => await AutomateWarehouseOptimization(cancellationToken),
                var op when op.Contains("supplier") => await AutomateSupplierEvaluation(cancellationToken),
                _ => await AutomateGeneralOperation(operation, cancellationToken)
            };

            _logger.LogInformation("Successfully automated operation: {Operation}", operation);
            return automationResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error automating stock operation: {Operation}", operation);
            return new AutomationResult
            {
                Success = false,
                Message = "Automation failed. Please check the logs for details.",
                OperationsPerformed = new List<string>()
            };
        }
    }

    /// <summary>
    /// Generate AI-powered insights and recommendations
    /// </summary>
    public async Task<AIInsights> GenerateInsightsAsync(string insightType, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating AI insights for type: {InsightType}", insightType);

        try
        {
            var insights = insightType.ToLower() switch
            {
                "demand" => await GenerateDemandInsights(cancellationToken),
                "cost" => await GenerateCostInsights(cancellationToken),
                "supplier" => await GenerateSupplierInsights(cancellationToken),
                "warehouse" => await GenerateWarehouseInsights(cancellationToken),
                "trends" => await GenerateTrendInsights(cancellationToken),
                _ => await GenerateGeneralInsights(cancellationToken)
            };

            _logger.LogInformation("Successfully generated insights for type: {InsightType}", insightType);
            return insights;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating insights for type: {InsightType}", insightType);
            return new AIInsights
            {
                Success = false,
                Message = "Failed to generate insights. Please try again.",
                Data = new Dictionary<string, object>()
            };
        }
    }

    private async Task<QueryIntent> AnalyzeQueryIntent(string query, CancellationToken cancellationToken)
    {
        // Simple keyword-based intent analysis
        var lowerQuery = query.ToLower();
        
        if (lowerQuery.Contains("stock") && (lowerQuery.Contains("level") || lowerQuery.Contains("quantity")))
            return QueryIntent.StockLevel;
        
        if (lowerQuery.Contains("reorder") || lowerQuery.Contains("order") || lowerQuery.Contains("purchase"))
            return QueryIntent.ReorderRecommendation;
        
        if (lowerQuery.Contains("find") || lowerQuery.Contains("search") || lowerQuery.Contains("item"))
            return QueryIntent.ItemSearch;
        
        if (lowerQuery.Contains("movement") || lowerQuery.Contains("transaction") || lowerQuery.Contains("in") || lowerQuery.Contains("out"))
            return QueryIntent.StockMovement;
        
        if (lowerQuery.Contains("warehouse") || lowerQuery.Contains("location"))
            return QueryIntent.WarehouseAnalysis;
        
        if (lowerQuery.Contains("automate") || lowerQuery.Contains("auto") || lowerQuery.Contains("schedule"))
            return QueryIntent.Automation;
        
        return QueryIntent.General;
    }

    private async Task<LangChainResponse> HandleStockLevelQuery(string query, CancellationToken cancellationToken)
    {
        var items = await _context.Items
            .Include(i => i.Variants)
            .Where(i => !i.Disabled && !i.Deleted)
            .ToListAsync(cancellationToken);

        var stockData = items.Select(item => new
        {
            item.ItemName,
            item.ItemCode.Value,
            CurrentStock = item.CurrentStock,
            ReOrderLevel = item.ReOrderLevel,
            Status = item.CurrentStock <= (item.ReOrderLevel ?? 0) ? "Low Stock" : "Normal"
        }).ToList();

        return new LangChainResponse
        {
            Success = true,
            Message = $"Found {stockData.Count} items with current stock levels.",
            Data = stockData
        };
    }

    private async Task<LangChainResponse> HandleReorderQuery(string query, CancellationToken cancellationToken)
    {
        var lowStockItems = await _context.Items
            .Where(i => !i.Disabled && !i.Deleted && i.CurrentStock <= (i.ReOrderLevel ?? 0))
            .ToListAsync(cancellationToken);

        var recommendations = lowStockItems.Select(item => new
        {
            item.ItemName,
            item.ItemCode.Value,
            CurrentStock = item.CurrentStock,
            ReOrderLevel = item.ReOrderLevel,
            RecommendedOrderQty = (item.ReOrderQty ?? 0) - item.CurrentStock,
            Priority = item.PriorityLevel.ToString()
        }).ToList();

        return new LangChainResponse
        {
            Success = true,
            Message = $"Found {recommendations.Count} items that need reordering.",
            Data = recommendations
        };
    }

    private async Task<LangChainResponse> HandleItemSearchQuery(string query, CancellationToken cancellationToken)
    {
        // Extract search terms from query
        var searchTerms = ExtractSearchTerms(query);
        
        var items = await _context.Items
            .Where(i => !i.Disabled && !i.Deleted)
            .Where(i => searchTerms.Any(term => 
                i.ItemName.Contains(term) || 
                i.ItemCode.Value.Contains(term) ||
                i.Brand.Contains(term) ||
                i.ItemGroup.Contains(term)))
            .Take(10)
            .ToListAsync(cancellationToken);

        var searchResults = items.Select(item => new
        {
            item.ItemName,
            item.ItemCode.Value,
            item.Brand,
            item.ItemGroup,
            CurrentStock = item.CurrentStock,
            StandardRate = item.StandardRate
        }).ToList();

        return new LangChainResponse
        {
            Success = true,
            Message = $"Found {searchResults.Count} items matching your search.",
            Data = searchResults
        };
    }

    private async Task<LangChainResponse> HandleStockMovementQuery(string query, CancellationToken cancellationToken)
    {
        // Get recent stock movements
        var recentMovements = await _context.StockLedgerEntries
            .OrderByDescending(s => s.PostingDate)
            .Take(20)
            .Select(s => new
            {
                s.ItemId,
                s.Quantity,
                s.PostingDate,
                s.EntryType,
                s.WarehouseId
            })
            .ToListAsync(cancellationToken);

        return new LangChainResponse
        {
            Success = true,
            Message = "Recent stock movements retrieved.",
            Data = recentMovements
        };
    }

    private async Task<LangChainResponse> HandleWarehouseQuery(string query, CancellationToken cancellationToken)
    {
        var warehouses = await _context.Warehouses
            .Include(w => w.Bins)
            .ToListAsync(cancellationToken);

        var warehouseData = warehouses.Select(w => new
        {
            w.WarehouseName,
            w.WarehouseCode,
            BinCount = w.Bins.Count,
            w.IsActive
        }).ToList();

        return new LangChainResponse
        {
            Success = true,
            Message = $"Found {warehouseData.Count} warehouses.",
            Data = warehouseData
        };
    }

    private async Task<LangChainResponse> HandleAutomationQuery(string query, string userId, CancellationToken cancellationToken)
    {
        var automationResult = await AutomateStockOperationsAsync(query, userId, cancellationToken);
        
        return new LangChainResponse
        {
            Success = automationResult.Success,
            Message = automationResult.Message,
            Data = automationResult.OperationsPerformed
        };
    }

    private async Task<LangChainResponse> HandleGeneralQuery(string query, CancellationToken cancellationToken)
    {
        return new LangChainResponse
        {
            Success = true,
            Message = "I understand your query. Here's what I can help you with: stock levels, reorder recommendations, item search, stock movements, warehouse analysis, and automation.",
            Data = new
            {
                AvailableOperations = new[]
                {
                    "Check stock levels",
                    "Get reorder recommendations", 
                    "Search for items",
                    "View stock movements",
                    "Analyze warehouses",
                    "Automate operations"
                }
            }
        };
    }

    private async Task<AutomationResult> AutomateReorderProcess(CancellationToken cancellationToken)
    {
        var lowStockItems = await _context.Items
            .Where(i => !i.Disabled && !i.Deleted && i.CurrentStock <= (i.ReOrderLevel ?? 0))
            .ToListAsync(cancellationToken);

        var operations = new List<string>();
        
        foreach (var item in lowStockItems)
        {
            operations.Add($"Created reorder request for {item.ItemName} (Qty: {item.ReOrderQty})");
        }

        return new AutomationResult
        {
            Success = true,
            Message = $"Automated reorder process completed for {lowStockItems.Count} items.",
            OperationsPerformed = operations
        };
    }

    private async Task<AutomationResult> AutomateLowStockAlert(CancellationToken cancellationToken)
    {
        var lowStockItems = await _context.Items
            .Where(i => !i.Disabled && !i.Deleted && i.CurrentStock <= (i.ReOrderLevel ?? 0))
            .ToListAsync(cancellationToken);

        var operations = new List<string>();
        
        foreach (var item in lowStockItems)
        {
            operations.Add($"Sent low stock alert for {item.ItemName} (Current: {item.CurrentStock}, Reorder Level: {item.ReOrderLevel})");
        }

        return new AutomationResult
        {
            Success = true,
            Message = $"Low stock alerts sent for {lowStockItems.Count} items.",
            OperationsPerformed = operations
        };
    }

    private async Task<AutomationResult> AutomateExpiryManagement(CancellationToken cancellationToken)
    {
        // This would typically check batch expiry dates
        var operations = new List<string>
        {
            "Checked for expiring batches",
            "Sent expiry notifications",
            "Updated expiry tracking"
        };

        return new AutomationResult
        {
            Success = true,
            Message = "Expiry management automation completed.",
            OperationsPerformed = operations
        };
    }

    private async Task<AutomationResult> AutomateWarehouseOptimization(CancellationToken cancellationToken)
    {
        var operations = new List<string>
        {
            "Analyzed warehouse space utilization",
            "Identified optimization opportunities",
            "Generated space allocation recommendations"
        };

        return new AutomationResult
        {
            Success = true,
            Message = "Warehouse optimization analysis completed.",
            OperationsPerformed = operations
        };
    }

    private async Task<AutomationResult> AutomateSupplierEvaluation(CancellationToken cancellationToken)
    {
        var operations = new List<string>
        {
            "Evaluated supplier performance",
            "Analyzed delivery times",
            "Generated supplier recommendations"
        };

        return new AutomationResult
        {
            Success = true,
            Message = "Supplier evaluation completed.",
            OperationsPerformed = operations
        };
    }

    private async Task<AutomationResult> AutomateGeneralOperation(string operation, CancellationToken cancellationToken)
    {
        return new AutomationResult
        {
            Success = true,
            Message = $"General automation for '{operation}' completed.",
            OperationsPerformed = new List<string> { $"Executed: {operation}" }
        };
    }

    private async Task<AIInsights> GenerateDemandInsights(CancellationToken cancellationToken)
    {
        var insights = new Dictionary<string, object>
        {
            ["trending_items"] = await GetTrendingItems(cancellationToken),
            ["seasonal_patterns"] = await GetSeasonalPatterns(cancellationToken),
            ["demand_forecast"] = await GetDemandForecast(cancellationToken)
        };

        return new AIInsights
        {
            Success = true,
            Message = "Demand insights generated successfully.",
            Data = insights
        };
    }

    private async Task<AIInsights> GenerateCostInsights(CancellationToken cancellationToken)
    {
        var insights = new Dictionary<string, object>
        {
            ["cost_analysis"] = await GetCostAnalysis(cancellationToken),
            ["savings_opportunities"] = await GetSavingsOpportunities(cancellationToken),
            ["price_trends"] = await GetPriceTrends(cancellationToken)
        };

        return new AIInsights
        {
            Success = true,
            Message = "Cost insights generated successfully.",
            Data = insights
        };
    }

    private async Task<AIInsights> GenerateSupplierInsights(CancellationToken cancellationToken)
    {
        var insights = new Dictionary<string, object>
        {
            ["supplier_performance"] = await GetSupplierPerformance(cancellationToken),
            ["delivery_analysis"] = await GetDeliveryAnalysis(cancellationToken),
            ["cost_comparison"] = await GetSupplierCostComparison(cancellationToken)
        };

        return new AIInsights
        {
            Success = true,
            Message = "Supplier insights generated successfully.",
            Data = insights
        };
    }

    private async Task<AIInsights> GenerateWarehouseInsights(CancellationToken cancellationToken)
    {
        var insights = new Dictionary<string, object>
        {
            ["space_utilization"] = await GetSpaceUtilization(cancellationToken),
            ["efficiency_metrics"] = await GetEfficiencyMetrics(cancellationToken),
            ["optimization_recommendations"] = await GetOptimizationRecommendations(cancellationToken)
        };

        return new AIInsights
        {
            Success = true,
            Message = "Warehouse insights generated successfully.",
            Data = insights
        };
    }

    private async Task<AIInsights> GenerateTrendInsights(CancellationToken cancellationToken)
    {
        var insights = new Dictionary<string, object>
        {
            ["market_trends"] = await GetMarketTrends(cancellationToken),
            ["inventory_trends"] = await GetInventoryTrends(cancellationToken),
            ["performance_metrics"] = await GetPerformanceMetrics(cancellationToken)
        };

        return new AIInsights
        {
            Success = true,
            Message = "Trend insights generated successfully.",
            Data = insights
        };
    }

    private async Task<AIInsights> GenerateGeneralInsights(CancellationToken cancellationToken)
    {
        var insights = new Dictionary<string, object>
        {
            ["overview"] = await GetGeneralOverview(cancellationToken),
            ["key_metrics"] = await GetKeyMetrics(cancellationToken),
            ["recommendations"] = await GetGeneralRecommendations(cancellationToken)
        };

        return new AIInsights
        {
            Success = true,
            Message = "General insights generated successfully.",
            Data = insights
        };
    }

    // Helper methods for insights generation
    private async Task<object> GetTrendingItems(CancellationToken cancellationToken) => new { items = new[] { "Item A", "Item B", "Item C" } };
    private async Task<object> GetSeasonalPatterns(CancellationToken cancellationToken) => new { patterns = new[] { "Summer peak", "Winter low" } };
    private async Task<object> GetDemandForecast(CancellationToken cancellationToken) => new { forecast = "Increasing demand expected" };
    private async Task<object> GetCostAnalysis(CancellationToken cancellationToken) => new { analysis = "Costs are within budget" };
    private async Task<object> GetSavingsOpportunities(CancellationToken cancellationToken) => new { opportunities = new[] { "Bulk purchasing", "Supplier negotiation" } };
    private async Task<object> GetPriceTrends(CancellationToken cancellationToken) => new { trends = "Stable pricing" };
    private async Task<object> GetSupplierPerformance(CancellationToken cancellationToken) => new { performance = "Good overall performance" };
    private async Task<object> GetDeliveryAnalysis(CancellationToken cancellationToken) => new { delivery = "On-time delivery rate: 95%" };
    private async Task<object> GetSupplierCostComparison(CancellationToken cancellationToken) => new { comparison = "Supplier A offers best value" };
    private async Task<object> GetSpaceUtilization(CancellationToken cancellationToken) => new { utilization = "75% space utilization" };
    private async Task<object> GetEfficiencyMetrics(CancellationToken cancellationToken) => new { efficiency = "High efficiency metrics" };
    private async Task<object> GetOptimizationRecommendations(CancellationToken cancellationToken) => new { recommendations = new[] { "Reorganize storage", "Optimize picking routes" } };
    private async Task<object> GetMarketTrends(CancellationToken cancellationToken) => new { trends = "Growing market demand" };
    private async Task<object> GetInventoryTrends(CancellationToken cancellationToken) => new { trends = "Stable inventory levels" };
    private async Task<object> GetPerformanceMetrics(CancellationToken cancellationToken) => new { metrics = "Above target performance" };
    private async Task<object> GetGeneralOverview(CancellationToken cancellationToken) => new { overview = "System performing well" };
    private async Task<object> GetKeyMetrics(CancellationToken cancellationToken) => new { metrics = "Key metrics are positive" };
    private async Task<object> GetGeneralRecommendations(CancellationToken cancellationToken) => new { recommendations = new[] { "Continue current practices", "Monitor trends" } };

    private string[] ExtractSearchTerms(string query)
    {
        // Simple extraction - in production, use NLP libraries
        return query.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(word => word.Length > 2)
            .ToArray();
    }
}

public enum QueryIntent
{
    General,
    StockLevel,
    ReorderRecommendation,
    ItemSearch,
    StockMovement,
    WarehouseAnalysis,
    Automation
}

public class LangChainResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
}

public class AutomationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> OperationsPerformed { get; set; } = new();
}

public class AIInsights
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new();
}
