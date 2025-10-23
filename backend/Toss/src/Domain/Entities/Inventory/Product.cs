namespace Toss.Domain.Entities.Inventory;

public class Product : BaseAuditableEntity
{
    public string SKU { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
    public ProductCategory? Category { get; set; }
    
    // Pricing
    public decimal BasePrice { get; set; }
    public decimal? CostPrice { get; set; }
    public string Currency { get; set; } = "ZAR";
    
    // Inventory
    public string? Unit { get; set; } // e.g., "piece", "kg", "liter"
    public int MinimumStockLevel { get; set; } = 10;
    public int? ReorderQuantity { get; set; }
    
    // Status
    public bool IsActive { get; set; } = true;
    public bool IsTaxable { get; set; } = true;
    
    // Relationships
    public ICollection<StockLevel> StockLevels { get; private set; } = new List<StockLevel>();
    public ICollection<StockMovement> StockMovements { get; private set; } = new List<StockMovement>();
}

