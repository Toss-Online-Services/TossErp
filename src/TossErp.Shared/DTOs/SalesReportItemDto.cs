namespace TossErp.Shared.DTOs;

public class SalesReportItemDto
{
    public string PeriodOrProduct { get; set; } = string.Empty;
    public decimal SalesAmount { get; set; }
    public int Orders { get; set; }
    public int QuantitySold { get; set; }
} 
