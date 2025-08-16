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
    
    // Alias property for compatibility with tests
    public decimal QuantityOnHand => Quantity;
    public string ItemCode => Item?.ItemCode?.Value ?? string.Empty;
    public string ItemName => Item?.ItemName ?? string.Empty;
    public string WarehouseCode => Warehouse?.WarehouseCode ?? string.Empty;
    public string BinCode => Bin?.BinCode ?? string.Empty;
    public string UnitOfMeasure => Item?.StockUOM?.Code ?? string.Empty;
    public decimal DefaultPrice => Item?.DefaultPrice ?? 0;
    public decimal ReorderLevel => Item?.ReorderLevel ?? 0;
    public decimal ReorderQuantity => Item?.ReorderQuantity ?? 0;
    public bool IsActive => Item?.IsActive ?? false;
    public DateTime LastMovementDate { get; private set; }
    public DateTime LastUpdated { get; private set; }

    // Navigation properties
    public virtual ItemAggregate Item { get; private set; } = null!;
    public virtual WarehouseAggregate Warehouse { get; private set; } = null!;
    public virtual Bin? Bin { get; private set; }

    protected StockLevel() { } // For EF Core

    // Constructor for compatibility with tests
    public StockLevel(Guid itemId, Guid warehouseId, Guid binId, decimal quantity, decimal unitCost)
    {
        if (itemId == Guid.Empty)
            throw new ArgumentException("Item ID cannot be empty", nameof(itemId));
        if (warehouseId == Guid.Empty)
            throw new ArgumentException("Warehouse ID cannot be empty", nameof(warehouseId));
        if (binId == Guid.Empty)
            throw new ArgumentException("Bin ID cannot be empty", nameof(binId));
        if (quantity < 0)
            throw new ArgumentException("Quantity cannot be negative", nameof(quantity));
        if (unitCost < 0)
            throw new ArgumentException("Unit cost cannot be negative", nameof(unitCost));

        Id = Guid.NewGuid();
        ItemId = itemId;
        WarehouseId = warehouseId;
        BinId = binId;
        Quantity = quantity;
        UnitCost = unitCost;
        ReservedQuantity = 0;
        LastMovementDate = DateTime.UtcNow;
        LastUpdated = DateTime.UtcNow;
    }

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
