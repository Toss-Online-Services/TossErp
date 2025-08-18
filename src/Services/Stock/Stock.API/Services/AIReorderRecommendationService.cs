using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Entities;

namespace TossErp.Stock.API.Services;

/// <summary>
/// AI-powered reorder recommendation service
/// </summary>
public class AIReorderRecommendationService : IAIReorderRecommendationService
{
    private readonly IItemRepository _itemRepository;
    private readonly IStockLevelRepository _stockLevelRepository;
    private readonly IStockMovementRepository _movementRepository;
    private readonly ILogger<AIReorderRecommendationService> _logger;

    public AIReorderRecommendationService(
        IItemRepository itemRepository,
        IStockLevelRepository stockLevelRepository,
        IStockMovementRepository movementRepository,
        ILogger<AIReorderRecommendationService> logger)
    {
        _itemRepository = itemRepository;
        _stockLevelRepository = stockLevelRepository;
        _movementRepository = movementRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get AI-powered reorder recommendations for all items
    /// </summary>
    public async Task<List<ReorderRecommendationDto>> GetReorderRecommendationsAsync(
        string tenantId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating reorder recommendations for tenant {TenantId}", tenantId);

            var items = await _itemRepository.GetAllAsync(cancellationToken);
            var stockLevels = await _stockLevelRepository.GetAllAsync(cancellationToken);
            var movements = await _movementRepository.GetAllAsync(cancellationToken);

            var recommendations = new List<ReorderRecommendationDto>();
            var cutoffDate = DateTime.UtcNow.AddDays(-90); // Look at last 90 days of movement

            foreach (var item in items)
            {
                var stockLevel = stockLevels.FirstOrDefault(sl => sl.ItemId == item.Id);
                if (stockLevel == null) continue;

                var itemMovements = movements
                    .Where(m => m.ItemId == item.Id && m.CreatedAt >= cutoffDate)
                    .OrderBy(m => m.CreatedAt)
                    .ToList();

                var recommendation = await GenerateRecommendationForItemAsync(
                    item, stockLevel, itemMovements, cancellationToken);

                if (recommendation != null)
                {
                    recommendations.Add(recommendation);
                }
            }

            _logger.LogInformation("Generated {Count} reorder recommendations for tenant {TenantId}", 
                recommendations.Count, tenantId);

            return recommendations.OrderByDescending(r => r.Priority)
                                .ThenBy(r => r.DaysUntilStockout)
                                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating reorder recommendations for tenant {TenantId}", tenantId);
            throw;
        }
    }

    /// <summary>
    /// Get AI-powered reorder recommendation for a specific item
    /// </summary>
    public async Task<ReorderRecommendationDto?> GetItemReorderRecommendationAsync(
        Guid itemId, 
        string tenantId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var item = await _itemRepository.GetByIdAsync(itemId, cancellationToken);
            if (item == null) return null;

            var stockLevels = await _stockLevelRepository.GetByItemAsync(itemId, cancellationToken);
            var stockLevelsList = stockLevels.ToList();
            if (!stockLevelsList.Any()) return null;

            // Aggregate stock levels across all warehouses
            var totalStock = stockLevelsList.Sum(sl => sl.Quantity);
            var avgUnitCost = stockLevelsList.Average(sl => sl.UnitCost);
            var primaryWarehouseId = stockLevelsList.OrderByDescending(sl => sl.Quantity).First().WarehouseId;

            // Create aggregated stock level for analysis
            var aggregatedStockLevel = new StockLevel
            {
                ItemId = itemId,
                WarehouseId = primaryWarehouseId,
                Quantity = totalStock,
                UnitCost = avgUnitCost,
                LastUpdated = stockLevelsList.Max(sl => sl.LastUpdated)
            };

            var cutoffDate = DateTime.UtcNow.AddDays(-90);
            var movements = await _movementRepository.GetByItemAsync(itemId, cancellationToken);
            var recentMovements = movements
                .Where(m => m.CreatedAt >= cutoffDate)
                .OrderBy(m => m.CreatedAt)
                .ToList();

            return await GenerateRecommendationForItemAsync(
                item, aggregatedStockLevel, recentMovements, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating reorder recommendation for item {ItemId}", itemId);
            throw;
        }
    }

