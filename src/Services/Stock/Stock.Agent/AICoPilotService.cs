using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Aggregates.ItemAggregate;
using TossErp.Stock.Domain.Enums;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Aggregates.ItemAggregate.Entities;
using System.Text.Json;

namespace TossErp.Stock.Agent;

/// <summary>
/// AI Co-Pilot service that provides intelligent recommendations for inventory management,
/// purchasing decisions, and collaborative economy optimization
/// </summary>
public class AICoPilotService
{
    private readonly ILogger<AICoPilotService> _logger;
    private readonly GroupPurchaseAgent _groupPurchaseAgent;
    private readonly IApplicationDbContext _context;

    public AICoPilotService(
        ILogger<AICoPilotService> logger,
        GroupPurchaseAgent groupPurchaseAgent,
        IApplicationDbContext context)
    {
        _logger = logger;
        _groupPurchaseAgent = groupPurchaseAgent;
        _context = context;
    }

    /// <summary>
    /// Processes natural language queries and provides intelligent responses
    /// </summary>
    public async Task<CoPilotResponse> ProcessQueryAsync(string query, string userId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement query processing
        await Task.CompletedTask;
        return new CoPilotResponse();
    }

    /// <summary>
    /// Gets comprehensive AI recommendations for inventory management
    /// </summary>
    public async Task<InventoryRecommendations> GetInventoryRecommendations(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating AI-powered inventory recommendations");

        var recommendations = new InventoryRecommendations
        {
            ReorderRecommendations = await GetReorderRecommendations(cancellationToken),
            GroupPurchaseOpportunities = await GetGroupPurchaseOpportunities(cancellationToken),
            SharedWarehouseOpportunities = await GetSharedWarehouseOpportunities(cancellationToken),
            DemandForecasting = await GetDemandForecasting(cancellationToken),
            CostOptimization = await GetCostOptimizationRecommendations(cancellationToken),
            RiskAlerts = await GetRiskAlerts(cancellationToken)
        };

        _logger.LogInformation("Generated {RecommendationCount} AI recommendations", 
            recommendations.GetTotalRecommendationCount());

        return recommendations;
    }

    /// <summary>
    /// Gets AI recommendations for a specific item
    /// </summary>
    public async Task<ItemRecommendations> GetItemRecommendations(Guid itemId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating AI recommendations for item {ItemId}", itemId);

        var item = await _context.Items.FindAsync(itemId);
        if (item == null)
        {
            throw new ArgumentException($"Item with ID {itemId} not found");
        }

        var currentStock = await GetCurrentStockLevel(itemId, cancellationToken);
        var stockHistory = await GetStockHistory(itemId, cancellationToken);

        var recommendations = new ItemRecommendations
        {
            ItemId = itemId,
            ItemName = item.ItemName,
            CurrentStock = currentStock,
            ReorderRecommendation = await GetItemReorderRecommendation(item, currentStock, cancellationToken),
            GroupPurchaseRecommendation = await GetItemGroupPurchaseRecommendation(item, currentStock, cancellationToken),
            PricingRecommendation = await GetItemPricingRecommendation(item, stockHistory, cancellationToken),
            DemandForecast = await GetItemDemandForecast(item, stockHistory, cancellationToken),
            RiskAssessment = await GetItemRiskAssessment(item, currentStock, stockHistory, cancellationToken)
        };

        _logger.LogInformation("Generated AI recommendations for item {ItemName}", item.ItemName);
        return recommendations;
    }

    /// <summary>
    /// Gets collaborative economy insights and recommendations
    /// </summary>
    public async Task<CooperativeEconomyInsights> GetCooperativeEconomyInsights(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Analyzing collaborative economy opportunities");

        var insights = new CooperativeEconomyInsights
        {
            GroupPurchaseSavings = await CalculateGroupPurchaseSavings(cancellationToken),
            SharedWarehouseBenefits = await CalculateSharedWarehouseBenefits(cancellationToken),
            CollectiveForecastingAccuracy = await CalculateCollectiveForecastingAccuracy(cancellationToken),
            ResourcePoolingOpportunities = await GetResourcePoolingOpportunities(cancellationToken),
            CommunityCreditScoring = await GetCommunityCreditScoring(cancellationToken)
        };

        _logger.LogInformation("Generated cooperative economy insights");
        return insights;
    }

