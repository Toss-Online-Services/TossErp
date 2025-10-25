namespace Toss.Domain.Entities.Shipping;

/// <summary>
/// Represents a shipment
/// </summary>
public class Shipment : BaseAuditableEntity
{
    public Shipment()
    {
        TrackingNumber = string.Empty;
        ShipmentItems = new List<ShipmentItem>();
    }

    /// <summary>
    /// Gets or sets the order ID
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Gets or sets the tracking number
    /// </summary>
    public string TrackingNumber { get; set; }

    /// <summary>
    /// Gets or sets the total weight
    /// </summary>
    public decimal? TotalWeight { get; set; }

    /// <summary>
    /// Gets or sets the shipped date and time
    /// </summary>
    public DateTime? ShippedDateUtc { get; set; }

    /// <summary>
    /// Gets or sets the delivery date and time
    /// </summary>
    public DateTime? DeliveryDateUtc { get; set; }

    /// <summary>
    /// Gets or sets the admin comment
    /// </summary>
    public string? AdminComment { get; set; }

    /// <summary>
    /// Gets or sets the shipment items
    /// </summary>
    public ICollection<ShipmentItem> ShipmentItems { get; set; }
}

