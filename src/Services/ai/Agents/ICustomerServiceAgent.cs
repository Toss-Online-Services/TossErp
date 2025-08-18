namespace TossErp.AI.Agents;

/// <summary>
/// Autonomous agent for customer service and engagement
/// </summary>
public interface ICustomerServiceAgent
{
    /// <summary>
    /// Handles customer inquiries and support requests
    /// </summary>
    Task<CustomerServiceActionResult> HandleCustomerInquiryAsync(CustomerInquiryRequest request);
    
    /// <summary>
    /// Manages customer relationships and follow-ups
    /// </summary>
    Task<CustomerServiceActionResult> ManageCustomerRelationshipsAsync(string userId);
    
    /// <summary>
    /// Sends proactive customer communications
    /// </summary>
    Task<CustomerServiceActionResult> SendProactiveCommunicationsAsync(string userId);
    
    /// <summary>
    /// Provides customer service insights and analytics
    /// </summary>
    Task<CustomerServiceInsights> GetCustomerServiceInsightsAsync(string userId);
}

public class CustomerInquiryRequest
{
    public string CustomerId { get; set; } = string.Empty;
    public string Inquiry { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty; // whatsapp, email, phone, chat
    public string Priority { get; set; } = "normal"; // low, normal, high, urgent
    public string? Category { get; set; } // billing, product, support, general
}

public class CustomerServiceActionResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Response { get; set; } = string.Empty;
    public bool Resolved { get; set; }
    public List<string> ActionsTaken { get; set; } = new();
    public TimeSpan ResolutionTime { get; set; }
    public bool RequiresFollowUp { get; set; }
}

public class CustomerServiceInsights
{
    public int TotalInquiries { get; set; }
    public int ResolvedInquiries { get; set; }
    public decimal ResolutionRate { get; set; }
    public TimeSpan AverageResolutionTime { get; set; }
    public List<InquiryCategory> InquiryBreakdown { get; set; } = new();
    public List<CustomerSatisfaction> SatisfactionMetrics { get; set; } = new();
    public List<CustomerServiceAlert> Alerts { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}

public class InquiryCategory
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
    public TimeSpan AverageResolutionTime { get; set; }
}

public class CustomerSatisfaction
{
    public string Metric { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public string Trend { get; set; } = string.Empty; // improving, declining, stable
    public int Responses { get; set; }
}

public class CustomerServiceAlert
{
    public string Type { get; set; } = string.Empty; // high_volume, long_resolution, low_satisfaction
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty; // low, medium, high, critical
    public DateTime DetectedAt { get; set; }
    public string Recommendation { get; set; } = string.Empty;
}

