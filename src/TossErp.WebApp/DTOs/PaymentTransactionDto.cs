using System.ComponentModel.DataAnnotations;

namespace TossErp.WebApp.DTOs
{
    public class PaymentTransactionDto
    {
        public Guid Id { get; set; }
        public Guid SaleId { get; set; }
        public string SaleNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string ReferenceNumber { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public Guid BusinessId { get; set; }
    }

    public class CreatePaymentTransactionDto
    {
        [Required]
        public Guid SaleId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        [StringLength(100)]
        public string ReferenceNumber { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Notes { get; set; }

        public Guid BusinessId { get; set; }
    }

    public class UpdatePaymentTransactionDto
    {
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        [StringLength(100)]
        public string ReferenceNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Notes { get; set; }
    }
} 
