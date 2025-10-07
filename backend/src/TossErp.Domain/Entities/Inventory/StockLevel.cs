using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.Inventory;

public class StockLevel : BaseEntity
{
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
    
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    
    public int QuantityOnHand { get; set; }
    public int QuantityReserved { get; set; }
    public int QuantityAvailable => QuantityOnHand - QuantityReserved;
    
    public int? ReorderPoint { get; set; }
    public int? ReorderQuantity { get; set; }
    
    public DateTime LastCountDate { get; set; } = DateTime.UtcNow;
    public int? LastCountQuantity { get; set; }
    
    // Business logic
    public void AdjustStock(int quantity, string reason, string adjustedBy)
    {
        var oldQuantity = QuantityOnHand;
        QuantityOnHand += quantity;
        
        if (QuantityOnHand < 0)
            throw new InvalidOperationException("Stock quantity cannot be negative");
        
        UpdatedBy = adjustedBy;
        UpdatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new StockAdjustedEvent(ProductId, WarehouseId, oldQuantity, QuantityOnHand, reason));
    }
    
    public void Reserve(int quantity)
    {
        if (quantity > QuantityAvailable)
            throw new InvalidOperationException($"Cannot reserve {quantity} units. Only {QuantityAvailable} available.");
        
        QuantityReserved += quantity;
        AddDomainEvent(new StockReservedEvent(ProductId, WarehouseId, quantity));
    }
    
    public void ReleaseReservation(int quantity)
    {
        if (quantity > QuantityReserved)
            throw new InvalidOperationException($"Cannot release {quantity} units. Only {QuantityReserved} reserved.");
        
        QuantityReserved -= quantity;
        AddDomainEvent(new StockReservationReleasedEvent(ProductId, WarehouseId, quantity));
    }
    
    public bool IsLowStock()
    {
        return ReorderPoint.HasValue && QuantityAvailable <= ReorderPoint.Value;
    }
}

public class StockMovement : BaseEntity
{
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
    
    public int? FromWarehouseId { get; set; }
    public string? FromWarehouseName { get; set; }
    
    public int? ToWarehouseId { get; set; }
    public string? ToWarehouseName { get; set; }
    
    public int Quantity { get; set; }
    public string MovementType { get; set; } = string.Empty; // Purchase, Sale, Transfer, Adjustment, Return
    public string? ReferenceType { get; set; } // Sale, PurchaseOrder, etc.
    public int? ReferenceId { get; set; }
    public string? ReferenceNumber { get; set; }
    
    public DateTime MovementDate { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}

// Domain Events
public class StockAdjustedEvent : DomainEvent
{
    public int ProductId { get; }
    public int WarehouseId { get; }
    public int OldQuantity { get; }
    public int NewQuantity { get; }
    public string Reason { get; }
    
    public StockAdjustedEvent(int productId, int warehouseId, int oldQuantity, int newQuantity, string reason)
    {
        ProductId = productId;
        WarehouseId = warehouseId;
        OldQuantity = oldQuantity;
        NewQuantity = newQuantity;
        Reason = reason;
    }
}

public class StockReservedEvent : DomainEvent
{
    public int ProductId { get; }
    public int WarehouseId { get; }
    public int Quantity { get; }
    
    public StockReservedEvent(int productId, int warehouseId, int quantity)
    {
        ProductId = productId;
        WarehouseId = warehouseId;
        Quantity = quantity;
    }
}

public class StockReservationReleasedEvent : DomainEvent
{
    public int ProductId { get; }
    public int WarehouseId { get; }
    public int Quantity { get; }
    
    public StockReservationReleasedEvent(int productId, int warehouseId, int quantity)
    {
        ProductId = productId;
        WarehouseId = warehouseId;
        Quantity = quantity;
    }
}

