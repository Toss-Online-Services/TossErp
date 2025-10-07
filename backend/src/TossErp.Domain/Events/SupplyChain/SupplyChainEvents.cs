using TossErp.Domain.Common;

namespace TossErp.Domain.Events.SupplyChain;

// Shipment Events
public class ShipmentDispatched : DomainEvent
{
    public int ShipmentId { get; }
    public string ShipmentNumber { get; }
    
    public ShipmentDispatched(int shipmentId, string shipmentNumber)
    {
        ShipmentId = shipmentId;
        ShipmentNumber = shipmentNumber;
    }
}

public class ShipmentDelivered : DomainEvent
{
    public int ShipmentId { get; }
    public string ShipmentNumber { get; }
    
    public ShipmentDelivered(int shipmentId, string shipmentNumber)
    {
        ShipmentId = shipmentId;
        ShipmentNumber = shipmentNumber;
    }
}

public class ShipmentCancelled : DomainEvent
{
    public int ShipmentId { get; }
    public string ShipmentNumber { get; }
    public string Reason { get; }
    
    public ShipmentCancelled(int shipmentId, string shipmentNumber, string reason)
    {
        ShipmentId = shipmentId;
        ShipmentNumber = shipmentNumber;
        Reason = reason;
    }
}

public class ShipmentDelayed : DomainEvent
{
    public int ShipmentId { get; }
    public string ShipmentNumber { get; }
    public string Reason { get; }
    public DateTime NewExpectedDelivery { get; }
    
    public ShipmentDelayed(int shipmentId, string shipmentNumber, string reason, DateTime newExpectedDelivery)
    {
        ShipmentId = shipmentId;
        ShipmentNumber = shipmentNumber;
        Reason = reason;
        NewExpectedDelivery = newExpectedDelivery;
    }
}
