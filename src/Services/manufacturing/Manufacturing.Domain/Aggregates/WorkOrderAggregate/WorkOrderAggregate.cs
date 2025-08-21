using TossErp.Manufacturing.Domain.SeedWork;
using TossErp.Manufacturing.Domain.ValueObjects;
using TossErp.Manufacturing.Domain.Enums;
using TossErp.Manufacturing.Domain.Aggregates.WorkOrderAggregate.Entities;
using TossErp.Manufacturing.Domain.Aggregates.WorkOrderAggregate.Events;

namespace TossErp.Manufacturing.Domain.Aggregates.WorkOrderAggregate;

/// <summary>
/// Work Order Aggregate Root
/// Manages manufacturing work order execution and operations
/// </summary>
public class WorkOrderAggregate : Entity, IAggregateRoot
{
    // Core Properties
    public WorkOrderNumber WorkOrderNumber { get; private set; } = null!;
    public string ItemCode { get; private set; } = string.Empty;
    public string ItemName { get; private set; } = string.Empty;
    public BOMNumber? BOMNumber { get; private set; }
    public Quantity PlannedQuantity { get; private set; } = null!;
    public Quantity ProducedQuantity { get; private set; } = null!;
    public WorkOrderStatus Status { get; private set; }
    public WorkOrderPriority Priority { get; private set; }
    public string Company { get; private set; } = string.Empty;
    
    // Scheduling
    public DateTime PlannedStartDate { get; private set; }
    public DateTime PlannedEndDate { get; private set; }
    public DateTime? ActualStartDate { get; private set; }
    public DateTime? ActualEndDate { get; private set; }
    public ProductionTime? EstimatedTime { get; private set; }
    public ProductionTime? ActualTime { get; private set; }
    
    // Cost Tracking
    public decimal? EstimatedCost { get; private set; }
    public decimal? ActualCost { get; private set; }
    public decimal? MaterialCost { get; private set; }
    public decimal? OperatingCost { get; private set; }
    
    // References
    public string? SalesOrder { get; private set; }
    public string? Project { get; private set; }
    public string? SourceWarehouse { get; private set; }
    public string? TargetWarehouse { get; private set; }
    
    // Optional Properties
    public string? Description { get; private set; }
    public bool SkipTransfer { get; private set; }
    public bool AllowAlternativeItem { get; private set; }
    public bool AutoTransferMaterial { get; private set; }
    
    // Child Collections
    private readonly List<WorkOrderOperation> _operations = new();
    private readonly List<WorkOrderItem> _requiredItems = new();
    private readonly List<MaterialRequest> _materialRequests = new();
    private readonly List<QualityInspection> _qualityInspections = new();
    
    // Navigation Properties
    public IReadOnlyCollection<WorkOrderOperation> Operations => _operations.AsReadOnly();
    public IReadOnlyCollection<WorkOrderItem> RequiredItems => _requiredItems.AsReadOnly();
    public IReadOnlyCollection<MaterialRequest> MaterialRequests => _materialRequests.AsReadOnly();
    public IReadOnlyCollection<QualityInspection> QualityInspections => _qualityInspections.AsReadOnly();
    
    protected WorkOrderAggregate() { } // For EF Core

    public WorkOrderAggregate(
        WorkOrderNumber workOrderNumber,
        string itemCode,
        string itemName,
        Quantity plannedQuantity,
        DateTime plannedStartDate,
        DateTime plannedEndDate,
        string company,
        BOMNumber? bomNumber = null)
    {
        if (string.IsNullOrWhiteSpace(itemCode))
            throw new ArgumentException("Item code cannot be empty", nameof(itemCode));
        if (string.IsNullOrWhiteSpace(itemName))
            throw new ArgumentException("Item name cannot be empty", nameof(itemName));
        if (string.IsNullOrWhiteSpace(company))
            throw new ArgumentException("Company cannot be empty", nameof(company));
        if (plannedStartDate >= plannedEndDate)
            throw new ArgumentException("Planned start date must be before planned end date");

        WorkOrderNumber = workOrderNumber;
        ItemCode = itemCode;
        ItemName = itemName;
        BOMNumber = bomNumber;
        PlannedQuantity = plannedQuantity;
        ProducedQuantity = new Quantity(0, plannedQuantity.UOM);
        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;
        Company = company;
        Status = WorkOrderStatus.Draft;
        Priority = WorkOrderPriority.Medium;
        SkipTransfer = false;
        AllowAlternativeItem = false;
        AutoTransferMaterial = false;

        AddDomainEvent(new WorkOrderCreatedEvent(this));
    }

