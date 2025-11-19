namespace Toss.Domain.Entities.Catalog;

/// <summary>
/// Represents a low stock alert for a product at a shop
/// </summary>
public class StockAlert : BaseAuditableEntity
{
    public StockAlert()
    {
        CurrentStock = 0;
        MinimumStock = 0;
        IsAcknowledged = false;
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
    /// Gets or sets the current stock level when alert was created
    /// </summary>
    public int CurrentStock { get; set; }

    /// <summary>
    /// Gets or sets the minimum stock threshold
    /// </summary>
    public int MinimumStock { get; set; }

    /// <summary>
    /// Gets or sets whether the alert has been acknowledged
    /// </summary>
    public bool IsAcknowledged { get; set; }

    /// <summary>
    /// Gets or sets when the alert was acknowledged
    /// </summary>
    public DateTimeOffset? AcknowledgedAt { get; set; }

    /// <summary>
    /// Gets or sets who acknowledged the alert
    /// </summary>
    public string? AcknowledgedBy { get; set; }

    /// <summary>
    /// Alias for IsAcknowledged (used by handlers for compatibility)
    /// </summary>
    public bool IsResolved
    {
        get => IsAcknowledged;
        set => IsAcknowledged = value;
    }
}

