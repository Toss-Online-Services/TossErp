namespace TossErp.Shared.DTOs;

public class FinancialReportDto
{
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetProfit { get; set; }
    public double ProfitMargin { get; set; }
    public decimal OperatingCashFlow { get; set; }
    public decimal InvestingCashFlow { get; set; }
    public decimal FinancingCashFlow { get; set; }
    public decimal NetCashFlow { get; set; }
    public List<RevenueBreakdownDto> RevenueBreakdown { get; set; } = new();
    public List<ExpenseBreakdownDto> ExpenseBreakdown { get; set; } = new();
    public List<FinancialReportItemDto> Items { get; set; } = new();
}

public class RevenueBreakdownDto
{
    public string Category { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public double Percentage { get; set; }
    public double Growth { get; set; }
}

public class ExpenseBreakdownDto
{
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public double Percentage { get; set; }
    public decimal BudgetVariance { get; set; }
} 