    // Status Management
    public void Submit()
    {
        if (Status != WorkOrderStatus.Draft)
            throw new InvalidOperationException("Only draft work orders can be submitted");

        Status = WorkOrderStatus.NotStarted;
        MarkAsModified();

        AddDomainEvent(new WorkOrderSubmittedEvent(this));
    }

    public void Start()
    {
        if (Status != WorkOrderStatus.NotStarted)
            throw new InvalidOperationException("Work order must be in NotStarted status to start");

        Status = WorkOrderStatus.InProgress;
        ActualStartDate = DateTime.UtcNow;
        MarkAsModified();

        AddDomainEvent(new WorkOrderStartedEvent(this));
    }

    public void Stop()
    {
        if (Status != WorkOrderStatus.InProgress)
            throw new InvalidOperationException("Work order must be in progress to stop");

        Status = WorkOrderStatus.Stopped;
        MarkAsModified();

        AddDomainEvent(new WorkOrderStoppedEvent(this));
    }

    public void Resume()
    {
        if (Status != WorkOrderStatus.Stopped)
            throw new InvalidOperationException("Work order must be stopped to resume");

        Status = WorkOrderStatus.InProgress;
        MarkAsModified();

        AddDomainEvent(new WorkOrderResumedEvent(this));
    }

    public void Complete()
    {
        if (Status != WorkOrderStatus.InProgress)
            throw new InvalidOperationException("Work order must be in progress to complete");

        if (ProducedQuantity.Value <= 0)
            throw new InvalidOperationException("Cannot complete work order with zero produced quantity");

        Status = WorkOrderStatus.Completed;
        ActualEndDate = DateTime.UtcNow;
        CalculateActualTime();
        MarkAsModified();

        AddDomainEvent(new WorkOrderCompletedEvent(this));
    }

    public void Cancel()
    {
        if (Status == WorkOrderStatus.Completed)
            throw new InvalidOperationException("Cannot cancel completed work order");
        if (Status == WorkOrderStatus.Cancelled)
            throw new InvalidOperationException("Work order is already cancelled");

        Status = WorkOrderStatus.Cancelled;
        MarkAsModified();

        AddDomainEvent(new WorkOrderCancelledEvent(this));
    }

    // Production Tracking
    public void UpdateProducedQuantity(Quantity quantity)
    {
        if (Status != WorkOrderStatus.InProgress)
            throw new InvalidOperationException("Can only update produced quantity for work orders in progress");

        if (quantity.UOM != PlannedQuantity.UOM)
            throw new InvalidOperationException($"Produced quantity UOM {quantity.UOM} must match planned UOM {PlannedQuantity.UOM}");

        if (quantity.Value > PlannedQuantity.Value)
            throw new InvalidOperationException("Produced quantity cannot exceed planned quantity");

        ProducedQuantity = quantity;
        MarkAsModified();

        AddDomainEvent(new WorkOrderProductionUpdatedEvent(this));
    }

    public void RecordActualCost(decimal cost)
    {
        if (cost < 0)
            throw new ArgumentException("Actual cost cannot be negative", nameof(cost));

        ActualCost = cost;
        MarkAsModified();

        AddDomainEvent(new WorkOrderCostUpdatedEvent(this));
    }

