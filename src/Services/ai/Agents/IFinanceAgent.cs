namespace TossErp.AI.Agents;

/// <summary>
/// Autonomous agent for financial management and reporting
/// </summary>
public interface IFinanceAgent
{
    /// <summary>
    /// Automatically generates financial reports and insights
    /// </summary>
    Task<FinanceActionResult> GenerateFinancialReportAsync(FinancialReportRequest request);
    
    /// <summary>
    /// Manages cash flow and financial health monitoring
    /// </summary>
    Task<FinanceActionResult> MonitorCashFlowAsync(string userId);
    
    /// <summary>
    /// Handles invoice processing and payment tracking
    /// </summary>
    Task<FinanceActionResult> ProcessInvoicesAsync(string userId);
    
    /// <summary>
    /// Provides financial insights and recommendations
    /// </summary>
    Task<FinancialInsights> GetFinancialInsightsAsync(string userId);
}

public class FinancialReportRequest
{
    public string ReportType { get; set; } = string.Empty; // income_statement, balance_sheet, cash_flow
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Format { get; set; } = "pdf"; // pdf, excel, json
    public bool IncludeCharts { get; set; } = true;
}

public class FinanceActionResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ReportId { get; set; } = string.Empty;
    public decimal ValueGenerated { get; set; }
    public List<string> ActionsPerformed { get; set; } = new();
    public TimeSpan TimeSaved { get; set; }
}

public class FinancialInsights
{
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetProfit { get; set; }
    public decimal ProfitMargin { get; set; }
    public List<RevenueTrend> RevenueTrends { get; set; } = new();
    public List<ExpenseCategory> ExpenseBreakdown { get; set; } = new();
    public List<FinancialAlert> Alerts { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}

public class RevenueTrend
{
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
    public decimal GrowthRate { get; set; }
    public string Trend { get; set; } = string.Empty; // increasing, decreasing, stable
}

public class ExpenseCategory
{
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
    public string Trend { get; set; } = string.Empty;
}

public class FinancialAlert
{
    public string Type { get; set; } = string.Empty; // low_cash, high_expenses, overdue_invoices
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty; // low, medium, high, critical
    public DateTime DetectedAt { get; set; }
    public string Recommendation { get; set; } = string.Empty;
}

