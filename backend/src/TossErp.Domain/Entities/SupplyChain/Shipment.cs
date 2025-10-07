using TossErp.Domain.Common;
using TossErp.Domain.Events.SupplyChain;

namespace TossErp.Domain.Entities.SupplyChain;

/// <summary>
/// Shipment tracking for inbound and outbound logistics
/// </summary>
public class Shipment : BaseEntity
{
    public string ShipmentNumber { get; set; } = string.Empty;
    public ShipmentType Type { get; set; }
    public ShipmentStatus Status { get; set; } = ShipmentStatus.Draft;
    
    // Origin & Destination
    public int? OriginWarehouseId { get; set; }
    public string? OriginWarehouseName { get; set; }
    public string? OriginAddress { get; set; }
    public string? OriginContactName { get; set; }
    public string? OriginContactPhone { get; set; }
    
    public int? DestinationWarehouseId { get; set; }
    public string? DestinationWarehouseName { get; set; }
    public string? DestinationAddress { get; set; }
    public string? DestinationContactName { get; set; }
    public string? DestinationContactPhone { get; set; }
    
    // Carrier Information
    public int? CarrierId { get; set; }
    public string? CarrierName { get; set; }
    public string? TrackingNumber { get; set; }
    public string? VehicleNumber { get; set; }
    public string? DriverName { get; set; }
    public string? DriverPhone { get; set; }
    
    // Scheduling
    public DateTime? PickupDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ActualPickupDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    
    // Dimensions & Weight
    public decimal? TotalWeight { get; set; }
    public string? WeightUnit { get; set; } = "kg";
    public decimal? TotalVolume { get; set; }
    public string? VolumeUnit { get; set; } = "m3";
    public int? PackageCount { get; set; }
    
    // Costs
    public decimal ShippingCost { get; set; }
    public decimal? InsuranceCost { get; set; }
    public decimal? OtherCharges { get; set; }
    public decimal TotalCost { get; set; }
    
    // References
    public int? PurchaseOrderId { get; set; }
    public string? PurchaseOrderNumber { get; set; }
    public int? SalesOrderId { get; set; }
    public string? SalesOrderNumber { get; set; }
    public int? TransferOrderId { get; set; }
    public string? TransferOrderNumber { get; set; }
    
    // Metadata
    public string? Notes { get; set; }
    public string? SpecialInstructions { get; set; }
    public bool RequiresSignature { get; set; }
    public string? SignedBy { get; set; }
    public DateTime? SignedAt { get; set; }
    
    // Navigation Properties
    public ICollection<ShipmentItem> Items { get; set; } = new List<ShipmentItem>();
    public ICollection<ShipmentTracking> TrackingHistory { get; set; } = new List<ShipmentTracking>();
    
    // Business Methods
    public void Dispatch()
    {
        if (Status != ShipmentStatus.Ready)
            throw new InvalidOperationException("Shipment must be ready for dispatch");
        
        Status = ShipmentStatus.InTransit;
        ActualPickupDate = DateTime.UtcNow;
        AddDomainEvent(new ShipmentDispatched(Id, ShipmentNumber));
    }
    
    public void Deliver()
    {
        if (Status != ShipmentStatus.InTransit)
            throw new InvalidOperationException("Only in-transit shipments can be delivered");
        
        Status = ShipmentStatus.Delivered;
        ActualDeliveryDate = DateTime.UtcNow;
        AddDomainEvent(new ShipmentDelivered(Id, ShipmentNumber));
    }
    
    public void Cancel(string reason)
    {
        if (Status == ShipmentStatus.Delivered)
            throw new InvalidOperationException("Cannot cancel delivered shipment");
        
        Status = ShipmentStatus.Cancelled;
        Notes = $"Cancelled: {reason}\n{Notes}";
        AddDomainEvent(new ShipmentCancelled(Id, ShipmentNumber, reason));
    }
}

public enum ShipmentType
{
    Inbound,        // Receiving from supplier
    Outbound,       // Shipping to customer
    Transfer,       // Between warehouses
    Return          // Return shipment
}

public enum ShipmentStatus
{
    Draft,
    Ready,          // Ready for pickup
    InTransit,
    Delivered,
    Cancelled,
    Delayed
}
