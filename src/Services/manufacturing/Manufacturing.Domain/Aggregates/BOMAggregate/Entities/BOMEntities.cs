using TossErp.Manufacturing.Domain.SeedWork;
using TossErp.Manufacturing.Domain.ValueObjects;

namespace TossErp.Manufacturing.Domain.Aggregates.BOMAggregate.Entities;

/// <summary>
/// BOM Item - represents a component item in the bill of materials
/// </summary>
public class BOMItem : Entity
{
    public string ItemCode { get; private set; } = string.Empty;
    public string ItemName { get; private set; } = string.Empty;
    public Quantity Quantity { get; private set; } = null!;
    public decimal? Rate { get; private set; }
    public string? Description { get; private set; }
    public bool IsOptional { get; private set; }
    public string? ItemGroup { get; private set; }
    public string? UOM { get; private set; }
    public decimal? ConversionFactor { get; private set; }
    
    // Calculated properties
    public decimal? TotalCost => Rate.HasValue ? Rate.Value * Quantity.Value : null;

    protected BOMItem() { } // For EF Core

    public BOMItem(
        string itemCode,
        string itemName,
        Quantity quantity,
        decimal? rate = null,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(itemCode))
            throw new ArgumentException("Item code cannot be empty", nameof(itemCode));
        if (string.IsNullOrWhiteSpace(itemName))
            throw new ArgumentException("Item name cannot be empty", nameof(itemName));

        ItemCode = itemCode;
        ItemName = itemName;
        Quantity = quantity;
        Rate = rate;
        Description = description;
        IsOptional = false;
        UOM = quantity.UOM;
    }

    public void UpdateQuantity(Quantity newQuantity)
    {
        Quantity = newQuantity;
        UOM = newQuantity.UOM;
        MarkAsModified();
    }

    public void UpdateRate(decimal newRate)
    {
        if (newRate < 0)
            throw new ArgumentException("Rate cannot be negative", nameof(newRate));

        Rate = newRate;
        MarkAsModified();
    }

    public void SetAsOptional(bool isOptional = true)
    {
        IsOptional = isOptional;
        MarkAsModified();
    }

    public void UpdateConversionFactor(decimal conversionFactor)
    {
        if (conversionFactor <= 0)
            throw new ArgumentException("Conversion factor must be positive", nameof(conversionFactor));

        ConversionFactor = conversionFactor;
        MarkAsModified();
    }
}

/// <summary>
/// BOM Operation - represents a manufacturing operation/step
/// </summary>
public class BOMOperation : Entity
{
    public string OperationName { get; private set; } = string.Empty;
    public string Workstation { get; private set; } = string.Empty;
    public ProductionTime TimeRequired { get; private set; } = null!;
    public decimal? OperatingCost { get; private set; }
    public string? Description { get; private set; }
    public int Sequence { get; private set; }
    public bool IsOptional { get; private set; }
    public string? QualityCheck { get; private set; }
    
    // Calculated properties
    public decimal? HourlyRate { get; private set; }
    public decimal? TotalCost => OperatingCost ?? (HourlyRate * (decimal?)TimeRequired.TotalHours);

    protected BOMOperation() { } // For EF Core

    public BOMOperation(
        string operationName,
        string workstation,
        ProductionTime timeRequired,
        decimal? operatingCost = null,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(operationName))
            throw new ArgumentException("Operation name cannot be empty", nameof(operationName));
        if (string.IsNullOrWhiteSpace(workstation))
            throw new ArgumentException("Workstation cannot be empty", nameof(workstation));

        OperationName = operationName;
        Workstation = workstation;
        TimeRequired = timeRequired;
        OperatingCost = operatingCost;
        Description = description;
        IsOptional = false;
        Sequence = 0;
    }

    public void UpdateTimeRequired(ProductionTime newTime)
    {
        TimeRequired = newTime;
        MarkAsModified();
    }

    public void UpdateOperatingCost(decimal newCost)
    {
        if (newCost < 0)
            throw new ArgumentException("Operating cost cannot be negative", nameof(newCost));

        OperatingCost = newCost;
        MarkAsModified();
    }

    public void SetSequence(int sequence)
    {
        if (sequence < 0)
            throw new ArgumentException("Sequence cannot be negative", nameof(sequence));

        Sequence = sequence;
        MarkAsModified();
    }

    public void SetAsOptional(bool isOptional = true)
    {
        IsOptional = isOptional;
        MarkAsModified();
    }

    public void SetHourlyRate(decimal hourlyRate)
    {
        if (hourlyRate < 0)
            throw new ArgumentException("Hourly rate cannot be negative", nameof(hourlyRate));

        HourlyRate = hourlyRate;
        MarkAsModified();
    }
}

/// <summary>
/// BOM Explosion - represents exploded/detailed view of BOM hierarchy
/// </summary>
public class BOMExplosion : Entity
{
    public string ItemCode { get; private set; } = string.Empty;
    public string ItemName { get; private set; } = string.Empty;
    public Quantity RequiredQuantity { get; private set; } = null!;
    public int Level { get; private set; }
    public string? ParentItemCode { get; private set; }
    public decimal? Rate { get; private set; }
    public decimal? Amount { get; private set; }
    public bool IsRawMaterial { get; private set; }
    public bool IsSubAssembly { get; private set; }

    protected BOMExplosion() { } // For EF Core

    public BOMExplosion(
        string itemCode,
        string itemName,
        Quantity requiredQuantity,
        int level,
        string? parentItemCode = null)
    {
        if (string.IsNullOrWhiteSpace(itemCode))
            throw new ArgumentException("Item code cannot be empty", nameof(itemCode));
        if (string.IsNullOrWhiteSpace(itemName))
            throw new ArgumentException("Item name cannot be empty", nameof(itemName));
        if (level < 0)
            throw new ArgumentException("Level cannot be negative", nameof(level));

        ItemCode = itemCode;
        ItemName = itemName;
        RequiredQuantity = requiredQuantity;
        Level = level;
        ParentItemCode = parentItemCode;
        IsRawMaterial = level > 0; // Assume items at level > 0 are raw materials
        IsSubAssembly = level == 0; // Root level items are sub-assemblies
    }

    public void UpdateRate(decimal rate)
    {
        if (rate < 0)
            throw new ArgumentException("Rate cannot be negative", nameof(rate));

        Rate = rate;
        Amount = rate * RequiredQuantity.Value;
        MarkAsModified();
    }

    public void MarkAsRawMaterial()
    {
        IsRawMaterial = true;
        IsSubAssembly = false;
        MarkAsModified();
    }

    public void MarkAsSubAssembly()
    {
        IsSubAssembly = true;
        IsRawMaterial = false;
        MarkAsModified();
    }
}
