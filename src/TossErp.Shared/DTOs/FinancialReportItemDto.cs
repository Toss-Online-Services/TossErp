namespace TossErp.Shared.DTOs;

public class FinancialReportItemDto
{
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public double Percentage { get; set; }
    public string Type { get; set; } = string.Empty;
} 
