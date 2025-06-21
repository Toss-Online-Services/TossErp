using TossErp.Inventory.Domain.Enums;

namespace TossErp.Inventory.API.DTOs
{
    public class ItemDto
    {
        public Guid Id { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Barcode { get; set; }
        public string? SKU { get; set; }
        public ItemType ItemType { get; set; }
        public bool IsStockable { get; set; }
        public bool IsSerialized { get; set; }
        public bool IsBatched { get; set; }
        public decimal StandardCost { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public string? UnitOfMeasure { get; set; }
        public int CurrentStock { get; set; }
        public decimal? MinimumStockLevel { get; set; }
        public decimal? MaximumStockLevel { get; set; }
        public decimal? ReorderPoint { get; set; }
        public decimal? ReorderQuantity { get; set; }
        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public Guid? BrandId { get; set; }
        public string? BrandName { get; set; }
        public Guid? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public List<ItemVariantDto> Variants { get; set; } = new();
        public List<ItemPriceHistoryDto> PriceHistory { get; set; } = new();
    }
} 
