namespace TossErp.AI.Agents;

/// <summary>
/// Autonomous agent for sales and customer service
/// </summary>
public interface ISalesAgent
{
    /// <summary>
    /// Automatically processes sales and generates invoices
    /// </summary>
    Task<SalesActionResult> ProcessSaleAsync(SaleRequest request);
    
    /// <summary>
    /// Manages customer relationships and follow-ups
    /// </summary>
    Task<SalesActionResult> ManageCustomerRelationshipsAsync(string userId);
    
    /// <summary>
    /// Generates sales reports and insights
    /// </summary>
    Task<SalesInsights> GetSalesInsightsAsync(string userId);
    
    /// <summary>
    /// Handles customer inquiries automatically
    /// </summary>
    Task<CustomerServiceResponse> HandleCustomerInquiryAsync(CustomerInquiryRequest request);
}

public class SaleRequest
{
    public string CustomerId { get; set; } = string.Empty;
    public List<SaleItem> Items { get; set; } = new();
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal Discount { get; set; }
    public string? Notes { get; set; }
}

public class SaleItem
{
    public string ItemId { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class SalesActionResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string InvoiceId { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public List<string> ActionsPerformed { get; set; } = new();
    public decimal RevenueGenerated { get; set; }
}

public class SalesInsights
{
    public decimal TotalSales { get; set; }
    public int TotalTransactions { get; set; }
    public decimal AverageTransactionValue { get; set; }
    public List<TopSellingItem> TopSellingItems { get; set; } = new();
    public List<CustomerInsight> CustomerInsights { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}

public class TopSellingItem
{
    public string ItemId { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal QuantitySold { get; set; }
    public decimal Revenue { get; set; }
    public decimal ProfitMargin { get; set; }
}

public class CustomerInsight
{
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalSpent { get; set; }
    public int VisitCount { get; set; }
    public DateTime LastVisit { get; set; }
    public string LoyaltyLevel { get; set; } = string.Empty;
}

public class CustomerInquiryRequest
{
    public string CustomerId { get; set; } = string.Empty;
    public string Inquiry { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty; // whatsapp, email, phone
}

public class CustomerServiceResponse
{
    public string Response { get; set; } = string.Empty;
    public bool Resolved { get; set; }
    public List<string> ActionsTaken { get; set; } = new();
    public bool RequiresFollowUp { get; set; }
}

