namespace TossErp.AI.Agents;

/// <summary>
/// Autonomous agent for inventory management services
/// </summary>
public interface IInventoryAgent
{
    /// <summary>
    /// Automatically monitors stock levels and places reorders
    /// </summary>
    Task<InventoryActionResult> MonitorAndReorderAsync(string userId);
    
    /// <summary>
    /// Analyzes demand patterns and optimizes inventory
    /// </summary>
    Task<InventoryActionResult> OptimizeInventoryAsync(string userId);
    
    /// <summary>
    /// Handles stock adjustments automatically
    /// </summary>
    Task<InventoryActionResult> HandleStockAdjustmentAsync(StockAdjustmentRequest request);
    
    /// <summary>
    /// Provides inventory insights and recommendations
    /// </summary>
    Task<InventoryInsights> GetInventoryInsightsAsync(string userId);
}

public class StockAdjustmentRequest
{
    public string ItemId { get; set; } = string.Empty;
    public string WarehouseId { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class InventoryActionResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> ActionsPerformed { get; set; } = new();
    public decimal MoneySaved { get; set; }
    public int StockoutsPrevented { get; set; }
    public int ItemsReordered { get; set; }
}

public class InventoryInsights
{
    public List<LowStockAlert> LowStockItems { get; set; } = new();
    public List<ReorderRecommendation> ReorderRecommendations { get; set; } = new();
    public List<SlowMovingItem> SlowMovingItems { get; set; } = new();
    public decimal TotalInventoryValue { get; set; }
    public decimal TurnoverRate { get; set; }
    public List<string> Recommendations { get; set; } = new();
}

public class LowStockAlert
{
    public string ItemId { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public decimal ReorderLevel { get; set; }
    public string Urgency { get; set; } = string.Empty; // low, medium, high, critical
}

public class ReorderRecommendation
{
    public string ItemId { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal RecommendedQuantity { get; set; }
    public decimal EstimatedCost { get; set; }
    public string Supplier { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}

public class SlowMovingItem
{
    public string ItemId { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public decimal MonthlySales { get; set; }
    public int DaysOfStock { get; set; }
    public string Recommendation { get; set; } = string.Empty;
}