    // Operations Management
    public void AddOperation(
        string operationName,
        string workstation,
        ProductionTime plannedTime,
        decimal? operatingCost = null,
        string? description = null)
    {
        if (Status != WorkOrderStatus.Draft)
            throw new InvalidOperationException("Cannot modify operations after work order submission");

        var operation = new WorkOrderOperation(
            operationName, 
            workstation, 
            plannedTime, 
            operatingCost, 
            description);
        
        _operations.Add(operation);
        RecalculateEstimatedTime();
        RecalculateEstimatedCost();
        MarkAsModified();

        AddDomainEvent(new WorkOrderOperationAddedEvent(this, operation));
    }

    public void CompleteOperation(Guid operationId, ProductionTime actualTime, decimal? actualCost = null)
    {
        var operation = _operations.FirstOrDefault(o => o.Id == operationId);
        if (operation == null)
            throw new InvalidOperationException($"Operation with id {operationId} not found");

        operation.Complete(actualTime, actualCost);
        RecalculateActualCost();
        MarkAsModified();

        AddDomainEvent(new WorkOrderOperationCompletedEvent(this, operation));
    }

    // Material Management
    public void AddRequiredItem(
        string itemCode,
        string itemName,
        Quantity requiredQuantity,
        string? sourceWarehouse = null)
    {
        if (Status != WorkOrderStatus.Draft)
            throw new InvalidOperationException("Cannot modify required items after work order submission");

        if (_requiredItems.Any(i => i.ItemCode == itemCode))
            throw new InvalidOperationException($"Item {itemCode} is already in the required items list");

        var workOrderItem = new WorkOrderItem(itemCode, itemName, requiredQuantity, sourceWarehouse);
        _requiredItems.Add(workOrderItem);
        MarkAsModified();

        AddDomainEvent(new WorkOrderItemAddedEvent(this, workOrderItem));
    }

    public void TransferMaterial(Guid itemId, Quantity transferredQuantity)
    {
        var item = _requiredItems.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new InvalidOperationException($"Item with id {itemId} not found");

        item.Transfer(transferredQuantity);
        MarkAsModified();

        AddDomainEvent(new WorkOrderMaterialTransferredEvent(this, item));
    }

    // Calculation Methods
    private void CalculateActualTime()
    {
        if (ActualStartDate.HasValue && ActualEndDate.HasValue)
        {
            var duration = ActualEndDate.Value - ActualStartDate.Value;
            ActualTime = new ProductionTime(duration);
        }
    }

    private void RecalculateEstimatedTime()
    {
        if (_operations.Any())
        {
            var totalHours = _operations.Sum(o => o.PlannedTime.TotalHours);
            EstimatedTime = new ProductionTime(totalHours);
        }
    }

    private void RecalculateEstimatedCost()
    {
        OperatingCost = _operations.Sum(o => o.OperatingCost ?? 0);
        MaterialCost = _requiredItems.Sum(i => i.EstimatedCost ?? 0);
        EstimatedCost = OperatingCost + MaterialCost;
    }

    private void RecalculateActualCost()
    {
        var actualOperatingCost = _operations.Sum(o => o.ActualCost ?? o.OperatingCost ?? 0);
        var actualMaterialCost = _requiredItems.Sum(i => i.ActualCost ?? i.EstimatedCost ?? 0);
        ActualCost = actualOperatingCost + actualMaterialCost;
    }

    // Business Rules
    public bool IsReadyForProduction() =>
        Status == WorkOrderStatus.NotStarted &&
        _requiredItems.All(i => i.IsTransferred) &&
        PlannedQuantity.Value > 0;

    public bool CanBeCompleted() =>
        Status == WorkOrderStatus.InProgress &&
        ProducedQuantity.Value > 0 &&
        _operations.All(o => o.Status == OperationStatus.Completed);

    public decimal GetCompletionPercentage() =>
        PlannedQuantity.Value > 0 ? (ProducedQuantity.Value / PlannedQuantity.Value) * 100 : 0;

    public bool IsOverdue() =>
        Status != WorkOrderStatus.Completed && 
        Status != WorkOrderStatus.Cancelled && 
        DateTime.UtcNow > PlannedEndDate;

    public TimeSpan GetScheduleVariance() =>
        ActualEndDate.HasValue ? ActualEndDate.Value - PlannedEndDate : TimeSpan.Zero;
}
