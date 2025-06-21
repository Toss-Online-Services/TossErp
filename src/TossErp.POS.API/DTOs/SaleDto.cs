using System.ComponentModel.DataAnnotations;
using TossErp.Shared.Enums;

namespace TossErp.POS.API.DTOs
{
    public class CreateSaleDto
    {
        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public Guid CashierId { get; set; }

        [Required]
        public SaleType SaleType { get; set; }

        public List<CreateSaleItemDto> Items { get; set; } = new();

        public PaymentMethod? PaymentMethod { get; set; }

        public decimal? AmountPaid { get; set; }

        [StringLength(100)]
        public string? ReferenceNumber { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
    }

    public class CreateSaleItemDto
    {
        [Required]
        public Guid ItemId { get; set; }

        [StringLength(200)]
        public string? ItemName { get; set; }

        [Required]
        public decimal Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        public decimal? DiscountAmount { get; set; }

        public decimal? DiscountPercentage { get; set; }
    }

    public class UpdateSaleDto
    {
        public List<CreateSaleItemDto> Items { get; set; } = new();

        [StringLength(1000)]
        public string? Notes { get; set; }
    }

    public class CompleteSaleDto
    {
        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        public decimal AmountPaid { get; set; }

        [StringLength(100)]
        public string? ReferenceNumber { get; set; }
    }

    public class CancelSaleDto
    {
        [Required]
        [StringLength(500)]
        public string CancellationReason { get; set; } = string.Empty;
    }

    public class SaleDto
    {
        public Guid Id { get; set; }
        public string SaleNumber { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public Guid CashierId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CashierName { get; set; } = string.Empty;
        public DateTime SaleDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal ChangeAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public SaleStatus Status { get; set; }
        public SaleType SaleType { get; set; }
        public string? TransactionReference { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Notes { get; set; }
        public string? CancellationReason { get; set; }
        public List<SaleItemDto> Items { get; set; } = new();
        public List<SalePaymentDto> Payments { get; set; } = new();
    }

    public class SaleItemDto
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class SalePaymentDto
    {
        public Guid Id { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public string? ReferenceNumber { get; set; }
        public DateTime PaymentDate { get; set; }
    }

    public class SaleListDto
    {
        public List<SaleDto> Sales { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public decimal TotalSalesAmount { get; set; }
    }
} 
