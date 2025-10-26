namespace Toss.Domain.Entities.Catalog;

/// <summary>
/// Represents the current stock level of a product at a specific shop
/// </summary>
public class StockLevel : BaseAuditableEntity
{
    public StockLevel()
    {
        CurrentStock = 0;
        ReservedStock = 0;
        ReorderPoint = 0;
        ReorderQuantity = 0;
        AverageCost = 0;
        LastStockDate = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Gets or sets the shop ID
    /// </summary>
    public int ShopId { get; set; }
    public Store Shop { get; set; } = null!;

    /// <summary>
    /// Gets or sets the product ID
    /// </summary>
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>
    /// Gets or sets the current physical stock quantity
    /// </summary>
    public int CurrentStock { get; set; }

    /// <summary>
    /// Gets or sets the reserved stock (allocated but not yet sold)
    /// </summary>
    public int ReservedStock { get; set; }

    /// <summary>
    /// Gets the available stock (current minus reserved)
    /// </summary>
    public int AvailableStock => CurrentStock - ReservedStock;

    /// <summary>
    /// Alias for CurrentStock (used by handlers for compatibility)
    /// </summary>
    public int Quantity
    {
        get => CurrentStock;
        set => CurrentStock = value;
    }

    /// <summary>
    /// Gets or sets the reorder point threshold
    /// </summary>
    public int ReorderPoint { get; set; }

    /// <summary>
    /// Gets or sets the quantity to reorder when below reorder point
    /// </summary>
    public int ReorderQuantity { get; set; }

    /// <summary>
    /// Gets or sets the average cost of stock
    /// </summary>
    public decimal AverageCost { get; set; }

    /// <summary>
    /// Gets or sets the last stock update date
    /// </summary>
    public DateTimeOffset LastStockDate { get; set; }
}

