using System.ComponentModel.DataAnnotations;

namespace TossErp.WebApp.DTOs
{
    public class ItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;
        public decimal StandardCost { get; set; }
        public decimal SellingPrice { get; set; }
        public int StockQuantity { get; set; }
        public int StockLevel { get; set; }
        public int MinimumStockLevel { get; set; }
        public int MaximumStockLevel { get; set; }
        public int ReorderPoint { get; set; }
        public int ReorderQuantity { get; set; }
        public string Warehouse { get; set; } = string.Empty;
        public string UnitOfMeasure { get; set; } = string.Empty;
        public Guid BusinessId { get; set; }
        public Guid? VendorId { get; set; }
        public bool IsActive { get; set; }
        public bool IsStockable { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public Guid? SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public decimal CostPrice { get; set; }
        public decimal Weight { get; set; }
        public string? ImageUrl { get; set; }
        public string? Dimensions { get; set; }
        public string? Notes { get; set; }
        
        // UI alias properties
        public string Category => CategoryName;
        public decimal Price => UnitPrice;
        public int CurrentStock => StockQuantity;
        public string Unit => UnitOfMeasure;
        public DateTime? UpdatedAt => LastModifiedAt;
    }

    public class CreateItemDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;
        public decimal StandardCost { get; set; }
        public decimal SellingPrice { get; set; }
        public int StockQuantity { get; set; }
        public int StockLevel { get; set; }
        public int MinimumStockLevel { get; set; }
        public int MaximumStockLevel { get; set; }
        public int ReorderPoint { get; set; }
        public int ReorderQuantity { get; set; }
        public string Warehouse { get; set; } = string.Empty;
        public string UnitOfMeasure { get; set; } = string.Empty;
        public Guid BusinessId { get; set; }
        public Guid? VendorId { get; set; }
        public bool IsActive { get; set; }
        public bool IsStockable { get; set; }
        public Guid CategoryId { get; set; }
        public Guid? SupplierId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal CostPrice { get; set; }
        public decimal Weight { get; set; }
        public string? ImageUrl { get; set; }
        public string? Dimensions { get; set; }
        public string? Notes { get; set; }
        
        // UI alias properties
        public string Unit => UnitOfMeasure;
        public decimal Price => UnitPrice;
        public int CurrentStock => StockQuantity;
    }

    public class CreateSaleDto
    {
        public Guid BusinessId { get; set; }
        public Guid? CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public Guid? CashierId { get; set; }
        public string? Notes { get; set; }
        public bool IsDraft { get; set; }
        public List<SaleItemDto> Items { get; set; } = new();
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }

    public class SaleItemDto
    {
        public Guid Id { get; set; }
        public Guid SaleId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Discount { get; set; }
        public decimal Total { get; set; }
    }

    public class UpdateItemDto
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

        public decimal Price => UnitPrice;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal CostPrice { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal SellingPrice { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int CurrentStock { get; set; }

        public int StockQuantity => CurrentStock;

        [Required]
        [Range(0, int.MaxValue)]
        public int MinimumStockLevel { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int MaximumStockLevel { get; set; }

        [Required]
        [StringLength(20)]
        public string UnitOfMeasure { get; set; } = string.Empty;

        public string Unit => UnitOfMeasure;

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
