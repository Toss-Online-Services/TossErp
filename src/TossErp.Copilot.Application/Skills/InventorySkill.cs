using Microsoft.SemanticKernel;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace TossErp.Copilot.Application.Skills;

/// <summary>
/// Inventory management skills for the AI Copilot
/// </summary>
public class InventorySkill
{
    private readonly ILogger<InventorySkill> _logger;

    public InventorySkill(ILogger<InventorySkill> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Check current stock levels for items
    /// </summary>
    [KernelFunction]
    [Description("Check current stock levels for specified items or all items")]
    public Task<string> CheckStock(
        [Description("Item name or SKU to check, or 'all' for all items")] string itemQuery = "all",
        [Description("Minimum stock level threshold")] decimal? threshold = null)
    {
        _logger.LogInformation("Checking stock for: {ItemQuery}", itemQuery);
        var response = itemQuery.ToLower() switch
        {
            "all" => "Checking all inventory items...\n• Bread: 45 units (Good)\n• Milk: 12 units (Low stock)\n• Eggs: 8 units (Reorder needed)\n• Sugar: 25 units (Good)\n• Flour: 15 units (Good)",
            var item when item.Contains("bread") => "Bread stock: 45 units\nStatus: Good\nLast restocked: 2 days ago",
            var item when item.Contains("milk") => "Milk stock: 12 units\nStatus: Low stock\nRecommendation: Reorder soon",
            var item when item.Contains("egg") => "Eggs stock: 8 units\nStatus: Reorder needed\nUrgent: Order within 24 hours",
            var item when item.Contains("sugar") => "Sugar stock: 25 units\nStatus: Good\nNext reorder: 1 week",
            var item when item.Contains("flour") => "Flour stock: 15 units\nStatus: Good\nNext reorder: 3 days",
            _ => $"Stock check for '{itemQuery}':\nNo specific data available. Please check the inventory system."
        };
        return Task.FromResult(response);
    }

    /// <summary>
    /// Generate reorder recommendations based on stock levels
    /// </summary>
    [KernelFunction]
    [Description("Generate reorder recommendations for items that need restocking")]
    public Task<string> GenerateReorderRecommendations(
        [Description("Include items below this stock level")] decimal? minStockLevel = 10)
    {
        _logger.LogInformation("Generating reorder recommendations with min stock level: {MinStockLevel}", minStockLevel);
        var recommendations = new List<string>
        {
            "📋 REORDER RECOMMENDATIONS",
            "",
            "🚨 URGENT (Order within 24 hours):",
            "• Eggs: 8 units remaining (Order 50 units)",
            "• Milk: 12 units remaining (Order 30 units)",
            "",
            "⚠️ SOON (Order within 3 days):",
            "• Flour: 15 units remaining (Order 25 units)",
            "• Cooking oil: 18 units remaining (Order 20 units)",
            "",
            "📊 Current low stock items: 5",
            "Estimated reorder value: R 2,450",
            "",
            "💡 Tips:",
            "• Consider bulk ordering for better prices",
            "• Check supplier delivery schedules",
            "• Review seasonal demand patterns"
        };
        return Task.FromResult(string.Join("\n", recommendations));
    }

    /// <summary>
    /// Get stock movement history for analysis
    /// </summary>
    [KernelFunction]
    [Description("Get stock movement history for trend analysis")]
    public Task<string> GetStockMovementHistory(
        [Description("Item name or SKU")] string itemName,
        [Description("Number of days to look back")] int days = 30)
    {
        _logger.LogInformation("Getting stock movement history for {ItemName} over {Days} days", itemName, days);
        var response = $"📈 Stock Movement History: {itemName}\nPeriod: Last {days} days\n\n📊 Summary:\n• Total In: 150 units\n• Total Out: 120 units\n• Net Change: +30 units\n• Average Daily Sales: 4 units\n\n📅 Recent Activity:\n• Today: -3 units (Sales)\n• Yesterday: +25 units (Restock)\n• 2 days ago: -5 units (Sales)\n• 3 days ago: -2 units (Sales)\n\n💡 Insights:\n• Peak sales: Weekends\n• Reorder cycle: Every 2 weeks\n• Seasonal trend: Stable";
        return Task.FromResult(response);
    }
} 
