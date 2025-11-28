using Toss.Domain.Entities.Catalog;

namespace Toss.Domain.Entities.Orders;

/// <summary>
/// Represents a line item in a purchase request
/// </summary>
public class PurchaseRequestLine : BaseEntity
{
    public PurchaseRequestLine()
    {
        QuantityRequested = 0;
        Remarks = string.Empty;
    }

    /// <summary>
    /// Gets or sets the purchase request ID
    /// </summary>
    public int PurchaseRequestId { get; set; }
    public PurchaseRequest PurchaseRequest { get; set; } = null!;

    /// <summary>
    /// Gets or sets the product/item ID
    /// </summary>
    public int ItemId { get; set; }
    public Product Item { get; set; } = null!;

    /// <summary>
    /// Gets or sets the quantity requested
    /// </summary>
    public decimal QuantityRequested { get; set; }

    /// <summary>
    /// Gets or sets optional remarks for this line item
    /// </summary>
    public string? Remarks { get; set; }
}

