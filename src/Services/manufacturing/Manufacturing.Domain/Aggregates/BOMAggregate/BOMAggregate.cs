using TossErp.Manufacturing.Domain.SeedWork;
using TossErp.Manufacturing.Domain.ValueObjects;
using TossErp.Manufacturing.Domain.Enums;
using TossErp.Manufacturing.Domain.Aggregates.BOMAggregate.Entities;
using TossErp.Manufacturing.Domain.Aggregates.BOMAggregate.Events;

namespace TossErp.Manufacturing.Domain.Aggregates.BOMAggregate;

/// <summary>
/// Bill of Materials Aggregate Root
/// Manages all BOM-related business logic and components
/// </summary>
public class BOMAggregate : Entity, IAggregateRoot
{
    // Core Properties
    public BOMNumber BOMNumber { get; private set; } = null!;
    public string ItemCode { get; private set; } = string.Empty;
    public string ItemName { get; private set; } = string.Empty;
    public Quantity Quantity { get; private set; } = null!;
    public BomType BOMType { get; private set; }
    public BomStatus Status { get; private set; }
    public string Company { get; private set; } = string.Empty;
    
    // Optional Properties
    public string? Description { get; private set; }
    public string? Project { get; private set; }
    public decimal? CostingMethod { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsDefault { get; private set; }
    public DateTime? ValidFrom { get; private set; }
    public DateTime? ValidUpto { get; private set; }
    
    // Operational Metrics
    public ProductionTime? EstimatedTime { get; private set; }
    public decimal? EstimatedCost { get; private set; }
    public decimal? OperatingCost { get; private set; }
    public decimal? RawMaterialCost { get; private set; }
    
    // Child Collections
    private readonly List<BOMItem> _items = new();
    private readonly List<BOMOperation> _operations = new();
    private readonly List<BOMExplosion> _explosions = new();
    
    // Navigation Properties
    public IReadOnlyCollection<BOMItem> Items => _items.AsReadOnly();
    public IReadOnlyCollection<BOMOperation> Operations => _operations.AsReadOnly();
    public IReadOnlyCollection<BOMExplosion> Explosions => _explosions.AsReadOnly();
    
    protected BOMAggregate() { } // For EF Core

    public BOMAggregate(
        BOMNumber bomNumber,
        string itemCode,
        string itemName,
        Quantity quantity,
        BomType bomType,
        string company)
    {
        if (string.IsNullOrWhiteSpace(itemCode))
            throw new ArgumentException("Item code cannot be empty", nameof(itemCode));
        if (string.IsNullOrWhiteSpace(itemName))
            throw new ArgumentException("Item name cannot be empty", nameof(itemName));
        if (string.IsNullOrWhiteSpace(company))
            throw new ArgumentException("Company cannot be empty", nameof(company));

        BOMNumber = bomNumber;
        ItemCode = itemCode;
        ItemName = itemName;
        Quantity = quantity;
        BOMType = bomType;
        Company = company;
        Status = BomStatus.Draft;
        IsActive = true;
        IsDefault = false;

        AddDomainEvent(new BOMCreatedEvent(this));
    }

    // Business Methods
    public void UpdateBasicInfo(string itemName, string? description, Quantity quantity)
    {
        if (string.IsNullOrWhiteSpace(itemName))
            throw new ArgumentException("Item name cannot be empty", nameof(itemName));

        ItemName = itemName;
        Description = description;
        Quantity = quantity;
        MarkAsModified();

        AddDomainEvent(new BOMUpdatedEvent(this));
    }

    public void Submit()
    {
        if (Status != BomStatus.Draft)
            throw new InvalidOperationException("Only draft BOMs can be submitted");

        if (!_items.Any())
            throw new InvalidOperationException("BOM must have at least one item to submit");

        Status = BomStatus.Submitted;
        MarkAsModified();

        AddDomainEvent(new BOMSubmittedEvent(this));
    }

    public void Cancel()
    {
        if (Status == BomStatus.Cancelled)
            throw new InvalidOperationException("BOM is already cancelled");

        Status = BomStatus.Cancelled;
        IsActive = false;
        MarkAsModified();

        AddDomainEvent(new BOMCancelledEvent(this));
    }

    public void SetAsDefault()
    {
        IsDefault = true;
        MarkAsModified();

        AddDomainEvent(new BOMSetAsDefaultEvent(this));
    }

    public void RemoveAsDefault()
    {
        IsDefault = false;
        MarkAsModified();
    }

    public void SetValidityPeriod(DateTime? validFrom, DateTime? validUpto)
    {
        if (validFrom.HasValue && validUpto.HasValue && validFrom > validUpto)
            throw new ArgumentException("Valid from date cannot be after valid upto date");

        ValidFrom = validFrom;
        ValidUpto = validUpto;
        MarkAsModified();
    }

    // Item Management
    public void AddItem(
        string itemCode,
        string itemName,
        Quantity quantity,
        decimal? rate = null,
        string? description = null)
    {
        if (Status != BomStatus.Draft)
            throw new InvalidOperationException("Cannot modify BOM items after submission");

        if (_items.Any(i => i.ItemCode == itemCode))
            throw new InvalidOperationException($"Item {itemCode} already exists in this BOM");

        var bomItem = new BOMItem(itemCode, itemName, quantity, rate, description);
        _items.Add(bomItem);
        
        RecalculateCosts();
        MarkAsModified();

        AddDomainEvent(new BOMItemAddedEvent(this, bomItem));
    }

    public void RemoveItem(Guid itemId)
    {
        if (Status != BomStatus.Draft)
            throw new InvalidOperationException("Cannot modify BOM items after submission");

        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new InvalidOperationException($"Item with id {itemId} not found");

        _items.Remove(item);
        
        RecalculateCosts();
        MarkAsModified();

        AddDomainEvent(new BOMItemRemovedEvent(this, item));
    }

    public void UpdateItemQuantity(Guid itemId, Quantity newQuantity)
    {
        if (Status != BomStatus.Draft)
            throw new InvalidOperationException("Cannot modify BOM items after submission");

        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new InvalidOperationException($"Item with id {itemId} not found");

        item.UpdateQuantity(newQuantity);
        
        RecalculateCosts();
        MarkAsModified();

        AddDomainEvent(new BOMItemUpdatedEvent(this, item));
    }

    // Operation Management
    public void AddOperation(
        string operationName,
        string workstation,
        ProductionTime timeRequired,
        decimal? operatingCost = null,
        string? description = null)
    {
        if (Status != BomStatus.Draft)
            throw new InvalidOperationException("Cannot modify BOM operations after submission");

        var operation = new BOMOperation(operationName, workstation, timeRequired, operatingCost, description);
        _operations.Add(operation);
        
        RecalculateTime();
        RecalculateCosts();
        MarkAsModified();

        AddDomainEvent(new BOMOperationAddedEvent(this, operation));
    }

    public void RemoveOperation(Guid operationId)
    {
        if (Status != BomStatus.Draft)
            throw new InvalidOperationException("Cannot modify BOM operations after submission");

        var operation = _operations.FirstOrDefault(o => o.Id == operationId);
        if (operation == null)
            throw new InvalidOperationException($"Operation with id {operationId} not found");

        _operations.Remove(operation);
        
        RecalculateTime();
        RecalculateCosts();
        MarkAsModified();

        AddDomainEvent(new BOMOperationRemovedEvent(this, operation));
    }

    // Cost Calculation
    private void RecalculateCosts()
    {
        RawMaterialCost = _items.Sum(i => i.TotalCost ?? 0);
        OperatingCost = _operations.Sum(o => o.OperatingCost ?? 0);
        EstimatedCost = RawMaterialCost + OperatingCost;
    }

    private void RecalculateTime()
    {
        if (_operations.Any())
        {
            var totalHours = _operations.Sum(o => o.TimeRequired.TotalHours);
            EstimatedTime = new ProductionTime(totalHours);
        }
        else
        {
            EstimatedTime = null;
        }
    }

    // Business Rules
    public bool IsValidForProduction() =>
        Status == BomStatus.Submitted && 
        IsActive && 
        _items.Any() &&
        (ValidFrom == null || ValidFrom <= DateTime.UtcNow) &&
        (ValidUpto == null || ValidUpto >= DateTime.UtcNow);

    public bool HasSufficientItems() => _items.Any();

    public decimal GetTotalRawMaterialCost() => RawMaterialCost ?? 0;

    public decimal GetTotalOperatingCost() => OperatingCost ?? 0;

    public decimal GetTotalEstimatedCost() => EstimatedCost ?? 0;

    public ProductionTime GetTotalEstimatedTime() => EstimatedTime ?? new ProductionTime(0);
}
