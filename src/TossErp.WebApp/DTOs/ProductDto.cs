using System.ComponentModel.DataAnnotations;

namespace TossErp.WebApp.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public Guid? SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public decimal Price { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int CurrentStock { get; set; }
        public int StockQuantity { get; set; }
        public int MinimumStockLevel { get; set; }
        public int MaximumStockLevel { get; set; }
        public string UnitOfMeasure { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Weight { get; set; }
        public string? Dimensions { get; set; }
        public string? Notes { get; set; }
        public Guid BusinessId { get; set; }
        public Guid? VendorId { get; set; }
    }

    public class CreateProductDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string SKU { get; set; } = string.Empty;

        [StringLength(100)]
        public string Barcode { get; set; } = string.Empty;

        [Required]
        public Guid CategoryId { get; set; }

        public Guid? SupplierId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        public decimal Price { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal CostPrice { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal SellingPrice { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int CurrentStock { get; set; }

        public int StockQuantity { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int MinimumStockLevel { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int MaximumStockLevel { get; set; }

        [Required]
        [StringLength(20)]
        public string UnitOfMeasure { get; set; } = string.Empty;

        public string Unit { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Weight { get; set; }

        [StringLength(100)]
        public string? Dimensions { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public Guid BusinessId { get; set; }
        public Guid VendorId { get; set; }
    }

    public class UpdateProductDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string SKU { get; set; } = string.Empty;

        [StringLength(100)]
        public string Barcode { get; set; } = string.Empty;

        [Required]
        public Guid CategoryId { get; set; }

        public Guid? SupplierId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        public decimal Price { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal CostPrice { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal SellingPrice { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int CurrentStock { get; set; }

        public int StockQuantity { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int MinimumStockLevel { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int MaximumStockLevel { get; set; }

        [Required]
        [StringLength(20)]
        public string UnitOfMeasure { get; set; } = string.Empty;

        public string Unit { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public string? ImageUrl { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Weight { get; set; }

        [StringLength(100)]
        public string? Dimensions { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public Guid BusinessId { get; set; }
        public Guid VendorId { get; set; }
    }
} 
