using TossErp.Shared.Enums;

namespace TossErp.Shared.DTOs
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
        public decimal StandardCost { get; set; }
        public decimal SellingPrice { get; set; }
        public string UnitOfMeasure { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int MinimumStockLevel { get; set; }
        public int MaximumStockLevel { get; set; }
        public int ReorderPoint { get; set; }
        public int ReorderQuantity { get; set; }
        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public Guid? BrandId { get; set; }
        public string? BrandName { get; set; }
        public Guid? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
    }
} 