    /// <summary>
    /// Generate AI recommendation for a specific item
    /// </summary>
    private async Task<ReorderRecommendationDto?> GenerateRecommendationForItemAsync(
        ItemAggregate item,
        StockLevel stockLevel,
        List<StockMovement> movements,
        CancellationToken cancellationToken)
    {
        try
        {
            // Calculate consumption patterns
            var consumptionAnalysis = AnalyzeConsumptionPattern(movements);
            
            // Determine if reorder is needed
            var reorderAnalysis = AnalyzeReorderNeed(item, stockLevel, consumptionAnalysis);
            
            if (!reorderAnalysis.ShouldReorder)
            {
                return null; // No recommendation needed
            }

            // Calculate optimal order quantity using AI-enhanced logic
            var optimalQuantity = CalculateOptimalOrderQuantity(item, stockLevel, consumptionAnalysis);
            
            // Calculate estimated cost
            var estimatedCost = optimalQuantity * (item.StandardRate ?? stockLevel.UnitCost);
            
            // Determine priority
            var priority = DeterminePriority(stockLevel, consumptionAnalysis, reorderAnalysis);
            
            // Generate reasoning
            var reasoning = GenerateRecommendationReasoning(item, stockLevel, consumptionAnalysis, reorderAnalysis);

            return new ReorderRecommendationDto
            {
                ItemId = item.Id,
                ItemCode = item.ItemCode,
                ItemName = item.ItemName,
                Category = item.ItemGroup,
                CurrentStock = stockLevel.Quantity,
                ReorderLevel = item.ReorderLevel ?? 0,
                RecommendedQuantity = optimalQuantity,
                EstimatedCost = estimatedCost,
                Priority = priority,
                DaysUntilStockout = reorderAnalysis.DaysUntilStockout,
                AverageDailyConsumption = consumptionAnalysis.AverageDailyConsumption,
                ConsumptionTrend = consumptionAnalysis.Trend,
                LeadTime = item.LeadTime ?? 7, // Default 7 days if not specified
                Reasoning = reasoning,
                GeneratedAt = DateTime.UtcNow,
                ConfidenceScore = CalculateConfidenceScore(movements.Count, consumptionAnalysis.Variability)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating recommendation for item {ItemId}", item.Id);
            return null;
        }
    }

    /// <summary>
    /// Analyze consumption patterns from movement history
    /// </summary>
    private ConsumptionAnalysis AnalyzeConsumptionPattern(List<StockMovement> movements)
    {
        if (movements.Count == 0)
        {
            return new ConsumptionAnalysis
            {
                AverageDailyConsumption = 0,
                Trend = "Unknown",
                Variability = 1.0m,
                Seasonality = "None"
            };
        }

        // Calculate daily consumption (outgoing movements only)
        var outgoingMovements = movements.Where(m => m.Quantity < 0).ToList();
        var dailyConsumption = new Dictionary<DateTime, decimal>();

        foreach (var movement in outgoingMovements)
        {
            var date = movement.CreatedAt.Date;
            if (!dailyConsumption.ContainsKey(date))
                dailyConsumption[date] = 0;
            
            dailyConsumption[date] += Math.Abs(movement.Quantity);
        }

        if (dailyConsumption.Count == 0)
        {
            return new ConsumptionAnalysis
            {
                AverageDailyConsumption = 0,
                Trend = "No consumption",
                Variability = 0,
                Seasonality = "None"
            };
        }

        var averageDaily = dailyConsumption.Values.Average();
        var variability = dailyConsumption.Count > 1 ? 
            CalculateVariability(dailyConsumption.Values.ToList()) : 0;

        // Determine trend
        var trend = DetermineTrend(dailyConsumption);

        // Determine seasonality (simplified)
        var seasonality = DetectSeasonality(dailyConsumption);

        return new ConsumptionAnalysis
        {
            AverageDailyConsumption = averageDaily,
            Trend = trend,
            Variability = variability,
            Seasonality = seasonality
        };
    }

    /// <summary>
    /// Analyze if reorder is needed
    /// </summary>
    private ReorderAnalysis AnalyzeReorderNeed(
        ItemAggregate item, 
        StockLevel stockLevel, 
        ConsumptionAnalysis consumption)
    {
        var reorderLevel = item.ReorderLevel ?? 10; // Default reorder level
        var leadTime = item.LeadTime ?? 7; // Default lead time in days
        
        var shouldReorder = stockLevel.Quantity <= reorderLevel;
        
        // Calculate days until stockout
        var daysUntilStockout = consumption.AverageDailyConsumption > 0 
            ? (int)(stockLevel.Quantity / consumption.AverageDailyConsumption)
            : int.MaxValue;

        // Consider lead time in recommendation
        var urgentReorder = daysUntilStockout <= leadTime;

        return new ReorderAnalysis
        {
            ShouldReorder = shouldReorder || urgentReorder,
            DaysUntilStockout = daysUntilStockout,
            IsUrgent = urgentReorder,
            ReorderLevel = reorderLevel
        };
    }

    /// <summary>
    /// Calculate optimal order quantity using AI-enhanced logic
    /// </summary>
    private decimal CalculateOptimalOrderQuantity(
        ItemAggregate item, 
        StockLevel stockLevel, 
        ConsumptionAnalysis consumption)
    {
        var reorderQuantity = item.ReorderQuantity ?? item.ReorderLevel ?? 50;
        var leadTime = item.LeadTime ?? 7;
        
        // Economic Order Quantity (EOQ) inspired calculation
        var demandDuringLeadTime = consumption.AverageDailyConsumption * leadTime;
        var safetyStock = demandDuringLeadTime * 0.2m; // 20% safety stock
        
        // Adjust for consumption trend
        var trendMultiplier = consumption.Trend switch
        {
            "Increasing" => 1.2m,
            "Decreasing" => 0.8m,
            _ => 1.0m
        };
        
        // Adjust for variability
        var variabilityMultiplier = 1 + (consumption.Variability * 0.1m);
        
        var optimalQuantity = Math.Max(
            reorderQuantity * trendMultiplier * variabilityMultiplier,
            demandDuringLeadTime + safetyStock
        );
        
        return Math.Round(optimalQuantity, 2);
    }

    /// <summary>
    /// Determine recommendation priority
    /// </summary>
    private ReorderPriority DeterminePriority(
        StockLevel stockLevel, 
        ConsumptionAnalysis consumption, 
        ReorderAnalysis reorderAnalysis)
    {
        if (stockLevel.Quantity <= 0)
            return ReorderPriority.Critical;
        
        if (reorderAnalysis.IsUrgent)
            return ReorderPriority.High;
        
        if (reorderAnalysis.DaysUntilStockout <= 14)
            return ReorderPriority.Medium;
        
        return ReorderPriority.Low;
    }

    /// <summary>
    /// Generate human-readable reasoning for the recommendation
    /// </summary>
    private string GenerateRecommendationReasoning(
        ItemAggregate item,
        StockLevel stockLevel,
        ConsumptionAnalysis consumption,
        ReorderAnalysis reorderAnalysis)
    {
        var reasons = new List<string>();
        
        if (stockLevel.Quantity <= 0)
        {
            reasons.Add("Item is out of stock");
        }
        else if (stockLevel.Quantity <= item.ReorderLevel)
        {
            reasons.Add($"Current stock ({stockLevel.Quantity}) is at or below reorder level ({item.ReorderLevel})");
        }
        
        if (reorderAnalysis.DaysUntilStockout < (item.LeadTime ?? 7))
        {
            reasons.Add($"Stock will run out in {reorderAnalysis.DaysUntilStockout} days, which is less than lead time");
        }
        
        if (consumption.Trend == "Increasing")
        {
            reasons.Add("Consumption trend is increasing");
        }
        
        if (consumption.Variability > 0.5m)
        {
            reasons.Add("High consumption variability requires safety stock");
        }
        
        return string.Join(". ", reasons) + ".";
    }

    /// <summary>
    /// Calculate confidence score based on data quality
    /// </summary>
    private decimal CalculateConfidenceScore(int movementCount, decimal variability)
    {
        var dataScore = Math.Min(movementCount / 30.0m, 1.0m); // More data = higher confidence
        var variabilityScore = Math.Max(1.0m - variability, 0.1m); // Lower variability = higher confidence
        
        return Math.Round((dataScore + variabilityScore) / 2 * 100, 1);
    }

    /// <summary>
    /// Calculate variability coefficient
    /// </summary>
    private decimal CalculateVariability(List<decimal> values)
    {
        if (values.Count <= 1) return 0;
        
        var mean = values.Average();
        if (mean == 0) return 0;
        
        var variance = values.Sum(v => (decimal)Math.Pow((double)(v - mean), 2)) / values.Count;
        var standardDeviation = (decimal)Math.Sqrt((double)variance);
        
        return standardDeviation / mean; // Coefficient of variation
    }

    /// <summary>
    /// Determine consumption trend
    /// </summary>
    private string DetermineTrend(Dictionary<DateTime, decimal> dailyConsumption)
    {
        if (dailyConsumption.Count < 7) return "Insufficient data";
        
        var orderedData = dailyConsumption.OrderBy(kv => kv.Key).ToList();
        var firstHalf = orderedData.Take(orderedData.Count / 2).Select(kv => kv.Value).Average();
        var secondHalf = orderedData.Skip(orderedData.Count / 2).Select(kv => kv.Value).Average();
        
        var changePercent = firstHalf > 0 ? (secondHalf - firstHalf) / firstHalf : 0;
        
        return changePercent switch
        {
            > 0.1m => "Increasing",
            < -0.1m => "Decreasing",
            _ => "Stable"
        };
    }

    /// <summary>
    /// Detect seasonality patterns (simplified)
    /// </summary>
    private string DetectSeasonality(Dictionary<DateTime, decimal> dailyConsumption)
    {
        // This is a simplified seasonality detection
        // In a real AI system, this would use more sophisticated algorithms
        
        if (dailyConsumption.Count < 30) return "Insufficient data";
        
        var weekdayAvg = dailyConsumption
            .Where(kv => kv.Key.DayOfWeek != DayOfWeek.Saturday && kv.Key.DayOfWeek != DayOfWeek.Sunday)
            .Average(kv => kv.Value);
            
        var weekendAvg = dailyConsumption
            .Where(kv => kv.Key.DayOfWeek == DayOfWeek.Saturday || kv.Key.DayOfWeek == DayOfWeek.Sunday)
            .Average(kv => kv.Value);
        
        if (Math.Abs(weekdayAvg - weekendAvg) / Math.Max(weekdayAvg, weekendAvg) > 0.3m)
        {
            return weekdayAvg > weekendAvg ? "Higher weekday consumption" : "Higher weekend consumption";
        }
        
        return "No clear pattern";
    }
}

/// <summary>
/// Analysis of consumption patterns
/// </summary>
public class ConsumptionAnalysis
{
    public decimal AverageDailyConsumption { get; set; }
    public string Trend { get; set; } = string.Empty;
    public decimal Variability { get; set; }
    public string Seasonality { get; set; } = string.Empty;
}

/// <summary>
/// Analysis of reorder needs
/// </summary>
public class ReorderAnalysis
{
    public bool ShouldReorder { get; set; }
    public int DaysUntilStockout { get; set; }
    public bool IsUrgent { get; set; }
    public decimal ReorderLevel { get; set; }
}

/// <summary>
/// Reorder recommendation priority levels
/// </summary>
public enum ReorderPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

/// <summary>
/// AI-powered reorder recommendation DTO
/// </summary>
public class ReorderRecommendationDto
{
    public Guid ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public decimal ReorderLevel { get; set; }
    public decimal RecommendedQuantity { get; set; }
    public decimal EstimatedCost { get; set; }
    public ReorderPriority Priority { get; set; }
    public int DaysUntilStockout { get; set; }
    public decimal AverageDailyConsumption { get; set; }
    public string ConsumptionTrend { get; set; } = string.Empty;
    public int LeadTime { get; set; }
    public string Reasoning { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public decimal ConfidenceScore { get; set; }
}
