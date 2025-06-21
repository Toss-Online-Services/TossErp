namespace TossErp.Inventory.API.DTOs
{
    public class ItemVariantDto
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string VariantName { get; set; } = string.Empty;
        public string? VariantCode { get; set; }
        public string? SKU { get; set; }
        public string? Barcode { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 
