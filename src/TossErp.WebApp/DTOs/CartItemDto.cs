using System.ComponentModel.DataAnnotations;

namespace TossErp.WebApp.DTOs
{
    public class CartItemDto
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal? Discount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountPercentage { get; set; }
        public string? Notes { get; set; }
        public DateTime AddedAt { get; set; }
        public Guid BusinessId { get; set; }
        
        // UI expected properties
        public ProductDto Product { get; set; } = new(); // UI expects this
        public decimal Total { get; set; } // UI expects this
    }

    public class AddToCartDto
    {
        [Required]
        public Guid ItemId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Discount { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public Guid BusinessId { get; set; }
    }

    public class UpdateCartItemDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Discount { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class CartDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public List<CartItemDto> Items { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public Guid BusinessId { get; set; }
    }
} 
