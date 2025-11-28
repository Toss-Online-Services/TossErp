namespace Toss.Domain.Entities.Orders;

/// <summary>
/// Represents a line item in a purchase order
/// </summary>
public class PurchaseOrderItem : BaseEntity
{
    public PurchaseOrderItem()
    {
        ProductName = string.Empty;
        QuantityOrdered = 0;
        QuantityReceived = 0;
        UnitPrice = 0;
        TaxAmount = 0;
        LineTotal = 0;
    }

    /// <summary>
    /// Gets or sets the purchase order ID
    /// </summary>
    public int PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    /// <summary>
    /// Gets or sets the product ID
    /// </summary>
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>
    /// Gets or sets the product name (snapshot)
    /// </summary>
    public string ProductName { get; set; }

    /// <summary>
    /// Gets or sets the product SKU (snapshot)
    /// </summary>
    public string? ProductSKU { get; set; }

    /// <summary>
    /// Gets or sets the quantity ordered
    /// </summary>
    public int QuantityOrdered { get; set; }

    /// <summary>
    /// Gets or sets the quantity received
    /// </summary>
    public int QuantityReceived { get; set; }

    /// <summary>
    /// Gets or sets the quantity that has been invoiced
    /// </summary>
    public int QuantityInvoiced { get; set; }

    /// <summary>
    /// Alias for QuantityOrdered (used by handlers)
    /// </summary>
    public int Quantity
    {
        get => QuantityOrdered;
        set => QuantityOrdered = value;
    }

    /// <summary>
    /// Gets or sets the unit price
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the tax amount
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// Gets or sets the line total
    /// </summary>
    public decimal LineTotal { get; set; }
}

