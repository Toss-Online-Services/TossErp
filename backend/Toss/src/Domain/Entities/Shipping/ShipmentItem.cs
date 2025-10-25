namespace Toss.Domain.Entities.Shipping;

/// <summary>
/// Represents a shipment item
/// </summary>
public class ShipmentItem : BaseEntity
{
    public ShipmentItem()
    {
        Quantity = 1;
    }

    /// <summary>
    /// Gets or sets the shipment ID
    /// </summary>
    public int ShipmentId { get; set; }
    public Shipment? Shipment { get; set; }

    /// <summary>
    /// Gets or sets the order item ID
    /// </summary>
    public int OrderItemId { get; set; }

    /// <summary>
    /// Gets or sets the quantity
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the warehouse ID
    /// </summary>
    public int? WarehouseId { get; set; }
}

