using Manufacturing.Domain.Common;
using Manufacturing.Domain.Enums;
using Manufacturing.Domain.ValueObjects;

namespace Manufacturing.Domain.Entities;

/// <summary>
/// Work Order aggregate root representing a manufacturing job
/// Based on ERPNext Work Order with DDD principles
/// </summary>
public class WorkOrder : AggregateRoot<Guid>
{
    public string WorkOrderNumber { get; private set; } = string.Empty;
    public string ItemCode { get; private set; } = string.Empty;
    public string ItemName { get; private set; } = string.Empty;
    public Guid BomId { get; private set; }
    public decimal PlannedQuantity { get; private set; }
    public decimal ProducedQuantity { get; private set; }
    public decimal ScrapQuantity { get; private set; }
    public string UnitOfMeasure { get; private set; } = string.Empty;
    
    // Dates
    public DateTime PlannedStartDate { get; private set; }
    public DateTime PlannedEndDate { get; private set; }
    public DateTime? ActualStartDate { get; private set; }
    public DateTime? ActualEndDate { get; private set; }
    
    // Status and tracking
    public WorkOrderStatus Status { get; private set; }
    public WorkOrderPriority Priority { get; private set; }
    public string? ProjectCode { get; private set; }
    public string? SalesOrderReference { get; private set; }
    
    // Operations tracking
    private readonly List<WorkOrderOperation> _operations = new();
    public IReadOnlyCollection<WorkOrderOperation> Operations => _operations.AsReadOnly();
    
    // Material consumption
    private readonly List<MaterialConsumption> _materialConsumptions = new();
    public IReadOnlyCollection<MaterialConsumption> MaterialConsumptions => _materialConsumptions.AsReadOnly();
    
    // Costs
    public Money PlannedOperatingCost { get; private set; } = Money.Zero();
    public Money ActualOperatingCost { get; private set; } = Money.Zero();
    public Money MaterialCost { get; private set; } = Money.Zero();
    
    protected WorkOrder() : base() { } // For EF Core

    public WorkOrder(Guid id, string workOrderNumber, string itemCode, string itemName,
        Guid bomId, decimal plannedQuantity, string unitOfMeasure, 
        DateTime plannedStartDate, DateTime plannedEndDate, string tenantId) : base(id, tenantId)
    {
        if (string.IsNullOrWhiteSpace(workOrderNumber))
            throw new ArgumentException("Work order number cannot be empty", nameof(workOrderNumber));
        if (plannedQuantity <= 0)
            throw new ArgumentException("Planned quantity must be positive", nameof(plannedQuantity));
        if (plannedEndDate <= plannedStartDate)
            throw new ArgumentException("Planned end date must be after start date");

        WorkOrderNumber = workOrderNumber.Trim().ToUpper();
        ItemCode = itemCode.Trim();
        ItemName = itemName.Trim();
        BomId = bomId;
        PlannedQuantity = plannedQuantity;
        ProducedQuantity = 0;
        ScrapQuantity = 0;
        UnitOfMeasure = unitOfMeasure.Trim();
        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;
        Status = WorkOrderStatus.Draft;
        Priority = WorkOrderPriority.Medium;
    }

    public static WorkOrder Create(string workOrderNumber, string itemCode, string itemName,
        Guid bomId, decimal plannedQuantity, string unitOfMeasure,
        DateTime plannedStartDate, DateTime plannedEndDate, string tenantId)
    {
        return new WorkOrder(Guid.NewGuid(), workOrderNumber, itemCode, itemName, bomId,
            plannedQuantity, unitOfMeasure, plannedStartDate, plannedEndDate, tenantId);
    }

    public void Submit(string submittedBy)
    {
        if (Status != WorkOrderStatus.Draft)
            throw new InvalidOperationException("Only draft work orders can be submitted");

        Status = WorkOrderStatus.NotStarted;
        MarkAsUpdated(submittedBy);
    }

    public void Start(string startedBy)
    {
        if (Status != WorkOrderStatus.NotStarted)
            throw new InvalidOperationException("Work order must be in NotStarted status to start");

        Status = WorkOrderStatus.InProgress;
        ActualStartDate = DateTime.UtcNow;
        MarkAsUpdated(startedBy);
    }

    public void Complete(string completedBy)
    {
        if (Status != WorkOrderStatus.InProgress)
            throw new InvalidOperationException("Work order must be in progress to complete");

        Status = WorkOrderStatus.Completed;
        ActualEndDate = DateTime.UtcNow;
        MarkAsUpdated(completedBy);
    }

    public void Stop(string stoppedBy, string? reason = null)
    {
        if (Status != WorkOrderStatus.InProgress)
            throw new InvalidOperationException("Work order must be in progress to stop");

        Status = WorkOrderStatus.Stopped;
        MarkAsUpdated(stoppedBy);
    }

    public void Cancel(string cancelledBy, string? reason = null)
    {
        if (Status == WorkOrderStatus.Completed)
            throw new InvalidOperationException("Cannot cancel completed work order");

        Status = WorkOrderStatus.Cancelled;
        MarkAsUpdated(cancelledBy);
    }

