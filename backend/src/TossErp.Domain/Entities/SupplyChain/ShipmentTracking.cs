using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.SupplyChain;

/// <summary>
/// Tracking history for shipments
/// </summary>
public class ShipmentTracking : BaseEntity
{
    public int ShipmentId { get; set; }
    public DateTime EventDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? LocationCity { get; set; }
    public string? LocationCountry { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public string? RecordedBy { get; set; }
    
    // Navigation Properties
    public Shipment Shipment { get; set; } = null!;
}
