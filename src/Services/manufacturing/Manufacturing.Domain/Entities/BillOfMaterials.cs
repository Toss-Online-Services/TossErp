using Manufacturing.Domain.Common;
using Manufacturing.Domain.Enums;
using Manufacturing.Domain.ValueObjects;

namespace Manufacturing.Domain.Entities;

/// <summary>
/// Bill of Materials (BOM) aggregate root representing a recipe for manufacturing an item
/// Inspired by ERPNext BOM structure with DDD principles
/// </summary>
public class BillOfMaterials : AggregateRoot<Guid>
{
    public string BomCode { get; private set; } = string.Empty;
    public string ItemCode { get; private set; } = string.Empty;
    public string ItemName { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public string UnitOfMeasure { get; private set; } = string.Empty;
    public BomType BomType { get; private set; }
    public BomStatus Status { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsDefault { get; private set; }
    public DateTime ValidFrom { get; private set; }
    public DateTime? ValidTo { get; private set; }
    
    // BOM structure
    private readonly List<BomItem> _items = new();
    public IReadOnlyCollection<BomItem> Items => _items.AsReadOnly();
    
    private readonly List<BomOperation> _operations = new();
    public IReadOnlyCollection<BomOperation> Operations => _operations.AsReadOnly();
    
    // Cost calculations
    public Money RawMaterialCost { get; private set; } = Money.Zero();
    public Money OperatingCost { get; private set; } = Money.Zero();
    public Money TotalCost { get; private set; } = Money.Zero();
    
    protected BillOfMaterials() : base() { } // For EF Core

    public BillOfMaterials(Guid id, string bomCode, string itemCode, string itemName, 
        decimal quantity, string unitOfMeasure, BomType bomType, string tenantId) : base(id, tenantId)
    {
        if (string.IsNullOrWhiteSpace(bomCode))
            throw new ArgumentException("BOM code cannot be empty", nameof(bomCode));
        if (string.IsNullOrWhiteSpace(itemCode))
            throw new ArgumentException("Item code cannot be empty", nameof(itemCode));
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        BomCode = bomCode.Trim().ToUpper();
        ItemCode = itemCode.Trim();
        ItemName = itemName.Trim();
        Quantity = quantity;
        UnitOfMeasure = unitOfMeasure.Trim();
        BomType = bomType;
        Status = BomStatus.Draft;
        IsActive = true;
        IsDefault = false;
        ValidFrom = DateTime.UtcNow;
    }

    public static BillOfMaterials Create(string bomCode, string itemCode, string itemName,
        decimal quantity, string unitOfMeasure, BomType bomType, string tenantId)
    {
        return new BillOfMaterials(Guid.NewGuid(), bomCode, itemCode, itemName, 
            quantity, unitOfMeasure, bomType, tenantId);
    }

    public void AddItem(string itemCode, string itemName, decimal quantity, string uom, 
        Money rate, bool isStockItem = true, string? description = null)
    {
        if (Status != BomStatus.Draft)
            throw new InvalidOperationException("Cannot modify BOM that is not in draft status");

        var bomItem = new BomItem(itemCode, itemName, quantity, uom, rate, isStockItem, description);
        _items.Add(bomItem);
        
        RecalculateCosts();
        MarkAsUpdated("system");
    }

    public void AddOperation(string operationCode, string operationName, 
        string workstationCode, decimal hourRate, decimal operatingTime)
    {
        if (Status != BomStatus.Draft)
            throw new InvalidOperationException("Cannot modify BOM that is not in draft status");

        var operation = new BomOperation(operationCode, operationName, workstationCode, hourRate, operatingTime);
        _operations.Add(operation);
        
        RecalculateCosts();
        MarkAsUpdated("system");
    }

    public void RemoveItem(string itemCode)
    {
        if (Status != BomStatus.Draft)
            throw new InvalidOperationException("Cannot modify BOM that is not in draft status");

        var item = _items.FirstOrDefault(i => i.ItemCode == itemCode);
        if (item != null)
        {
            _items.Remove(item);
            RecalculateCosts();
            MarkAsUpdated("system");
        }
    }

    public void Submit(string submittedBy)
    {
        if (Status != BomStatus.Draft)
            throw new InvalidOperationException("Only draft BOMs can be submitted");
        
        if (!_items.Any())
            throw new InvalidOperationException("BOM must have at least one item");

        Status = BomStatus.Submitted;
        MarkAsUpdated(submittedBy);
    }

    public void Cancel(string cancelledBy, string? reason = null)
    {
        if (Status == BomStatus.Cancelled)
            throw new InvalidOperationException("BOM is already cancelled");

        Status = BomStatus.Cancelled;
        IsActive = false;
        MarkAsUpdated(cancelledBy);
    }

    public void SetAsDefault()
    {
        if (Status != BomStatus.Submitted)
            throw new InvalidOperationException("Only submitted BOMs can be set as default");

        IsDefault = true;
        MarkAsUpdated("system");
    }

    private void RecalculateCosts()
    {
        // Calculate raw material cost
        var rawMaterialTotal = _items
            .Where(i => i.IsStockItem)
            .Sum(i => i.Amount.Amount);
        
        // Calculate operating cost  
        var operatingTotal = _operations
            .Sum(o => o.TotalCost.Amount);

        RawMaterialCost = new Money(rawMaterialTotal);
        OperatingCost = new Money(operatingTotal);
        TotalCost = RawMaterialCost + OperatingCost;
    }

    public Money GetCostPerUnit()
    {
        return Quantity > 0 ? TotalCost / Quantity : Money.Zero();
    }

    public bool IsValid => _items.Any() && Status != BomStatus.Cancelled;
    
    public string DisplayName => $"{BomCode} - {ItemName}";
}

/// <summary>
/// BOM Item value object representing a single component in the BOM
/// </summary>
public record BomItem
{
    public string ItemCode { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public string UnitOfMeasure { get; init; } = string.Empty;
    public Money Rate { get; init; } = Money.Zero();
    public Money Amount { get; init; } = Money.Zero();
    public bool IsStockItem { get; init; }
    public string? Description { get; init; }

    public BomItem(string itemCode, string itemName, decimal quantity, string uom, 
        Money rate, bool isStockItem = true, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(itemCode))
            throw new ArgumentException("Item code cannot be empty", nameof(itemCode));
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        ItemCode = itemCode.Trim();
        ItemName = itemName.Trim();
        Quantity = quantity;
        UnitOfMeasure = uom.Trim();
        Rate = rate;
        Amount = rate * quantity;
        IsStockItem = isStockItem;
        Description = description?.Trim();
    }
}

/// <summary>
/// BOM Operation value object representing a manufacturing operation
/// </summary>
public record BomOperation
{
    public string OperationCode { get; init; } = string.Empty;
    public string OperationName { get; init; } = string.Empty;
    public string WorkstationCode { get; init; } = string.Empty;
    public decimal HourRate { get; init; }
    public decimal OperatingTime { get; init; } // in hours
    public Money TotalCost { get; init; } = Money.Zero();

    public BomOperation(string operationCode, string operationName, string workstationCode,
        decimal hourRate, decimal operatingTime)
    {
        if (string.IsNullOrWhiteSpace(operationCode))
            throw new ArgumentException("Operation code cannot be empty", nameof(operationCode));
        if (hourRate < 0)
            throw new ArgumentException("Hour rate cannot be negative", nameof(hourRate));
        if (operatingTime <= 0)
            throw new ArgumentException("Operating time must be positive", nameof(operatingTime));

        OperationCode = operationCode.Trim();
        OperationName = operationName.Trim();
        WorkstationCode = workstationCode.Trim();
        HourRate = hourRate;
        OperatingTime = operatingTime;
        TotalCost = new Money(hourRate * operatingTime);
    }
}
