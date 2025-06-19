using System.ComponentModel.DataAnnotations;
using TossErp.Inventory.Domain.Enums;

namespace TossErp.Inventory.API.DTOs
{
    public class CreateItemDto
    {
        [Required]
        [StringLength(50)]
        public string ItemCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? Barcode { get; set; }

        [StringLength(100)]
        public string? SKU { get; set; }

        [Required]
        public ItemType ItemType { get; set; }

        public bool IsStockable { get; set; } = true;

        public decimal StandardCost { get; set; }

        public decimal SellingPrice { get; set; }

        [StringLength(20)]
        public string? UnitOfMeasure { get; set; }

        public decimal? MinimumStockLevel { get; set; }

        public decimal? MaximumStockLevel { get; set; }

        public decimal? ReorderPoint { get; set; }

        public decimal? ReorderQuantity { get; set; }

        public Guid? CategoryId { get; set; }

        public Guid? BrandId { get; set; }

        public Guid? SupplierId { get; set; }
    }

    public class UpdateItemDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? Barcode { get; set; }

        [StringLength(100)]
        public string? SKU { get; set; }

        public ItemType ItemType { get; set; }

        public bool IsStockable { get; set; } = true;

        public decimal StandardCost { get; set; }

        public decimal SellingPrice { get; set; }

        [StringLength(20)]
        public string? UnitOfMeasure { get; set; }

        public decimal? MinimumStockLevel { get; set; }

        public decimal? MaximumStockLevel { get; set; }

        public decimal? ReorderPoint { get; set; }

        public decimal? ReorderQuantity { get; set; }

        public Guid? CategoryId { get; set; }

        public Guid? BrandId { get; set; }

        public Guid? SupplierId { get; set; }
    }

    public class ItemDto
    {
        public Guid Id { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Barcode { get; set; }
        public string? SKU { get; set; }
        public ItemType ItemType { get; set; }
        public bool IsActive { get; set; }
        public bool IsStockable { get; set; }
        public bool IsSerialized { get; set; }
        public bool IsBatched { get; set; }
        public decimal StandardCost { get; set; }
        public decimal SellingPrice { get; set; }
        public string? UnitOfMeasure { get; set; }
        public decimal? MinimumStockLevel { get; set; }
        public decimal? MaximumStockLevel { get; set; }
        public decimal? ReorderPoint { get; set; }
        public decimal? ReorderQuantity { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? BrandId { get; set; }
        public Guid? SupplierId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public List<ItemVariantDto> Variants { get; set; } = new();
        public List<ItemPriceHistoryDto> PriceHistory { get; set; } = new();
        public decimal CurrentPrice { get; set; }
    }

    public class ItemVariantDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? VariantCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ItemPriceHistoryDto
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string? Reason { get; set; }
    }

    public class ItemListDto
    {
        public List<ItemDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
} 
