namespace TossErp.Accounting.Application.Common.DTOs;

/// <summary>
/// Profit & Loss report data
/// </summary>
public class ProfitLossReportDto
{
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
    public string Currency { get; init; } = "ZAR";
    
    // Revenue
    public decimal TotalRevenue { get; init; }
    public Dictionary<string, decimal> RevenueByCategory { get; init; } = new();
    
    // Cost of Goods Sold
    public decimal TotalCostOfGoodsSold { get; init; }
    public Dictionary<string, decimal> CostOfGoodsSoldByCategory { get; init; } = new();
    
    // Gross Profit
    public decimal GrossProfit { get; init; }
    public decimal GrossProfitMargin { get; init; }
    
    // Operating Expenses
    public decimal TotalOperatingExpenses { get; init; }
    public Dictionary<string, decimal> OperatingExpensesByCategory { get; init; } = new();
    
    // Net Profit
    public decimal NetProfit { get; init; }
    public decimal NetProfitMargin { get; init; }
    
    // Stock Valuation
    public decimal OpeningStockValue { get; init; }
    public decimal ClosingStockValue { get; init; }
    public decimal StockValuationChange { get; init; }
    
    // Summary
    public int TotalTransactions { get; init; }
    public DateTime GeneratedAt { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Cash position report data
/// </summary>
public class CashPositionReportDto
{
    public DateTime AsOfDate { get; init; }
    public string Currency { get; init; } = "ZAR";
    
    // Current Position
    public decimal CurrentCashBalance { get; init; }
    public decimal AvailableCash { get; init; }
    public decimal CommittedCash { get; init; }
    
    // Cash Flow Summary
    public decimal CashInflow { get; init; }
    public decimal CashOutflow { get; init; }
    public decimal NetCashFlow { get; init; }
    
    // Breakdown by Category
    public Dictionary<string, decimal> CashInflowByCategory { get; init; } = new();
    public Dictionary<string, decimal> CashOutflowByCategory { get; init; } = new();
    
    // Liquidity Metrics
    public decimal DaysOfCashOnHand { get; init; }
    public decimal CashFlowRatio { get; init; }
    
    // Recent Activity
    public List<CashTransactionDto> RecentTransactions { get; init; } = new();
    public int TotalTransactions { get; init; }
    public DateTime GeneratedAt { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Month-over-month comparison report data
/// </summary>
public class MonthOverMonthReportDto
{
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
    public string Currency { get; init; } = "ZAR";
    
    // Monthly Data
    public List<MonthlyDataDto> MonthlyData { get; init; } = new();
    
    // Growth Metrics
    public decimal RevenueGrowthRate { get; init; }
    public decimal ExpenseGrowthRate { get; init; }
    public decimal ProfitGrowthRate { get; init; }
    
    // Trends
    public Dictionary<string, List<MonthlyTrendDto>> CategoryTrends { get; init; } = new();
    
    // Summary
    public int TotalMonths { get; init; }
    public DateTime GeneratedAt { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Cash transaction data for reports
/// </summary>
public class CashTransactionDto
{
    public Guid Id { get; init; }
    public DateTime TransactionDate { get; init; }
    public decimal Amount { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Reference { get; init; }
}

/// <summary>
/// Monthly data for comparison reports
/// </summary>
public class MonthlyDataDto
{
    public DateTime Month { get; init; }
    public decimal Revenue { get; init; }
    public decimal Expenses { get; init; }
    public decimal Profit { get; init; }
    public decimal CashFlow { get; init; }
    public int TransactionCount { get; init; }
    public Dictionary<string, decimal> CategoryBreakdown { get; init; } = new();
}

/// <summary>
/// Monthly trend data
/// </summary>
public class MonthlyTrendDto
{
    public DateTime Month { get; init; }
    public decimal Value { get; init; }
    public decimal ChangeFromPrevious { get; init; }
    public decimal PercentageChange { get; init; }
}


