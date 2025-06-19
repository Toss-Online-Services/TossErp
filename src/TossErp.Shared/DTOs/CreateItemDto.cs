using TossErp.Shared.DTOs.Enums;

namespace TossErp.Shared.DTOs
{
    public class CreateItemDto
    {
        public string ItemCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Barcode { get; set; }
        public string? SKU { get; set; }
        public ItemType ItemType { get; set; }
        public bool IsStockable { get; set; }
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
    }
} 
