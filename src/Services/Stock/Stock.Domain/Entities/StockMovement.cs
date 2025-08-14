using System;
using TossErp.Stock.Domain.SeedWork;
using TossErp.Stock.Domain.ValueObjects;
using TossErp.Stock.Domain.Enums;
using TossErp.Stock.Domain.Aggregates.ItemAggregate;
using TossErp.Stock.Domain.Aggregates.WarehouseAggregate;
using TossErp.Stock.Domain.Aggregates.WarehouseAggregate.Entities;

namespace TossErp.Stock.Domain.Entities;

public class StockMovement : Entity<Guid>
{
    public Guid TenantId { get; private set; }
    public Guid ItemId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public Guid? BinId { get; private set; }
    public MovementType MovementType { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal? UnitCost { get; private set; }
    public string? ReferenceNumber { get; private set; }
    public string? ReferenceType { get; private set; }
    public string? Reason { get; private set; }
    public DateTime MovementDate { get; private set; }
    public Guid? BatchId { get; private set; }
    public string CreatedBy { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    // Navigation properties
    public virtual ItemAggregate Item { get; private set; } = null!;
    public virtual WarehouseAggregate Warehouse { get; private set; } = null!;
    public virtual Bin? Bin { get; private set; }
    public virtual Batch? Batch { get; private set; }

    // For EF Core
    private StockMovement() { }

    public static StockMovement Create(
        Guid tenantId,
        Guid itemId,
        Guid warehouseId,
        MovementType movementType,
        decimal quantity,
        string createdBy,
        Guid? binId = null,
        decimal? unitCost = null,
        string? referenceNumber = null,
        string? referenceType = null,
        string? reason = null,
        Guid? batchId = null)
    {
        if (string.IsNullOrWhiteSpace(createdBy))
            throw new DomainException("CreatedBy cannot be empty");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero");

        var movement = new StockMovement
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ItemId = itemId,
            WarehouseId = warehouseId,
            BinId = binId,
            MovementType = movementType,
            Quantity = quantity,
            UnitCost = unitCost,
            ReferenceNumber = referenceNumber,
            ReferenceType = referenceType,
            Reason = reason,
            MovementDate = DateTime.UtcNow,
            BatchId = batchId,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        return movement;
    }

    public static StockMovement CreateReceipt(
        Guid tenantId,
        Guid itemId,
        Guid warehouseId,
        decimal quantity,
        string createdBy,
        Guid? binId = null,
        decimal? unitCost = null,
        string? referenceNumber = null,
        string? referenceType = null,
        string? reason = null,
        Guid? batchId = null)
    {
        return Create(tenantId, itemId, warehouseId, MovementType.Receipt, quantity, createdBy, binId, unitCost, referenceNumber, referenceType, reason, batchId);
    }

    public static StockMovement CreateIssue(
        Guid tenantId,
        Guid itemId,
        Guid warehouseId,
        decimal quantity,
        string createdBy,
        Guid? binId = null,
        decimal? unitCost = null,
        string? referenceNumber = null,
        string? referenceType = null,
        string? reason = null)
    {
        return Create(tenantId, itemId, warehouseId, MovementType.Issue, quantity, createdBy, binId, unitCost, referenceNumber, referenceType, reason);
    }

    public static StockMovement CreateAdjustment(
        Guid tenantId,
        Guid itemId,
        Guid warehouseId,
        decimal quantity,
        string createdBy,
        string reason,
        Guid? binId = null,
        decimal? unitCost = null)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Reason is required for adjustments");

        return Create(tenantId, itemId, warehouseId, MovementType.Adjustment, quantity, createdBy, binId, unitCost, reason: reason);
    }

    public static StockMovement CreateTransfer(
        Guid tenantId,
        Guid itemId,
        Guid warehouseId,
        decimal quantity,
        string createdBy,
        string? reason = null,
        Guid? binId = null,
        decimal? unitCost = null)
    {
        return Create(tenantId, itemId, warehouseId, MovementType.Transfer, quantity, createdBy, binId, unitCost, reason: reason);
    }

    public bool IsReceipt => MovementType == MovementType.Receipt;
    public bool IsIssue => MovementType == MovementType.Issue;
    public bool IsAdjustment => MovementType == MovementType.Adjustment;
    public bool IsTransfer => MovementType == MovementType.Transfer;
    public bool IsReturn => MovementType == MovementType.Return;
    public bool IsDamage => MovementType == MovementType.Damage;
    public bool IsExpiry => MovementType == MovementType.Expiry;

    public override string ToString()
    {
        return $"{MovementType} {Quantity} of Item {ItemId} at Warehouse {WarehouseId} on {CreatedAt:yyyy-MM-dd HH:mm:ss}";
    }
}
