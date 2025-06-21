using TossErp.Domain.AggregatesModel.ProductAggregate;
using TossErp.Domain.AggregatesModel.SaleAggregate;
using TossErp.Domain.AggregatesModel.GroupPurchaseAggregate;

namespace TossErp.Application.Services;

public class CopilotService : ICopilotService
{
    private readonly IProductRepository _productRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly IGroupPurchaseRepository _groupPurchaseRepository;

    public CopilotService(
        IProductRepository productRepository,
        ISaleRepository saleRepository,
        IGroupPurchaseRepository groupPurchaseRepository)
    {
        _productRepository = productRepository;
        _saleRepository = saleRepository;
        _groupPurchaseRepository = groupPurchaseRepository;
    }

    public async Task<string> ProcessQueryAsync(string query, Guid businessId, Guid? userId = null)
    {
        var lowerQuery = query.ToLower();
        
        // Sales-related queries
        if (lowerQuery.Contains("sales") || lowerQuery.Contains("sold") || lowerQuery.Contains("revenue"))
        {
            return await GetSalesInsightsAsync(businessId, DateTime.Today.AddDays(-7), DateTime.Today);
        }
        
        // Stock-related queries
        if (lowerQuery.Contains("stock") || lowerQuery.Contains("inventory") || lowerQuery.Contains("restock"))
        {
            return await GetInventoryAlertsAsync(businessId);
        }
        
        // Group purchase queries
        if (lowerQuery.Contains("group") || lowerQuery.Contains("bulk") || lowerQuery.Contains("nearby"))
        {
            return await GetGroupPurchaseSuggestionsAsync(businessId);
        }
        
        // Promotion queries
        if (lowerQuery.Contains("promotion") || lowerQuery.Contains("discount") || lowerQuery.Contains("offer"))
        {
            return await GetPromotionSuggestionsAsync(businessId);
        }
        
        // General business queries
        if (lowerQuery.Contains("how") || lowerQuery.Contains("what") || lowerQuery.Contains("recommend"))
        {
            return await GetBusinessRecommendationsAsync(businessId);
        }
        
        // Default response
        return "I'm here to help! You can ask me about sales, inventory, group purchases, or business recommendations. What would you like to know?";
    }

    public async Task<string> GetSalesInsightsAsync(Guid businessId, DateTime fromDate, DateTime toDate)
    {
        try
        {
            var sales = await _saleRepository.GetByDateRangeAsync(businessId, fromDate, toDate);
            var totalSales = sales.Sum(s => s.TotalAmount);
            var salesCount = sales.Count();
            
            if (salesCount == 0)
            {
                return "No sales recorded for this period. Consider running a promotion to boost sales!";
            }
            
            var topProducts = sales
                .SelectMany(s => s.Items)
                .GroupBy(item => item.ProductName)
                .OrderByDescending(g => g.Sum(item => item.TotalPrice))
                .Take(3)
                .Select(g => $"{g.Key} (R{g.Sum(item => item.TotalPrice):F2})");
            
            var response = $"Sales Summary ({fromDate:MMM dd} - {toDate:MMM dd}):\n" +
                          $"• Total Sales: R{totalSales:F2}\n" +
                          $"• Number of Transactions: {salesCount}\n" +
                          $"• Average Transaction: R{totalSales / salesCount:F2}\n" +
                          $"• Top Products: {string.Join(", ", topProducts)}";
            
            return response;
        }
        catch
        {
            return "I'm having trouble accessing sales data right now. Please try again later.";
        }
    }

    public async Task<string> GetInventoryAlertsAsync(Guid businessId)
    {
        try
        {
            var lowStockProducts = await _productRepository.GetLowStockProductsAsync(businessId);
            
            if (!lowStockProducts.Any())
            {
                return "Great news! All your products have sufficient stock levels.";
            }
            
            var lowStockList = lowStockProducts
                .Take(5)
                .Select(p => $"• {p.Name} - {p.StockQuantity} {p.Unit} remaining");
            
            var response = "⚠️ Low Stock Alert:\n" +
                          $"{string.Join("\n", lowStockList)}\n\n" +
                          "Consider reordering these items soon to avoid stockouts!";
            
            return response;
        }
        catch
        {
            return "I'm having trouble checking inventory levels right now. Please try again later.";
        }
    }

    public async Task<string> GetGroupPurchaseSuggestionsAsync(Guid businessId)
    {
        try
        {
            var activeGroups = await _groupPurchaseRepository.GetActiveGroupPurchasesAsync(businessId);
            
            if (!activeGroups.Any())
            {
                return "No active group purchases at the moment. You can start one to save money on bulk orders!";
            }
            
            var suggestions = activeGroups
                .Take(3)
                .Select(gp => $"• {gp.ProductName} - {gp.GetRemainingQuantity()} units needed, save R{gp.GetTotalSavings():F2}");
            
            var response = "🔥 Active Group Purchases:\n" +
                          $"{string.Join("\n", suggestions)}\n\n" +
                          "Join these groups to save money on bulk purchases!";
            
            return response;
        }
        catch
        {
            return "I'm having trouble checking group purchases right now. Please try again later.";
        }
    }

    public async Task<string> GetPromotionSuggestionsAsync(Guid businessId)
    {
        try
        {
            var lowStockProducts = await _productRepository.GetLowStockProductsAsync(businessId);
            var slowMovingProducts = await _productRepository.GetByBusinessIdAsync(businessId);
            
            var suggestions = new List<string>();
            
            if (lowStockProducts.Any())
            {
                suggestions.Add("• Run a 'Last Few Items' promotion on low stock products");
            }
            
            if (slowMovingProducts.Any())
            {
                suggestions.Add("• Consider a 'Buy 2 Get 1 Free' offer on slow-moving items");
            }
            
            suggestions.Add("• Weekend special: 10% off on all items");
            suggestions.Add("• Loyalty program: 5% cashback for repeat customers");
            
            var response = "💡 Promotion Ideas:\n" +
                          $"{string.Join("\n", suggestions)}\n\n" +
                          "These promotions can help boost your sales and clear inventory!";
            
            return response;
        }
        catch
        {
            return "I'm having trouble generating promotion suggestions right now. Please try again later.";
        }
    }

    public Task<string> GetBusinessRecommendationsAsync(Guid businessId)
    {
        try
        {
            var recommendations = new List<string>
            {
                "• Check your daily sales report to identify trends",
                "• Review low stock items and plan reorders",
                "• Consider joining or creating group purchases for better prices",
                "• Send promotional messages to your customer list",
                "• Update your product prices based on market trends",
                "• Plan for upcoming seasonal demand"
            };
            
            var response = "📊 Business Recommendations:\n" +
                          $"{string.Join("\n", recommendations)}\n\n" +
                          "Focus on these areas to grow your business!";
            
            return Task.FromResult(response);
        }
        catch
        {
            return Task.FromResult("I'm having trouble generating recommendations right now. Please try again later.");
        }
    }
} 
