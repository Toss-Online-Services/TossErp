namespace Toss.Domain.Entities.Inventory;

public class StockLevel : BaseAuditableEntity
{
    public int ShopId { get; set; }
    public Shop Shop { get; set; } = null!;
    
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    public int CurrentStock { get; set; }
    public int ReservedStock { get; set; }
    public int AvailableStock => CurrentStock - ReservedStock;
    
    // Alias for CurrentStock (used by handlers)
    public int Quantity
    {
        get => CurrentStock;
        set => CurrentStock = value;
    }
    
    // Reorder settings
    public int ReorderPoint { get; set; }
    public int ReorderQuantity { get; set; }
    
    public decimal AverageCost { get; set; }
    public DateTimeOffset LastStockDate { get; set; }
}