    /// <summary>
    /// Executes AI-suggested actions automatically
    /// </summary>
    public async Task<ActionExecutionResult> ExecuteSuggestedActionsAsync(Guid itemId, List<string> actions, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Executing {ActionCount} AI-suggested actions for item {ItemId}", actions.Count, itemId);

        var results = new List<ActionResult>();
        var successCount = 0;

        foreach (var action in actions)
        {
            try
            {
                var result = await ExecuteActionAsync(itemId, action, cancellationToken);
                results.Add(result);
                
                if (result.Success)
                    successCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing action {Action} for item {ItemId}", action, itemId);
                results.Add(new ActionResult
                {
                    Action = action,
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        var executionResult = new ActionExecutionResult
        {
            ItemId = itemId,
            Actions = results,
            SuccessCount = successCount,
            TotalCount = actions.Count,
            Timestamp = DateTime.UtcNow
        };

        _logger.LogInformation("Executed {SuccessCount}/{TotalCount} actions successfully for item {ItemId}", 
            successCount, actions.Count, itemId);

        return executionResult;
    }

    /// <summary>
    /// Learns from user feedback to improve recommendations
    /// </summary>
    public async Task LearnFromFeedbackAsync(string query, string response, bool wasHelpful, string userId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement learning from feedback
        await Task.CompletedTask;
    }

    // Private helper methods
    private async Task<List<ReorderRecommendation>> GetReorderRecommendations(CancellationToken cancellationToken)
    {
        var recommendations = new List<ReorderRecommendation>();

        var items = await _context.Items
            .Where(i => i.CurrentStock <= i.ReOrderLevel && !i.Disabled)
            .Take(20)
            .ToListAsync(cancellationToken);

        foreach (var item in items)
        {
            var urgency = CalculateReorderUrgency(item.CurrentStock, item.ReOrderLevel ?? 0);
            var recommendedQty = await CalculateRecommendedReorderQuantity(item, item.CurrentStock, cancellationToken);
            var reason = GetReorderReason(item.CurrentStock, item.ReOrderLevel ?? 0);

            recommendations.Add(new ReorderRecommendation
            {
                ItemId = item.Id,
                ItemName = item.ItemName,
                CurrentStock = item.CurrentStock,
                ReorderLevel = item.ReOrderLevel ?? 0,
                RecommendedQuantity = recommendedQty,
                Urgency = urgency,
                EstimatedCost = recommendedQty * (item.StandardRate ?? 0),
                Reason = reason
            });
        }

        return recommendations.OrderBy(r => r.Urgency).ToList();
    }

    private async Task<List<GroupPurchaseOpportunity>> GetGroupPurchaseOpportunities(CancellationToken cancellationToken)
    {
        return await _groupPurchaseAgent.AnalyzeGroupPurchaseOpportunities(cancellationToken);
    }

    private async Task<List<SharedWarehouseOpportunity>> GetSharedWarehouseOpportunities(CancellationToken cancellationToken)
    {
        var opportunities = new List<SharedWarehouseOpportunity>();

        var items = await _context.Items
            .Where(i => !i.Disabled)
            .Take(50)
            .ToListAsync(cancellationToken);

        foreach (var item in items)
        {
            var warehouseCount = await GetWarehouseCount(item.Id, cancellationToken);
            
            if (warehouseCount > 2) // Items in multiple warehouses
            {
                var savings = CalculateSharedWarehouseSavings(item, item.CurrentStock);
                var benefits = GetSharedWarehouseBenefits(item);

                opportunities.Add(new SharedWarehouseOpportunity
                {
                    ItemId = item.Id,
                    ItemName = item.ItemName,
                    CurrentWarehouseCount = warehouseCount,
                    RecommendedWarehouseCount = Math.Max(1, warehouseCount - 1),
                    EstimatedSavings = savings,
                    Benefits = benefits
                });
            }
        }

        return opportunities.OrderByDescending(o => o.EstimatedSavings).ToList();
    }

    private async Task<DemandForecasting> GetDemandForecasting(CancellationToken cancellationToken)
    {
        var seasonalTrends = await AnalyzeSeasonalTrends(cancellationToken);
        var growthPredictions = await AnalyzeGrowthPredictions(cancellationToken);
        var marketInsights = await GetMarketInsights(cancellationToken);

        return new DemandForecasting
        {
            SeasonalTrends = seasonalTrends,
            GrowthPredictions = growthPredictions,
            MarketInsights = marketInsights
        };
    }

    private async Task<List<CostOptimizationRecommendation>> GetCostOptimizationRecommendations(CancellationToken cancellationToken)
    {
        var recommendations = new List<CostOptimizationRecommendation>();

        var items = await _context.Items
            .Where(i => !i.Disabled)
            .Take(100)
            .ToListAsync(cancellationToken);

        foreach (var item in items)
        {
            var pricingAnalysis = await AnalyzePricingStrategy(item, cancellationToken);
            
            if (pricingAnalysis.HasOptimizationOpportunity)
            {
                recommendations.Add(new CostOptimizationRecommendation
                {
                    ItemId = item.Id,
                    ItemName = item.ItemName,
                    CurrentCost = pricingAnalysis.CurrentCost,
                    RecommendedCost = pricingAnalysis.RecommendedCost,
                    PotentialSavings = pricingAnalysis.PotentialSavings,
                    OptimizationType = pricingAnalysis.OptimizationType,
                    ImplementationDifficulty = pricingAnalysis.ImplementationDifficulty
                });
            }
        }

        return recommendations.OrderByDescending(r => r.PotentialSavings).ToList();
    }

    private async Task<List<RiskAlert>> GetRiskAlerts(CancellationToken cancellationToken)
    {
        var alerts = new List<RiskAlert>();

        var items = await _context.Items
            .Where(i => !i.Disabled)
            .Take(100)
            .ToListAsync(cancellationToken);

        foreach (var item in items)
        {
            var currentStock = await GetCurrentStockLevel(item.Id, cancellationToken);
            var stockHistory = await GetStockHistory(item.Id, cancellationToken);

            // Stockout risk
            if (currentStock <= 0)
            {
                alerts.Add(new RiskAlert
                {
                    ItemId = item.Id,
                    ItemName = item.ItemName,
                    RiskType = RiskType.StockoutRisk,
                    Severity = RiskSeverity.Critical,
                    Description = "Item is out of stock",
                    RecommendedAction = "Create urgent purchase order",
                    EstimatedImpact = "High - potential lost sales"
                });
            }
            // Low stock risk
            else if (currentStock <= item.ReOrderLevel)
            {
                alerts.Add(new RiskAlert
                {
                    ItemId = item.Id,
                    ItemName = item.ItemName,
                    RiskType = RiskType.StockoutRisk,
                    Severity = RiskSeverity.High,
                    Description = "Item stock is below reorder level",
                    RecommendedAction = "Create purchase order",
                    EstimatedImpact = "Medium - stockout risk"
                });
            }

            // Overstock risk
            var stockoutProbability = CalculateStockoutProbability(currentStock, stockHistory);
            var overstockProbability = CalculateOverstockProbability(currentStock, stockHistory);

            if (overstockProbability > 0.7m)
            {
                alerts.Add(new RiskAlert
                {
                    ItemId = item.Id,
                    ItemName = item.ItemName,
                    RiskType = RiskType.OverstockRisk,
                    Severity = RiskSeverity.Medium,
                    Description = "Item may be overstocked",
                    RecommendedAction = "Review demand and adjust reorder levels",
                    EstimatedImpact = "Medium - tied up capital"
                });
            }
        }

        return alerts.OrderByDescending(a => a.Severity).ToList();
    }

    private async Task<ReorderRecommendation?> GetItemReorderRecommendation(ItemAggregate item, decimal currentStock, CancellationToken cancellationToken)
    {
        if (item.ReOrderLevel == null || currentStock > item.ReOrderLevel.Value)
            return null;

        var urgency = CalculateReorderUrgency(currentStock, item.ReOrderLevel.Value);
        var recommendedQty = await CalculateRecommendedReorderQuantity(item, currentStock, cancellationToken);
        var reason = GetReorderReason(currentStock, item.ReOrderLevel.Value);

        return new ReorderRecommendation
        {
            ItemId = item.Id,
            ItemName = item.ItemName,
            CurrentStock = currentStock,
            ReorderLevel = item.ReOrderLevel.Value,
            RecommendedQuantity = recommendedQty,
            Urgency = urgency,
            EstimatedCost = recommendedQty * (item.StandardRate ?? 0),
            Reason = reason
        };
    }

    private async Task<GroupPurchaseRecommendation?> GetItemGroupPurchaseRecommendation(ItemAggregate item, decimal currentStock, CancellationToken cancellationToken)
    {
        // TODO: Implement group purchase recommendation
        await Task.CompletedTask;
        return null;
    }

    private async Task<PricingRecommendation> GetItemPricingRecommendation(ItemAggregate item, List<StockLedgerEntry> stockHistory, CancellationToken cancellationToken)
    {
        var currentPrice = item.StandardRate ?? 0;
        var marketPrice = await GetMarketPrice(item, cancellationToken);
        var costPrice = await GetAverageCostPrice(stockHistory, cancellationToken);

        var recommendedPrice = CalculateRecommendedPrice(currentPrice, marketPrice, costPrice);
        var priceChange = CalculatePriceChange(currentPrice, marketPrice, costPrice);
        var priceChangePercentage = CalculatePriceChangePercentage(currentPrice, marketPrice, costPrice);
        var reasoning = GetPricingReasoning(currentPrice, marketPrice, costPrice);

        return new PricingRecommendation
        {
            CurrentPrice = currentPrice,
            RecommendedPrice = recommendedPrice,
            PriceChange = priceChange,
            PriceChangePercentage = priceChangePercentage,
            Reasoning = reasoning
        };
    }

    private async Task<DemandForecast> GetItemDemandForecast(ItemAggregate item, List<StockLedgerEntry> stockHistory, CancellationToken cancellationToken)
    {
        await Task.CompletedTask; // Add await to satisfy async requirement
        
        var demandHistory = stockHistory
            .Where(s => s.Qty.Value < 0) // Outgoing stock
            .Select(s => new DemandHistory { Date = s.PostingDate, Demand = Math.Abs(s.Qty.Value) })
            .ToList();

        var nextWeekDemand = CalculateWeeklyDemand(demandHistory, 1);
        var nextMonthDemand = CalculateWeeklyDemand(demandHistory, 4) * 4;
        var confidenceLevel = CalculateForecastConfidence(demandHistory);
        var trend = AnalyzeDemandTrend(demandHistory);

        return new DemandForecast
        {
            NextWeekDemand = nextWeekDemand,
            NextMonthDemand = nextMonthDemand,
            ConfidenceLevel = confidenceLevel,
            Trend = trend
        };
    }

    private async Task<RiskAssessment> GetItemRiskAssessment(ItemAggregate item, decimal currentStock, List<StockLedgerEntry> stockHistory, CancellationToken cancellationToken)
    {
        await Task.CompletedTask; // Add await to satisfy async requirement
        
        var risks = new List<RiskFactor>();

        // Stockout risk
        var stockoutProbability = CalculateStockoutProbability(currentStock, stockHistory);
        if (stockoutProbability > 0.3m)
        {
            risks.Add(new RiskFactor
            {
                Type = RiskType.StockoutRisk,
                Severity = stockoutProbability > 0.7m ? RiskSeverity.Critical : RiskSeverity.High,
                Probability = stockoutProbability,
                Impact = "Loss of sales and customer dissatisfaction"
            });
        }

        // Overstock risk
        var overstockProbability = CalculateOverstockProbability(currentStock, stockHistory);
        if (overstockProbability > 0.3m)
        {
            risks.Add(new RiskFactor
            {
                Type = RiskType.OverstockRisk,
                Severity = overstockProbability > 0.7m ? RiskSeverity.High : RiskSeverity.Medium,
                Probability = overstockProbability,
                Impact = "Increased storage costs and potential obsolescence"
            });
        }

        // Price risk
        if (item.StandardRate.HasValue && item.StandardRate.Value > 0)
        {
            risks.Add(new RiskFactor
            {
                Type = RiskType.PriceRisk,
                Severity = RiskSeverity.Medium,
                Probability = 0.5m,
                Impact = "Market price fluctuations affecting profitability"
            });
        }

        var overallRiskLevel = CalculateOverallRiskLevel(risks);
        var mitigationStrategies = GetMitigationStrategies(risks);

        return new RiskAssessment
        {
            OverallRiskLevel = overallRiskLevel,
            RiskFactors = risks,
            MitigationStrategies = mitigationStrategies
        };
    }

    private async Task<decimal> CalculateGroupPurchaseSavings(CancellationToken cancellationToken)
    {
        // TODO: Implement group purchase savings calculation
        await Task.CompletedTask;
        return 0;
    }

    private async Task<decimal> CalculateSharedWarehouseBenefits(CancellationToken cancellationToken)
    {
        // TODO: Implement shared warehouse benefits calculation
        await Task.CompletedTask;
        return 0;
    }

    private async Task<decimal> CalculateCollectiveForecastingAccuracy(CancellationToken cancellationToken)
    {
        // TODO: Implement collective forecasting accuracy calculation
        await Task.CompletedTask;
        return 0;
    }

    private async Task<List<ResourcePoolingOpportunity>> GetResourcePoolingOpportunities(CancellationToken cancellationToken)
    {
        // TODO: Implement resource pooling opportunities
        await Task.CompletedTask;
        return new List<ResourcePoolingOpportunity>();
    }

    private async Task<CommunityCreditScoring> GetCommunityCreditScoring(CancellationToken cancellationToken)
    {
        // TODO: Implement community credit scoring
        await Task.CompletedTask;
        return new CommunityCreditScoring();
    }

    private async Task<decimal> GetCurrentStockLevel(Guid itemId, CancellationToken cancellationToken)
    {
        var item = await _context.Items.FindAsync(itemId, cancellationToken);
        return item?.CurrentStock ?? 0;
    }

    private async Task<List<StockLedgerEntry>> GetStockHistory(Guid itemId, CancellationToken cancellationToken)
    {
        return await _context.StockLedgerEntries
            .Where(s => s.Item.Id == itemId)
            .OrderByDescending(s => s.PostingDate)
            .Take(100)
            .ToListAsync(cancellationToken);
    }

    private async Task<int> GetWarehouseCount(Guid itemId, CancellationToken cancellationToken)
    {
        return await _context.StockLedgerEntries
            .Where(s => s.Item.Id == itemId)
            .Select(s => s.Warehouse.Id)
            .Distinct()
            .CountAsync(cancellationToken);
    }

    private ReorderUrgency CalculateReorderUrgency(decimal currentStock, decimal reorderLevel) => 
        currentStock <= 0 ? ReorderUrgency.Critical :
        currentStock <= reorderLevel * 0.5m ? ReorderUrgency.High :
        currentStock <= reorderLevel ? ReorderUrgency.Medium : ReorderUrgency.Low;

    private async Task<decimal> CalculateRecommendedReorderQuantity(ItemAggregate item, decimal currentStock, CancellationToken cancellationToken)
    {
        await Task.CompletedTask; // Add await to satisfy async requirement
        return Math.Max(item.ReOrderQty ?? 10, (item.ReOrderLevel ?? 0) - currentStock + 10);
    }

    private string GetReorderReason(decimal currentStock, decimal reorderLevel) =>
        currentStock <= 0 ? "Item is out of stock" :
        currentStock <= reorderLevel * 0.5m ? "Stock critically low" :
        currentStock <= reorderLevel ? "Stock below reorder level" : "Preventive reorder";

    private decimal CalculateSharedWarehouseSavings(ItemAggregate item, decimal currentStock) =>
        currentStock * 0.05m; // 5% savings estimate

    private List<string> GetSharedWarehouseBenefits(ItemAggregate item) =>
        new List<string> { "Reduced storage costs", "Better inventory distribution", "Improved availability" };

    private async Task<SeasonalTrends> AnalyzeSeasonalTrends(CancellationToken cancellationToken)
    {
        // TODO: Implement seasonal trends analysis
        await Task.CompletedTask;
        return new SeasonalTrends();
    }

    private async Task<GrowthPredictions> AnalyzeGrowthPredictions(CancellationToken cancellationToken)
    {
        // TODO: Implement growth predictions analysis
        await Task.CompletedTask;
        return new GrowthPredictions();
    }

    private async Task<MarketInsights> GetMarketInsights(CancellationToken cancellationToken)
    {
        // TODO: Implement market insights
        await Task.CompletedTask;
        return new MarketInsights();
    }

    private async Task<PricingAnalysis> AnalyzePricingStrategy(ItemAggregate item, CancellationToken cancellationToken)
    {
        // TODO: Implement pricing strategy analysis
        await Task.CompletedTask;
        return new PricingAnalysis();
    }

    private async Task<decimal> GetMarketPrice(ItemAggregate item, CancellationToken cancellationToken)
    {
        // TODO: Implement market price retrieval
        await Task.CompletedTask;
        return item.StandardRate ?? 0;
    }

    private async Task<decimal> GetAverageCostPrice(List<StockLedgerEntry> stockHistory, CancellationToken cancellationToken)
    {
        await Task.CompletedTask; // Add await to satisfy async requirement
        
        if (!stockHistory.Any()) return 0;

        var incomingEntries = stockHistory
            .Where(s => s.IsIncoming() && s.ValuationRate.Value > 0)
            .ToList();

        if (!incomingEntries.Any()) return 0;

        var totalValue = incomingEntries.Sum(s => s.StockValue.Value);
        var totalQuantity = incomingEntries.Sum(s => s.Qty.Value);

        return totalQuantity > 0 ? totalValue / totalQuantity : 0;
    }

    private decimal CalculateRecommendedPrice(decimal currentPrice, decimal marketPrice, decimal costPrice) =>
        Math.Max(costPrice * 1.2m, marketPrice * 0.95m);

    private decimal CalculatePriceChange(decimal currentPrice, decimal marketPrice, decimal costPrice) =>
        CalculateRecommendedPrice(currentPrice, marketPrice, costPrice) - currentPrice;

    private decimal CalculatePriceChangePercentage(decimal currentPrice, decimal marketPrice, decimal costPrice) =>
        currentPrice > 0 ? (CalculatePriceChange(currentPrice, marketPrice, costPrice) / currentPrice) * 100 : 0;

    private string GetPricingReasoning(decimal currentPrice, decimal marketPrice, decimal costPrice)
    {
        if (currentPrice < costPrice) return "Price below cost - consider increasing";
        if (currentPrice > marketPrice * 1.1m) return "Price significantly above market - consider reducing";
        if (currentPrice < marketPrice * 0.9m) return "Price below market - opportunity to increase";
        return "Price is competitive";
    }

    private decimal CalculateWeeklyDemand(List<DemandHistory> historicalDemand, int weeks)
    {
        if (!historicalDemand.Any()) return 0;

        var recentDemand = historicalDemand
            .OrderByDescending(d => d.Date)
            .Take(weeks * 7)
            .ToList();

        return recentDemand.Any() ? recentDemand.Average(d => d.Demand) : 0;
    }

    private decimal CalculateForecastConfidence(List<DemandHistory> historicalDemand)
    {
        if (historicalDemand.Count < 7) return 0.5m;

        var demands = historicalDemand.Select(d => d.Demand).ToList();
        var variance = demands.Variance();
        var mean = demands.Average();

        return mean > 0 ? Math.Max(0, 1 - (variance / (mean * mean))) : 0.5m;
    }

    private string AnalyzeDemandTrend(List<DemandHistory> historicalDemand)
    {
        if (historicalDemand.Count < 14) return "Insufficient data";

        var recent = historicalDemand.OrderByDescending(d => d.Date).Take(7).Average(d => d.Demand);
        var previous = historicalDemand.OrderByDescending(d => d.Date).Skip(7).Take(7).Average(d => d.Demand);

        if (recent > previous * 1.1m) return "Increasing";
        if (recent < previous * 0.9m) return "Decreasing";
        return "Stable";
    }

    private decimal CalculateStockoutProbability(decimal currentStock, List<StockLedgerEntry> stockHistory)
    {
        if (!stockHistory.Any()) return 0.1m;

        var weeklyDemand = CalculateWeeklyDemand(
            stockHistory.Select(s => new DemandHistory { Date = s.PostingDate, Demand = s.Qty.Value }).ToList(), 
            4);

        if (weeklyDemand <= 0) return 0;

        var weeksUntilStockout = currentStock / weeklyDemand;
        return weeksUntilStockout < 2 ? 0.8m : weeksUntilStockout < 4 ? 0.4m : 0.1m;
    }

    private decimal CalculateOverstockProbability(decimal currentStock, List<StockLedgerEntry> stockHistory)
    {
        if (!stockHistory.Any()) return 0.1m;

        var weeklyDemand = CalculateWeeklyDemand(
            stockHistory.Select(s => new DemandHistory { Date = s.PostingDate, Demand = s.Qty.Value }).ToList(), 
            4);

        if (weeklyDemand <= 0) return 0.5m;

        var monthsOfStock = currentStock / (weeklyDemand * 4);
        return monthsOfStock > 6 ? 0.8m : monthsOfStock > 3 ? 0.4m : 0.1m;
    }

    private RiskLevel CalculateOverallRiskLevel(List<RiskFactor> risks)
    {
        if (!risks.Any()) return RiskLevel.Low;

        var highRiskCount = risks.Count(r => r.Severity == RiskSeverity.High || r.Severity == RiskSeverity.Critical);
        var totalRisks = risks.Count;

        if (highRiskCount >= totalRisks * 0.5m) return RiskLevel.High;
        if (highRiskCount >= totalRisks * 0.2m) return RiskLevel.Medium;
        return RiskLevel.Low;
    }

    private List<string> GetMitigationStrategies(List<RiskFactor> risks)
    {
        var strategies = new List<string>();

        foreach (var risk in risks.Where(r => r.Severity == RiskSeverity.High || r.Severity == RiskSeverity.Critical))
        {
            switch (risk.Type)
            {
                case RiskType.StockoutRisk:
                    strategies.Add("Increase reorder level and quantity");
                    break;
                case RiskType.OverstockRisk:
                    strategies.Add("Reduce reorder quantity and implement promotions");
                    break;
                case RiskType.PriceRisk:
                    strategies.Add("Review pricing strategy and monitor market trends");
                    break;
            }
        }

        return strategies;
    }

    private async Task<decimal> CalculateWarehouseUtilization(Guid warehouseId, CancellationToken cancellationToken)
    {
        // TODO: Implement warehouse utilization calculation
        await Task.CompletedTask;
        return 0.75m; // Placeholder
    }

    private async Task<ActionResult> ExecuteActionAsync(Guid itemId, string action, CancellationToken cancellationToken)
    {
        // TODO: Implement action execution
        await Task.CompletedTask;
        return new ActionResult
        {
            Action = action,
            Success = true,
            Message = "Action executed successfully",
            Timestamp = DateTime.UtcNow
        };
    }
}

// Model classes for AI Co-Pilot
public class CoPilotResponse
{
    public string Query { get; set; } = string.Empty;
    public string Response { get; set; } = string.Empty;
    public float Confidence { get; set; }
    public List<string> Suggestions { get; set; } = new();
    public List<CoPilotAction> Actions { get; set; } = new();
    public DateTime Timestamp { get; set; }
}

public class CoPilotAction
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ActionType Type { get; set; }
    public float Confidence { get; set; }
}

public class ActionExecutionResult
{
    public Guid ItemId { get; set; }
    public List<ActionResult> Actions { get; set; } = new();
    public int SuccessCount { get; set; }
    public int TotalCount { get; set; }
    public DateTime Timestamp { get; set; }
}

public class ActionResult
{
    public string Action { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime Timestamp { get; set; }
}

public enum ActionType
{
    PurchaseOrder,
    GroupPurchase,
    PricingAnalysis,
    StockTransfer,
    SupplierChange
}

// Inventory recommendation models
public class InventoryRecommendations
{
    public List<ReorderRecommendation> ReorderRecommendations { get; set; } = new();
    public List<GroupPurchaseOpportunity> GroupPurchaseOpportunities { get; set; } = new();
    public List<SharedWarehouseOpportunity> SharedWarehouseOpportunities { get; set; } = new();
    public DemandForecasting DemandForecasting { get; set; } = new();
    public List<CostOptimizationRecommendation> CostOptimization { get; set; } = new();
    public List<RiskAlert> RiskAlerts { get; set; } = new();

    public int GetTotalRecommendationCount() =>
        ReorderRecommendations.Count + GroupPurchaseOpportunities.Count + 
        SharedWarehouseOpportunities.Count + CostOptimization.Count + RiskAlerts.Count;
}

public class ReorderRecommendation
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public decimal ReorderLevel { get; set; }
    public decimal RecommendedQuantity { get; set; }
    public ReorderUrgency Urgency { get; set; }
    public decimal EstimatedCost { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public enum ReorderUrgency
{
    Low,
    Medium,
    High,
    Critical
}

public class SharedWarehouseOpportunity
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int CurrentWarehouseCount { get; set; }
    public int RecommendedWarehouseCount { get; set; }
    public decimal EstimatedSavings { get; set; }
    public List<string> Benefits { get; set; } = new();
}

public class DemandForecasting
{
    public SeasonalTrends SeasonalTrends { get; set; } = new();
    public GrowthPredictions GrowthPredictions { get; set; } = new();
    public MarketInsights MarketInsights { get; set; } = new();
}

public class SeasonalTrends
{
    public bool HasSeasonalPattern { get; set; }
    public string PeakSeason { get; set; } = string.Empty;
    public string LowSeason { get; set; } = string.Empty;
}

public class GrowthPredictions
{
    public decimal PredictedGrowth { get; set; }
    public decimal ConfidenceLevel { get; set; }
}

public class MarketInsights
{
    public string MarketTrend { get; set; } = string.Empty;
    public string CompetitionLevel { get; set; } = string.Empty;
    public string PriceTrend { get; set; } = string.Empty;
}

public class CostOptimizationRecommendation
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal CurrentCost { get; set; }
    public decimal RecommendedCost { get; set; }
    public decimal PotentialSavings { get; set; }
    public string OptimizationType { get; set; } = string.Empty;
    public ImplementationDifficulty ImplementationDifficulty { get; set; }
}

public enum ImplementationDifficulty
{
    Easy,
    Medium,
    Hard
}

public class RiskAlert
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public RiskType RiskType { get; set; }
    public RiskSeverity Severity { get; set; }
    public string Description { get; set; } = string.Empty;
    public string RecommendedAction { get; set; } = string.Empty;
    public string EstimatedImpact { get; set; } = string.Empty;
}

public enum RiskType
{
    StockoutRisk,
    OverstockRisk,
    PriceRisk
}

public enum RiskSeverity
{
    Low,
    Medium,
    High,
    Critical
}

// Item recommendation models
public class ItemRecommendations
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public ReorderRecommendation? ReorderRecommendation { get; set; }
    public GroupPurchaseRecommendation? GroupPurchaseRecommendation { get; set; }
    public PricingRecommendation PricingRecommendation { get; set; } = new();
    public DemandForecast DemandForecast { get; set; } = new();
    public RiskAssessment RiskAssessment { get; set; } = new();
}

public class GroupPurchaseRecommendation
{
    public bool IsRecommended { get; set; }
    public int ParticipatingBusinesses { get; set; }
    public decimal EstimatedSavings { get; set; }
    public decimal BulkUnitPrice { get; set; }
    public decimal IndividualUnitPrice { get; set; }
    public GroupPurchasePriority Priority { get; set; }
}



public class PricingRecommendation
{
    public decimal CurrentPrice { get; set; }
    public decimal RecommendedPrice { get; set; }
    public decimal PriceChange { get; set; }
    public decimal PriceChangePercentage { get; set; }
    public string Reasoning { get; set; } = string.Empty;
}

public class DemandForecast
{
    public decimal NextWeekDemand { get; set; }
    public decimal NextMonthDemand { get; set; }
    public decimal ConfidenceLevel { get; set; }
    public string Trend { get; set; } = string.Empty;
}

public class RiskAssessment
{
    public RiskLevel OverallRiskLevel { get; set; }
    public List<RiskFactor> RiskFactors { get; set; } = new();
    public List<string> MitigationStrategies { get; set; } = new();
}

public enum RiskLevel
{
    Low,
    Medium,
    High
}

public class RiskFactor
{
    public RiskType Type { get; set; }
    public RiskSeverity Severity { get; set; }
    public decimal Probability { get; set; }
    public string Impact { get; set; } = string.Empty;
}

public class DemandHistory
{
    public DateTime Date { get; set; }
    public decimal Demand { get; set; }
}

// Cooperative economy models
public class CooperativeEconomyInsights
{
    public decimal GroupPurchaseSavings { get; set; }
    public decimal SharedWarehouseBenefits { get; set; }
    public decimal CollectiveForecastingAccuracy { get; set; }
    public List<ResourcePoolingOpportunity> ResourcePoolingOpportunities { get; set; } = new();
    public CommunityCreditScoring CommunityCreditScoring { get; set; } = new();
}

public class ResourcePoolingOpportunity
{
    public string ResourceType { get; set; } = string.Empty;
    public decimal CurrentUtilization { get; set; }
    public decimal PotentialUtilization { get; set; }
    public decimal EstimatedSavings { get; set; }
    public ImplementationDifficulty ImplementationDifficulty { get; set; }
}

public class CommunityCreditScoring
{
    public int AverageCreditScore { get; set; }
    public int CreditworthyMembers { get; set; }
    public int TotalMembers { get; set; }
    public int AverageTransactionHistory { get; set; }
    public decimal DefaultRate { get; set; }
    public decimal RecommendedCreditLimit { get; set; }
}

// Pricing analysis models
public class PricingAnalysis
{
    public bool HasOptimizationOpportunity { get; set; }
    public decimal CurrentCost { get; set; }
    public decimal RecommendedCost { get; set; }
    public decimal PotentialSavings { get; set; }
    public string OptimizationType { get; set; } = string.Empty;
    public ImplementationDifficulty ImplementationDifficulty { get; set; }
}

// Extension method for variance calculation
public static class EnumerableExtensions
{
    public static decimal Variance(this IEnumerable<decimal> values)
    {
        var list = values.ToList();
        if (list.Count == 0) return 0;
        
        var mean = list.Average();
        var variance = list.Select(x => (x - mean) * (x - mean)).Average();
        return variance;
    }
} 
