using System.ComponentModel.DataAnnotations;

namespace TossErp.WebApp.DTOs
{
    public class StockAdjustmentDto
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public Guid ProductId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string AdjustmentType { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int PreviousStock { get; set; }
        public int NewStock { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalValue { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Reason { get; set; }
        public DateTime AdjustmentDate { get; set; }
        public Guid? UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Guid BusinessId { get; set; }
    }

    public class CreateStockAdjustmentDto
    {
        [Required]
        public Guid ItemId { get; set; }

        [Required]
        [StringLength(50)]
        public string AdjustmentType { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [StringLength(100)]
        public string? ReferenceNumber { get; set; }

        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        public Guid BusinessId { get; set; }
    }

    public class UpdateStockAdjustmentDto
    {
        [Required]
        [StringLength(50)]
        public string AdjustmentType { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [StringLength(100)]
        public string? ReferenceNumber { get; set; }

        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;
    }

    public enum StockAdjustmentType
    {
        Addition,
        Subtraction,
        Correction,
        Damage,
        Expiry,
        Transfer
    }
} 
