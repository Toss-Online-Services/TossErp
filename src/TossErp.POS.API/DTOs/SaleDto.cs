using System.ComponentModel.DataAnnotations;

namespace TossErp.POS.API.DTOs
{
    public class SaleDto
    {
        public Guid Id { get; set; }
        public string SaleNumber { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public Guid CashierId { get; set; }
        public string CashierName { get; set; } = string.Empty;
        public List<SaleItemDto> Items { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string SaleType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancellationReason { get; set; }
        public string? Notes { get; set; }
        public List<PaymentDto> Payments { get; set; } = new();
        public decimal AmountPaid { get; set; }
        public string? ReferenceNumber { get; set; }
        public Guid BusinessId { get; set; }
    }

    public class SaleItemDto
    {
        public Guid Id { get; set; }
        public Guid SaleId { get; set; }
        public Guid ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? Discount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountPercentage { get; set; }
        public string? Notes { get; set; }
    }

    public class PaymentDto
    {
        public Guid Id { get; set; }
        public Guid SaleId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string ReferenceNumber { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class CreateSaleDto
    {
        public Guid BusinessId { get; set; }
        public Guid? CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public Guid? CashierId { get; set; }
        public string? Notes { get; set; }
        public bool IsDraft { get; set; }
        public List<CreateSaleItemDto> Items { get; set; } = new();
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string SaleType { get; set; } = string.Empty;
    }

    public class CreateSaleItemDto
    {
        public Guid ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Discount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountPercentage { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateSaleItemDto
    {
        public Guid ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Discount { get; set; }
        public string? Notes { get; set; }
    }

    public class SaleListDto
    {
        public Guid Id { get; set; }
        public string SaleNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CashierName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }

    public class DailySummaryDto
    {
        public DateTime Date { get; set; }
        public int TotalSales { get; set; }
        public int TotalTransactions { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal CashSales { get; set; }
        public decimal CardSales { get; set; }
        public decimal MobileMoneySales { get; set; }
        public decimal OtherPaymentSales { get; set; }
        public decimal AverageTransactionValue { get; set; }
        public int TotalItems { get; set; }
        public int ItemsSold { get; set; }
        public decimal AverageOrderValue { get; set; }
        public decimal TotalDiscounts { get; set; }
        public decimal TotalTax { get; set; }
    }

    public enum PaymentMethod
    {
        Cash,
        Card,
        CreditCard,
        MobileMoney,
        MobilePayment,
        BankTransfer,
        Other
    }

    public enum SaleStatus
    {
        Pending,
        Completed,
        Cancelled,
        Refunded
    }

    public class RefundSaleDto
    {
        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        [Required]
        public List<RefundSaleItemDto> Items { get; set; } = new();
    }

    public class RefundSaleItemDto
    {
        [Required]
        public Guid SaleItemId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }

    public class CompleteSaleDto
    {
        [Required]
        public Guid SaleId { get; set; }
        [Required]
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal AmountPaid { get; set; }
        public string? ReferenceNumber { get; set; }
    }

    public class CancelSaleDto
    {
        public Guid SaleId { get; set; }
        public string CancellationReason { get; set; } = string.Empty;
    }

    public class SalesReportDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalTransactions { get; set; }
        public decimal AverageTransactionValue { get; set; }
        public List<SalesReportItemDto> TopSellingItems { get; set; } = new();
        public List<SalesReportItemDto> TopSellingCategories { get; set; } = new();
        public decimal CashSales { get; set; }
        public decimal CardSales { get; set; }
        public decimal MobileMoneySales { get; set; }
        public decimal OtherPaymentSales { get; set; }
        public decimal TotalSales { get; set; }
    }

    public class SalesReportItemDto
    {
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Total { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class SalePaymentDto
    {
        public Guid Id { get; set; }
        public Guid SaleId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
    }
} 
