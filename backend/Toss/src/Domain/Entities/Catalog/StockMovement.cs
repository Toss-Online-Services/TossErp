namespace Toss.Domain.Entities.Catalog;

/// <summary>
/// Represents a stock movement transaction (stock in/out)
/// </summary>
public class StockMovement : BaseAuditableEntity
{
    public StockMovement()
    {
        QuantityBefore = 0;
        QuantityChange = 0;
        QuantityAfter = 0;
        MovementDate = DateTimeOffset.UtcNow;
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
    /// Gets or sets the type of stock movement
    /// </summary>
    public StockMovementType MovementType { get; set; }

    /// <summary>
    /// Gets or sets the quantity before the movement
    /// </summary>
    public int QuantityBefore { get; set; }

    /// <summary>
    /// Gets or sets the quantity change (positive for increase, negative for decrease)
    /// </summary>
    public int QuantityChange { get; set; }

    /// <summary>
    /// Gets or sets the quantity after the movement
    /// </summary>
    public int QuantityAfter { get; set; }

    /// <summary>
    /// Alias for QuantityChange (used by handlers for compatibility)
    /// </summary>
    public int Quantity
    {
        get => QuantityChange;
        set => QuantityChange = value;
    }

    /// <summary>
    /// Gets or sets the reference type (e.g., "Sale", "Purchase", "Adjustment")
    /// </summary>
    public string? ReferenceType { get; set; }

    /// <summary>
    /// Gets or sets the reference ID (ID of the related sale, purchase, etc.)
    /// </summary>
    public int? ReferenceId { get; set; }

    /// <summary>
    /// Gets or sets optional notes about the movement
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the date of the movement
    /// </summary>
    public DateTimeOffset MovementDate { get; set; }
}

