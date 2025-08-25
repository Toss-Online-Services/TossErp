namespace Crm.Application.DTOs;

public class CustomerAnalyticsDto
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalSpent { get; set; }
    public int PurchaseCount { get; set; }
    public decimal AverageOrderValue { get; set; }
    public int LoyaltyPoints { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Segment { get; set; } = string.Empty;
    public DateTime? FirstPurchaseDate { get; set; }
    public DateTime? LastPurchaseDate { get; set; }
    public int? DaysSinceLastPurchase { get; set; }
    public bool IsLapsed { get; set; }
    public bool IsHighValue { get; set; }
    public List<MonthlyPurchaseData> MonthlyPurchaseTrend { get; set; } = new();
    public List<DayOfWeekData> PreferredPurchaseDays { get; set; } = new();
    public List<SeasonalData> SeasonalTrends { get; set; } = new();
    public decimal RiskScore { get; set; }
    public List<string> Recommendations { get; set; } = new();
}

public class MonthlyPurchaseData
{
    public string Month { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int TransactionCount { get; set; }
}

public class DayOfWeekData
{
    public string DayOfWeek { get; set; } = string.Empty;
    public int PurchaseCount { get; set; }
    public decimal TotalAmount { get; set; }
}

public class SeasonalData
{
    public string Season { get; set; } = string.Empty;
    public decimal AverageAmount { get; set; }
    public int TransactionCount { get; set; }
}

public class CustomerSegmentDto
{
    public string Segment { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal AverageSpent { get; set; }
    public decimal Percentage { get; set; }
}

public class SalesTrendsDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string GroupBy { get; set; } = string.Empty;
    public decimal TotalSales { get; set; }
    public int TotalTransactions { get; set; }
    public decimal AverageOrderValue { get; set; }
    public List<SalesTrendDataPoint> TrendData { get; set; } = new();
    public decimal GrowthRate { get; set; }
    public List<SeasonalityData> Seasonality { get; set; } = new();
}

public class SalesTrendDataPoint
{
    public string Period { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int TransactionCount { get; set; }
    public decimal AverageOrderValue { get; set; }
    public int NewCustomers { get; set; }
    public int ReturningCustomers { get; set; }
}

public class SeasonalityData
{
    public string Period { get; set; } = string.Empty;
    public decimal AverageAmount { get; set; }
    public decimal PerformanceIndex { get; set; }
}

// Supporting data classes for analytics
public class CustomerAnalyticsData
{
    public DateTime? FirstPurchaseDate { get; set; }
    public List<MonthlyPurchaseData> MonthlyPurchaseTrend { get; set; } = new();
    public List<DayOfWeekData> PreferredPurchaseDays { get; set; } = new();
    public List<SeasonalData> SeasonalTrends { get; set; } = new();
}

public class CustomerSegmentData
{
    public CustomerSegment Segment { get; set; }
    public int Count { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal Percentage { get; set; }
}

public class SalesTrendData
{
    public string Period { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int TransactionCount { get; set; }
    public int NewCustomers { get; set; }
    public int ReturningCustomers { get; set; }
}
