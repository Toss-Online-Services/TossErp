using System;
using TossErp.Stock.Domain.SeedWork;
using TossErp.Stock.Domain.Aggregates.ItemAggregate;
using TossErp.Stock.Domain.Aggregates.WarehouseAggregate;
using TossErp.Stock.Domain.Aggregates.WarehouseAggregate.Entities;

namespace TossErp.Stock.Domain.Entities;

public class StockLevel : Entity<Guid>
{
    public Guid ItemId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public Guid? BinId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal ReservedQuantity { get; private set; }
    public decimal UnitCost { get; private set; }
    public DateTime LastMovementDate { get; private set; }
    public DateTime LastUpdated { get; private set; }

    // Navigation properties
    public virtual ItemAggregate Item { get; private set; } = null!;
    public virtual WarehouseAggregate Warehouse { get; private set; } = null!;
    public virtual Bin? Bin { get; private set; }

    // For EF Core
    private StockLevel() { }

    public static StockLevel Create(Guid itemId, Guid warehouseId, Guid? binId = null, decimal quantity = 0, decimal unitCost = 0)
    {
        var stockLevel = new StockLevel
        {
            Id = Guid.NewGuid(),
            ItemId = itemId,
            WarehouseId = warehouseId,
            BinId = binId,
            Quantity = quantity,
            ReservedQuantity = 0,
            UnitCost = unitCost,
            LastMovementDate = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        return stockLevel;
    }

    public void UpdateStock(decimal quantity, decimal unitCost = 0)
    {
        if (quantity < 0)
            throw new DomainException("Quantity cannot be negative");

        Quantity = quantity;
        if (unitCost > 0)
            UnitCost = unitCost;
        LastMovementDate = DateTime.UtcNow;
        LastUpdated = DateTime.UtcNow;
    }

    public void ReceiveStock(decimal quantity, decimal unitCost = 0)
    {
        if (quantity <= 0)
            throw new DomainException("Received quantity must be greater than zero");

        Quantity += quantity;
        if (unitCost > 0)
            UnitCost = unitCost;
        LastMovementDate = DateTime.UtcNow;
        LastUpdated = DateTime.UtcNow;
    }

    public void IssueStock(decimal quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Issued quantity must be greater than zero");

        var availableStock = Quantity - ReservedQuantity;
        if (availableStock < quantity)
            throw new DomainException($"Insufficient available stock. Available: {availableStock}, Requested: {quantity}");

        Quantity -= quantity;
        LastMovementDate = DateTime.UtcNow;
        LastUpdated = DateTime.UtcNow;
    }

    public void ReserveStock(decimal quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Reserved quantity must be greater than zero");

        var availableStock = Quantity - ReservedQuantity;
        if (availableStock < quantity)
            throw new DomainException($"Insufficient available stock to reserve. Available: {availableStock}, Requested: {quantity}");

        ReservedQuantity += quantity;
        LastUpdated = DateTime.UtcNow;
    }

    public void ReleaseReservation(decimal quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Released quantity must be greater than zero");

        if (ReservedQuantity < quantity)
            throw new DomainException($"Cannot release more than reserved. Reserved: {ReservedQuantity}, Requested: {quantity}");

        ReservedQuantity -= quantity;
        LastUpdated = DateTime.UtcNow;
    }

    public decimal GetAvailableQuantity()
    {
        return Quantity - ReservedQuantity;
    }

    public bool HasAvailableStock(decimal requiredQuantity)
    {
        return GetAvailableQuantity() >= requiredQuantity;
    }

    public bool IsLowStock(decimal reorderLevel)
    {
        return GetAvailableQuantity() < reorderLevel;
    }
} 
