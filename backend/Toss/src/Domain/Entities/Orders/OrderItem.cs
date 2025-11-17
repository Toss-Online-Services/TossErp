namespace Toss.Domain.Entities.Orders;

/// <summary>
/// Represents an order item
/// </summary>
public class OrderItem : BaseEntity
{
    public OrderItem()
    {
        OrderItemGuid = Guid.NewGuid();
        Quantity = 1;
        UnitPriceExclTax = 0;
        UnitPriceInclTax = 0;
        PriceExclTax = 0;
        PriceInclTax = 0;
    }

    /// <summary>
    /// Gets or sets the order item GUID
    /// </summary>
    public Guid OrderItemGuid { get; set; }

    /// <summary>
    /// Gets or sets the order ID
    /// </summary>
    public int OrderId { get; set; }
    public Order? Order { get; set; }

    /// <summary>
    /// Gets or sets the product ID
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the quantity
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price (excl tax)
    /// </summary>
    public decimal UnitPriceExclTax { get; set; }

    /// <summary>
    /// Gets or sets the unit price (incl tax)
    /// </summary>
    public decimal UnitPriceInclTax { get; set; }

    /// <summary>
    /// Gets or sets the price (excl tax)
    /// </summary>
    public decimal PriceExclTax { get; set; }

    /// <summary>
    /// Gets or sets the price (incl tax)
    /// </summary>
    public decimal PriceInclTax { get; set; }

    /// <summary>
    /// Gets or sets the discount amount (excl tax)
    /// </summary>
    public decimal DiscountAmountExclTax { get; set; }

    /// <summary>
    /// Gets or sets the discount amount (incl tax)
    /// </summary>
    public decimal DiscountAmountInclTax { get; set; }

    /// <summary>
    /// Gets or sets the download count
    /// </summary>
    public int DownloadCount { get; set; }
}

