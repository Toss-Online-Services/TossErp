using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Events;
using TossErp.Stock.Domain.ValueObjects;

namespace TossErp.Stock.Domain.Entities;

public class ProductWarehouseInventory : BaseAuditableEntity
{
    public ItemCode ItemCode { get; private set; } = null!;
    public WarehouseCode WarehouseCode { get; private set; } = null!;
    public decimal Quantity { get; private set; }
    public decimal ReservedQuantity { get; private set; }
    public bool IsActive { get; private set; } = true;

    private ProductWarehouseInventory() { }

    public ProductWarehouseInventory(ItemCode itemCode, WarehouseCode warehouseCode, decimal quantity, decimal reservedQuantity = 0)
    {
        if (quantity < 0)
            throw new ArgumentException("Quantity cannot be negative.", nameof(quantity));
        if (reservedQuantity < 0)
            throw new ArgumentException("Reserved quantity cannot be negative.", nameof(reservedQuantity));
        ItemCode = itemCode;
        WarehouseCode = warehouseCode;
        Quantity = quantity;
        ReservedQuantity = reservedQuantity;
        AddDomainEvent(new ProductWarehouseInventoryCreatedEvent(this));
    }

    public void UpdateQuantity(decimal quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Quantity cannot be negative.", nameof(quantity));
        Quantity = quantity;
        AddDomainEvent(new ProductWarehouseInventoryUpdatedEvent(this));
    }

    public void UpdateReservedQuantity(decimal reservedQuantity)
    {
        if (reservedQuantity < 0)
            throw new ArgumentException("Reserved quantity cannot be negative.", nameof(reservedQuantity));
        ReservedQuantity = reservedQuantity;
        AddDomainEvent(new ProductWarehouseInventoryReservedUpdatedEvent(this));
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
        AddDomainEvent(new ProductWarehouseInventoryDeactivatedEvent(this));
    }

    public void Activate()
    {
        if (IsActive) return;
        IsActive = true;
        AddDomainEvent(new ProductWarehouseInventoryActivatedEvent(this));
    }

    public void Delete()
    {
        AddDomainEvent(new ProductWarehouseInventoryDeletedEvent(this));
    }
} 
