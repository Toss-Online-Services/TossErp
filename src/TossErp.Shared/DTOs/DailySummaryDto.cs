namespace TossErp.Shared.DTOs
{
    public class DailySummaryDto
    {
        public DateTime Date { get; set; }
        public int TotalSales { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalCustomers { get; set; }
    }
} 
