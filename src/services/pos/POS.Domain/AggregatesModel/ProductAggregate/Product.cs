#nullable enable
using TossErp.POS.Domain.Common;

namespace TossErp.POS.Domain.AggregatesModel.ProductAggregate;

public class Product : Entity, IAggregateRoot
{
    public string StoreId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string? Sku { get; set; }
    public decimal Price { get; set; }
    public decimal Cost { get; set; }
    public int StockQuantity { get; set; }
    public int LowStockThreshold { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string Code { get; set; } = string.Empty;
    public int StockLevel { get; set; }

    public void UpdateStock(int quantity)
    {
        StockQuantity = quantity;
        StockLevel = StockQuantity;
        UpdatedAt = DateTime.UtcNow;
    }
} 
