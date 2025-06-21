namespace TossErp.POS.API.DTOs
{
    public class SalesReportDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalTransactions { get; set; }
        public decimal TotalSales { get; set; }
        public decimal AverageTransactionValue { get; set; }
        public List<SalesReportItemDto> TopSellingItems { get; set; } = new();
        public List<SalesReportItemDto> TopSellingCategories { get; set; } = new();
        public decimal CashSales { get; set; }
        public decimal CardSales { get; set; }
        public decimal MobileMoneySales { get; set; }
        public decimal OtherPaymentSales { get; set; }
    }

    public class SalesReportItemDto
    {
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Percentage { get; set; }
    }
} 
