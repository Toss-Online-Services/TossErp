namespace TossErp.Shared.DTOs;

public class SalesReportDto
{
    public decimal TotalSales { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageOrderValue { get; set; }
    public string TopSellingProduct { get; set; } = string.Empty;
    public string TopProduct { get; set; } = string.Empty;
    public List<PaymentMethodDto> PaymentMethods { get; set; } = new();
    public List<DailySalesDto> SalesData { get; set; } = new();
    public List<SalesReportItemDto> Items { get; set; } = new();
}

public class PaymentMethodDto
{
    public string Method { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public double Percentage { get; set; }
}

public class DailySalesDto
{
    public DateTime Date { get; set; }
    public int Orders { get; set; }
    public decimal Revenue { get; set; }
    public decimal AverageOrder { get; set; }
} 
