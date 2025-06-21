namespace TossErp.POS.API.DTOs
{
    public class DailySummaryDto
    {
        public DateTime Date { get; set; }
        public int TotalTransactions { get; set; }
        public decimal TotalSales { get; set; }
        public decimal CashSales { get; set; }
        public decimal CardSales { get; set; }
        public decimal MobileMoneySales { get; set; }
        public decimal OtherPaymentSales { get; set; }
        public decimal AverageTransactionValue { get; set; }
        public int ItemsSold { get; set; }
        public decimal TotalDiscounts { get; set; }
        public decimal TotalTax { get; set; }
    }
} 
