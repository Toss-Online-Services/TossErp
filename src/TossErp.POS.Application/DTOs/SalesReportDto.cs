namespace TossErp.POS.Application.DTOs;

public class SalesReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalSales { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalTax { get; set; }
    public decimal TotalDiscount { get; set; }
    public int ItemsSold { get; set; }
    public List<DailySummaryDto> DailySummaries { get; set; } = new();
    public Dictionary<string, decimal> RevenueByPaymentMethod { get; set; } = new();
    public Dictionary<string, int> SalesByStatus { get; set; } = new();
} 