    public void AddOperation(string operationCode, string operationName, string workstationCode,
        decimal plannedTime, decimal hourRate)
    {
        if (Status != WorkOrderStatus.Draft)
            throw new InvalidOperationException("Cannot modify operations for non-draft work orders");

        var operation = new WorkOrderOperation(operationCode, operationName, workstationCode, 
            plannedTime, hourRate);
        _operations.Add(operation);
        
        RecalculateOperatingCost();
        MarkAsUpdated("system");
    }

    public void RecordProduction(decimal producedQty, decimal scrapQty, string recordedBy)
    {
        if (Status != WorkOrderStatus.InProgress)
            throw new InvalidOperationException("Can only record production for work orders in progress");

        if (producedQty < 0 || scrapQty < 0)
            throw new ArgumentException("Production quantities cannot be negative");

        var totalProduced = ProducedQuantity + producedQty + scrapQty;
        if (totalProduced > PlannedQuantity)
            throw new InvalidOperationException("Total production cannot exceed planned quantity");

        ProducedQuantity += producedQty;
        ScrapQuantity += scrapQty;
        MarkAsUpdated(recordedBy);
    }

    public void ConsumeMaterial(string itemCode, decimal quantity, Money rate, string consumedBy)
    {
        if (Status != WorkOrderStatus.InProgress)
            throw new InvalidOperationException("Can only consume material for work orders in progress");

        var consumption = new MaterialConsumption(itemCode, quantity, rate, DateTime.UtcNow);
        _materialConsumptions.Add(consumption);
        
        RecalculateMaterialCost();
        MarkAsUpdated(consumedBy);
    }

    public void SetPriority(WorkOrderPriority priority, string updatedBy)
    {
        Priority = priority;
        MarkAsUpdated(updatedBy);
    }

    public void SetProject(string projectCode, string updatedBy)
    {
        ProjectCode = projectCode?.Trim();
        MarkAsUpdated(updatedBy);
    }

    public void SetSalesOrderReference(string salesOrderRef, string updatedBy)
    {
        SalesOrderReference = salesOrderRef?.Trim();
        MarkAsUpdated(updatedBy);
    }

    private void RecalculateOperatingCost()
    {
        var totalCost = _operations.Sum(o => o.PlannedCost.Amount);
        PlannedOperatingCost = new Money(totalCost);
    }

    private void RecalculateMaterialCost()
    {
        var totalCost = _materialConsumptions.Sum(m => m.Amount.Amount);
        MaterialCost = new Money(totalCost);
    }

    // Calculated properties
    public decimal CompletionPercentage => PlannedQuantity > 0 
        ? Math.Min(100, (ProducedQuantity / PlannedQuantity) * 100) 
        : 0;

    public decimal RemainingQuantity => Math.Max(0, PlannedQuantity - ProducedQuantity - ScrapQuantity);

    public bool IsOverdue => ActualEndDate == null && DateTime.UtcNow > PlannedEndDate;

    public TimeSpan? ActualDuration => ActualStartDate.HasValue && ActualEndDate.HasValue
        ? ActualEndDate - ActualStartDate
        : null;

    public TimeSpan PlannedDuration => PlannedEndDate - PlannedStartDate;

    public string DisplayName => $"{WorkOrderNumber} - {ItemName}";
}

/// <summary>
/// Work Order Operation value object representing a single manufacturing operation
/// </summary>
public record WorkOrderOperation
{
    public string OperationCode { get; init; } = string.Empty;
    public string OperationName { get; init; } = string.Empty;
    public string WorkstationCode { get; init; } = string.Empty;
    public decimal PlannedTime { get; init; } // in hours
    public decimal ActualTime { get; init; } // in hours
    public decimal HourRate { get; init; }
    public Money PlannedCost { get; init; } = Money.Zero();
    public Money ActualCost { get; init; } = Money.Zero();
    public OperationStatus Status { get; init; }

    public WorkOrderOperation(string operationCode, string operationName, string workstationCode,
        decimal plannedTime, decimal hourRate)
    {
        if (string.IsNullOrWhiteSpace(operationCode))
            throw new ArgumentException("Operation code cannot be empty", nameof(operationCode));
        if (plannedTime <= 0)
            throw new ArgumentException("Planned time must be positive", nameof(plannedTime));

        OperationCode = operationCode.Trim();
        OperationName = operationName.Trim();
        WorkstationCode = workstationCode.Trim();
        PlannedTime = plannedTime;
        ActualTime = 0;
        HourRate = hourRate;
        PlannedCost = new Money(plannedTime * hourRate);
        ActualCost = Money.Zero();
        Status = OperationStatus.Pending;
    }
}

/// <summary>
/// Material Consumption value object representing material used in production
/// </summary>
public record MaterialConsumption
{
    public string ItemCode { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public Money Rate { get; init; } = Money.Zero();
    public Money Amount { get; init; } = Money.Zero();
    public DateTime ConsumedAt { get; init; }

    public MaterialConsumption(string itemCode, decimal quantity, Money rate, DateTime consumedAt)
    {
        if (string.IsNullOrWhiteSpace(itemCode))
            throw new ArgumentException("Item code cannot be empty", nameof(itemCode));
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        ItemCode = itemCode.Trim();
        Quantity = quantity;
        Rate = rate;
        Amount = rate * quantity;
        ConsumedAt = consumedAt;
    }
}
