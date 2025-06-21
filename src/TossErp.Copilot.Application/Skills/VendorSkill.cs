using Microsoft.SemanticKernel;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace TossErp.Copilot.Application.Skills;

/// <summary>
/// Vendor management skills for the AI Copilot
/// </summary>
public class VendorSkill
{
    private readonly ILogger<VendorSkill> _logger;

    public VendorSkill(ILogger<VendorSkill> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Find the cheapest vendor for a specific product
    /// </summary>
    [KernelFunction]
    [Description("Find the cheapest vendor for a specific product")]
    public Task<string> FindCheapestVendor(
        [Description("Product name or SKU")] string productName,
        [Description("Quantity needed")] int quantity = 1)
    {
        _logger.LogInformation("Finding cheapest vendor for {ProductName}, quantity: {Quantity}", productName, quantity);
        var response = productName.ToLower() switch
        {
            var p when p.Contains("bread") => GenerateBreadVendorComparison(quantity),
            var p when p.Contains("milk") => GenerateMilkVendorComparison(quantity),
            var p when p.Contains("egg") => GenerateEggVendorComparison(quantity),
            var p when p.Contains("sugar") => GenerateSugarVendorComparison(quantity),
            var p when p.Contains("flour") => GenerateFlourVendorComparison(quantity),
            _ => $"🔍 Vendor Search Results: {productName}\n\nNo specific vendor data available for '{productName}'.\nPlease check the vendor management system for current pricing."
        };
        return Task.FromResult(response);
    }

    /// <summary>
    /// Compare vendor prices for multiple products
    /// </summary>
    [KernelFunction]
    [Description("Compare vendor prices for multiple products")]
    public Task<string> CompareVendorPrices(
        [Description("Comma-separated list of products")] string products)
    {
        _logger.LogInformation("Comparing vendor prices for products: {Products}", products);
        var productList = products.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList();
        var comparison = "🔍 VENDOR PRICE COMPARISON\n\n";
        foreach (var product in productList)
        {
            comparison += $"📦 {product.ToUpper()}:\n";
            comparison += GetProductVendorComparison(product);
            comparison += "\n";
        }
        comparison += "💡 RECOMMENDATIONS:\n• Consider bulk ordering for better prices\n• Factor in delivery costs and times\n• Check payment terms and credit options\n• Review vendor reliability ratings";
        return Task.FromResult(comparison);
    }

    /// <summary>
    /// Get vendor performance analysis
    /// </summary>
    [KernelFunction]
    [Description("Analyze vendor performance and reliability")]
    public Task<string> AnalyzeVendorPerformance(
        [Description("Vendor name or 'all' for all vendors")] string vendorName = "all")
    {
        _logger.LogInformation("Analyzing vendor performance for: {VendorName}", vendorName);
        var analysis = vendorName.ToLower() switch
        {
            "all" => GenerateAllVendorsAnalysis(),
            var v when v.Contains("fresh") => GenerateVendorAnalysis("Fresh Foods Ltd", 4.2, 95, 2.5),
            var v when v.Contains("quality") => GenerateVendorAnalysis("Quality Suppliers", 4.5, 98, 1.8),
            var v when v.Contains("metro") => GenerateVendorAnalysis("Metro Wholesale", 3.8, 92, 3.2),
            var v when v.Contains("local") => GenerateVendorAnalysis("Local Market Co", 4.0, 96, 2.0),
            _ => $"📊 Vendor Performance: {vendorName}\n\nNo specific performance data available for '{vendorName}'.\nPlease check the vendor management system for current ratings."
        };
        return Task.FromResult(analysis);
    }

    private string GenerateBreadVendorComparison(int quantity)
    {
        return $"🍞 BREAD VENDOR COMPARISON (Qty: {quantity})\n\n" +
               $"🥇 BEST PRICE: Fresh Foods Ltd\n" +
               $"   • Price: R 8.50 per unit\n" +
               $"   • Total: R {8.50 * quantity:F2}\n" +
               $"   • Delivery: 2-3 days\n" +
               $"   • Rating: ⭐⭐⭐⭐⭐ (4.2/5)\n\n" +
               
               $"🥈 Quality Suppliers\n" +
               $"   • Price: R 9.00 per unit\n" +
               $"   • Total: R {9.00 * quantity:F2}\n" +
               $"   • Delivery: 1-2 days\n" +
               $"   • Rating: ⭐⭐⭐⭐⭐ (4.5/5)\n\n" +
               
               $"🥉 Metro Wholesale\n" +
               $"   • Price: R 8.80 per unit\n" +
               $"   • Total: R {8.80 * quantity:F2}\n" +
               $"   • Delivery: 3-4 days\n" +
               $"   • Rating: ⭐⭐⭐⭐ (3.8/5)\n\n" +
               
               $"💡 RECOMMENDATION: Fresh Foods Ltd\n" +
               $"   • Best price for quality\n" +
               $"   • Reliable delivery\n" +
               $"   • Good payment terms";
    }

    private string GenerateMilkVendorComparison(int quantity)
    {
        return $"🥛 MILK VENDOR COMPARISON (Qty: {quantity})\n\n" +
               $"🥇 BEST PRICE: Quality Suppliers\n" +
               $"   • Price: R 12.50 per unit\n" +
               $"   • Total: R {12.50 * quantity:F2}\n" +
               $"   • Delivery: Same day\n" +
               $"   • Rating: ⭐⭐⭐⭐⭐ (4.5/5)\n\n" +
               
               $"🥈 Local Market Co\n" +
               $"   • Price: R 13.00 per unit\n" +
               $"   • Total: R {13.00 * quantity:F2}\n" +
               $"   • Delivery: 1 day\n" +
               $"   • Rating: ⭐⭐⭐⭐ (4.0/5)\n\n" +
               
               $"🥉 Fresh Foods Ltd\n" +
               $"   • Price: R 13.20 per unit\n" +
               $"   • Total: R {13.20 * quantity:F2}\n" +
               $"   • Delivery: 2 days\n" +
               $"   • Rating: ⭐⭐⭐⭐⭐ (4.2/5)\n\n" +
               
               $"💡 RECOMMENDATION: Quality Suppliers\n" +
               $"   • Best price with same-day delivery\n" +
               $"   • Highest reliability rating\n" +
               $"   • Fresh product guarantee";
    }

    private string GenerateEggVendorComparison(int quantity)
    {
        return $"🥚 EGGS VENDOR COMPARISON (Qty: {quantity})\n\n" +
               $"🥇 BEST PRICE: Local Market Co\n" +
               $"   • Price: R 15.00 per unit\n" +
               $"   • Total: R {15.00 * quantity:F2}\n" +
               $"   • Delivery: 1 day\n" +
               $"   • Rating: ⭐⭐⭐⭐ (4.0/5)\n\n" +
               
               $"🥈 Fresh Foods Ltd\n" +
               $"   • Price: R 15.50 per unit\n" +
               $"   • Total: R {15.50 * quantity:F2}\n" +
               $"   • Delivery: 2 days\n" +
               $"   • Rating: ⭐⭐⭐⭐⭐ (4.2/5)\n\n" +
               
               $"🥉 Metro Wholesale\n" +
               $"   • Price: R 16.00 per unit\n" +
               $"   • Total: R {16.00 * quantity:F2}\n" +
               $"   • Delivery: 3 days\n" +
               $"   • Rating: ⭐⭐⭐⭐ (3.8/5)\n\n" +
               
               $"💡 RECOMMENDATION: Local Market Co\n" +
               $"   • Best price with quick delivery\n" +
               $"   • Local supplier advantage\n" +
               $"   • Good quality rating";
    }

    private string GenerateSugarVendorComparison(int quantity)
    {
        return $"🍯 SUGAR VENDOR COMPARISON (Qty: {quantity})\n\n" +
               $"🥇 BEST PRICE: Metro Wholesale\n" +
               $"   • Price: R 18.00 per unit\n" +
               $"   • Total: R {18.00 * quantity:F2}\n" +
               $"   • Delivery: 3-4 days\n" +
               $"   • Rating: ⭐⭐⭐⭐ (3.8/5)\n\n" +
               
               $"🥈 Quality Suppliers\n" +
               $"   • Price: R 18.50 per unit\n" +
               $"   • Total: R {18.50 * quantity:F2}\n" +
               $"   • Delivery: 2 days\n" +
               $"   • Rating: ⭐⭐⭐⭐⭐ (4.5/5)\n\n" +
               
               $"🥉 Fresh Foods Ltd\n" +
               $"   • Price: R 19.00 per unit\n" +
               $"   • Total: R {19.00 * quantity:F2}\n" +
               $"   • Delivery: 2-3 days\n" +
               $"   • Rating: ⭐⭐⭐⭐⭐ (4.2/5)\n\n" +
               
               $"💡 RECOMMENDATION: Metro Wholesale\n" +
               $"   • Best price for bulk orders\n" +
               $"   • Good for non-urgent needs\n" +
               $"   • Consider delivery time";
    }

    private string GenerateFlourVendorComparison(int quantity)
    {
        return $"🌾 FLOUR VENDOR COMPARISON (Qty: {quantity})\n\n" +
               $"🥇 BEST PRICE: Metro Wholesale\n" +
               $"   • Price: R 22.00 per unit\n" +
               $"   • Total: R {22.00 * quantity:F2}\n" +
               $"   • Delivery: 3-4 days\n" +
               $"   • Rating: ⭐⭐⭐⭐ (3.8/5)\n\n" +
               
               $"🥈 Quality Suppliers\n" +
               $"   • Price: R 22.50 per unit\n" +
               $"   • Total: R {22.50 * quantity:F2}\n" +
               $"   • Delivery: 2 days\n" +
               $"   • Rating: ⭐⭐⭐⭐⭐ (4.5/5)\n\n" +
               
               $"🥉 Local Market Co\n" +
               $"   • Price: R 23.00 per unit\n" +
               $"   • Total: R {23.00 * quantity:F2}\n" +
               $"   • Delivery: 1 day\n" +
               $"   • Rating: ⭐⭐⭐⭐ (4.0/5)\n\n" +
               
               $"💡 RECOMMENDATION: Metro Wholesale\n" +
               $"   • Best price for bulk orders\n" +
               $"   • Good for storage items\n" +
               $"   • Plan ahead for delivery";
    }

    private string GetProductVendorComparison(string product)
    {
        return product.ToLower() switch
        {
            var p when p.Contains("bread") => "• Fresh Foods Ltd: R 8.50 (Best)\n• Quality Suppliers: R 9.00\n• Metro Wholesale: R 8.80",
            var p when p.Contains("milk") => "• Quality Suppliers: R 12.50 (Best)\n• Local Market Co: R 13.00\n• Fresh Foods Ltd: R 13.20",
            var p when p.Contains("egg") => "• Local Market Co: R 15.00 (Best)\n• Fresh Foods Ltd: R 15.50\n• Metro Wholesale: R 16.00",
            var p when p.Contains("sugar") => "• Metro Wholesale: R 18.00 (Best)\n• Quality Suppliers: R 18.50\n• Fresh Foods Ltd: R 19.00",
            var p when p.Contains("flour") => "• Metro Wholesale: R 22.00 (Best)\n• Quality Suppliers: R 22.50\n• Local Market Co: R 23.00",
            _ => "• No vendor data available"
        };
    }

    private string GenerateAllVendorsAnalysis()
    {
        return "📊 ALL VENDORS PERFORMANCE ANALYSIS\n\n" +
               "🥇 TOP PERFORMER: Quality Suppliers\n" +
               "   • Rating: 4.5/5 ⭐⭐⭐⭐⭐\n" +
               "   • On-time Delivery: 98%\n" +
               "   • Average Delivery: 1.8 days\n" +
               "   • Price Competitiveness: 92%\n" +
               "   • Customer Service: Excellent\n\n" +
               
               "🥈 Fresh Foods Ltd\n" +
               "   • Rating: 4.2/5 ⭐⭐⭐⭐⭐\n" +
               "   • On-time Delivery: 95%\n" +
               "   • Average Delivery: 2.5 days\n" +
               "   • Price Competitiveness: 88%\n" +
               "   • Customer Service: Good\n\n" +
               
               "🥉 Local Market Co\n" +
               "   • Rating: 4.0/5 ⭐⭐⭐⭐\n" +
               "   • On-time Delivery: 96%\n" +
               "   • Average Delivery: 2.0 days\n" +
               "   • Price Competitiveness: 85%\n" +
               "   • Customer Service: Very Good\n\n" +
               
               "📉 Metro Wholesale\n" +
               "   • Rating: 3.8/5 ⭐⭐⭐⭐\n" +
               "   • On-time Delivery: 92%\n" +
               "   • Average Delivery: 3.2 days\n" +
               "   • Price Competitiveness: 95%\n" +
               "   • Customer Service: Fair\n\n" +
               
               "💡 RECOMMENDATIONS:\n" +
               "• Use Quality Suppliers for urgent, high-quality needs\n" +
               "• Use Metro Wholesale for bulk, non-urgent orders\n" +
               "• Fresh Foods Ltd for balanced price/quality\n" +
               "• Local Market Co for local, fresh products";
    }

    private string GenerateVendorAnalysis(string vendorName, double rating, int deliveryRate, double avgDelivery)
    {
        return $"📊 VENDOR ANALYSIS: {vendorName}\n\n" +
               $"⭐ Overall Rating: {rating}/5\n" +
               $"📦 On-time Delivery: {deliveryRate}%\n" +
               $"⏱️ Average Delivery: {avgDelivery} days\n\n" +
               
               $"📈 PERFORMANCE METRICS:\n" +
               $"• Price Competitiveness: {(rating * 20):F0}%\n" +
               $"• Quality Consistency: {(rating * 18):F0}%\n" +
               $"• Communication: {(rating * 19):F0}%\n" +
               $"• Payment Terms: {(rating * 17):F0}%\n\n" +
               
               $"💡 STRENGTHS:\n" +
               $"• Reliable delivery schedule\n" +
               $"• Good product quality\n" +
               $"• Competitive pricing\n" +
               $"• Professional service\n\n" +
               
               $"⚠️ AREAS FOR IMPROVEMENT:\n" +
               $"• Faster delivery for urgent orders\n" +
               $"• Better communication on delays\n" +
               $"• More flexible payment options";
    }
} 
