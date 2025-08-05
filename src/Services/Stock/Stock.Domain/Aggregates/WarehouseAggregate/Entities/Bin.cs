using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.ValueObjects;

namespace TossErp.Stock.Domain.Aggregates.WarehouseAggregate.Entities;

/// <summary>
/// Bin - Child entity of Warehouse Aggregate
/// Represents storage locations within a warehouse
/// </summary>
public class Bin : Entity
{
    public BinCode BinCode { get; private set; } = null!;
    public string BinName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public string? BinType { get; private set; }
    public decimal? Capacity { get; private set; }
    public string? CapacityUOM { get; private set; }

    protected Bin() { } // For EF Core

    public Bin(BinCode binCode, string binName, string? description = null, string? binType = null)
    {
        if (string.IsNullOrWhiteSpace(binName))
            throw new ArgumentException("Bin name cannot be empty", nameof(binName));

        BinCode = binCode;
        BinName = binName.Trim();
        Description = description?.Trim();
        BinType = binType?.Trim();
        IsActive = true;
    }

    public void UpdateInfo(string binName, string? description)
    {
        if (string.IsNullOrWhiteSpace(binName))
            throw new ArgumentException("Bin name cannot be empty", nameof(binName));

        BinName = binName.Trim();
        Description = description?.Trim();
    }

    public void UpdateBinType(string? binType)
    {
        BinType = binType?.Trim();
    }

    public void UpdateCapacity(decimal? capacity, string? capacityUOM)
    {
        if (capacity.HasValue && capacity.Value < 0)
            throw new ArgumentException("Capacity cannot be negative", nameof(capacity));

        Capacity = capacity;
        CapacityUOM = capacityUOM?.Trim();
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public bool IsAvailable() => IsActive;

    public bool HasCapacity() => Capacity.HasValue && Capacity.Value > 0;

    public bool CanAccommodate(decimal quantity)
    {
        if (!HasCapacity()) return true;
        return quantity <= Capacity!.Value;
    }
} 
