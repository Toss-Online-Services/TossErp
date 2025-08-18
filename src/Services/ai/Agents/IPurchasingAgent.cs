namespace TossErp.AI.Agents;

/// <summary>
/// Autonomous agent for purchasing and procurement services
/// </summary>
public interface IPurchasingAgent
{
    /// <summary>
    /// Automatically places purchase orders based on inventory needs
    /// </summary>
    Task<PurchaseActionResult> PlacePurchaseOrderAsync(PurchaseOrderRequest request);
    
    /// <summary>
    /// Manages supplier relationships and evaluations
    /// </summary>
    Task<PurchaseActionResult> ManageSuppliersAsync(string userId);
    
    /// <summary>
    /// Analyzes purchasing patterns and optimizes costs
    /// </summary>
    Task<PurchaseInsights> GetPurchaseInsightsAsync(string userId);
    
    /// <summary>
    /// Handles group purchasing opportunities
    /// </summary>
    Task<GroupPurchaseResult> FacilitateGroupPurchaseAsync(GroupPurchaseRequest request);
}

public class PurchaseOrderRequest
{
    public string SupplierId { get; set; } = string.Empty;
    public List<PurchaseItem> Items { get; set; } = new();
    public DateTime ExpectedDelivery { get; set; }
    public string? Notes { get; set; }
    public bool IsUrgent { get; set; }
}

public class PurchaseItem
{
    public string ItemId { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? SupplierItemCode { get; set; }
}

public class PurchaseActionResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string PurchaseOrderId { get; set; } = string.Empty;
    public decimal TotalCost { get; set; }
    public List<string> ActionsPerformed { get; set; } = new();
    public decimal MoneySaved { get; set; }
}

public class PurchaseInsights
{
    public decimal TotalPurchases { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageOrderValue { get; set; }
    public List<SupplierInsight> SupplierInsights { get; set; } = new();
    public List<CostOptimization> CostOptimizations { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}

public class SupplierInsight
{
    public string SupplierId { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public decimal TotalSpent { get; set; }
    public int OrderCount { get; set; }
    public decimal AverageDeliveryTime { get; set; }
    public string ReliabilityRating { get; set; } = string.Empty;
}

public class CostOptimization
{
    public string ItemId { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal CurrentCost { get; set; }
    public decimal OptimizedCost { get; set; }
    public decimal PotentialSavings { get; set; }
    public string Recommendation { get; set; } = string.Empty;
}

public class GroupPurchaseRequest
{
    public string ItemId { get; set; } = string.Empty;
    public decimal TotalQuantity { get; set; }
    public List<string> ParticipantIds { get; set; } = new();
    public DateTime Deadline { get; set; }
}

public class GroupPurchaseResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string GroupPurchaseId { get; set; } = string.Empty;
    public decimal TotalSavings { get; set; }
    public int Participants { get; set; }
    public List<string> ActionsPerformed { get; set; } = new();
}

