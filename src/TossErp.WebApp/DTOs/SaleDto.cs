using System.ComponentModel.DataAnnotations;

namespace TossErp.WebApp.DTOs
{
    public class SaleDto
    {
        public Guid Id { get; set; }
        public string SaleNumber { get; set; } = string.Empty;
        public Guid CustomerId { get; set; } // UI expects this
        public string CustomerName { get; set; } = string.Empty;
        public Guid CashierId { get; set; }
        public string CashierName { get; set; } = string.Empty;
        public List<SaleItemDto> Items { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal SubTotal { get; set; } // UI expects this
        public decimal TaxAmount { get; set; }
        public decimal Tax { get; set; } // UI expects this
        public decimal DiscountAmount { get; set; }
        public decimal Discount { get; set; } // UI expects this
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string SaleStatus { get; set; } = string.Empty; // UI expects this
        public DateTime CreatedAt { get; set; }
        public DateTime SaleDate { get; set; } // UI expects this
        public DateTime? CompletedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancellationReason { get; set; }
        public string? Notes { get; set; }
        public List<PaymentTransactionDto> Payments { get; set; } = new(); // UI expects this
        public decimal AmountPaid { get; set; }
        public string? ReferenceNumber { get; set; }
        public Guid BusinessId { get; set; }
        public bool IsRefunded { get; set; } // UI expects this
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
} 
