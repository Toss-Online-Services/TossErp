using TossErp.Domain.Common;
using TossErp.Domain.Events.Manufacturing;

namespace TossErp.Domain.Entities.Manufacturing;

/// <summary>
/// Work Order - authorization to manufacture a product
/// </summary>
public class WorkOrder : BaseEntity
{
    public string WorkOrderNumber { get; set; } = string.Empty;
    public WorkOrderType Type { get; set; } = WorkOrderType.Production;
    public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Draft;
    
    // Product Information
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductSku { get; set; }
    public int? BomId { get; set; }
    public string? BomNumber { get; set; }
    
    // Quantities
    public decimal QuantityOrdered { get; set; }
    public decimal QuantityProduced { get; set; }
    public decimal QuantityRejected { get; set; }
    public decimal QuantityRemaining => QuantityOrdered - QuantityProduced - QuantityRejected;
    public string? UnitOfMeasure { get; set; }
    
    // Scheduling
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public int Priority { get; set; } = 5; // 1-10, 10 is highest
    
    // Location
    public int? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public int? WorkCenterId { get; set; }
    public string? WorkCenterName { get; set; }
    
    // Costing
    public decimal EstimatedMaterialCost { get; set; }
    public decimal EstimatedLaborCost { get; set; }
    public decimal EstimatedOverheadCost { get; set; }
    public decimal EstimatedTotalCost { get; set; }
    
    public decimal ActualMaterialCost { get; set; }
    public decimal ActualLaborCost { get; set; }
    public decimal ActualOverheadCost { get; set; }
    public decimal ActualTotalCost { get; set; }
    
    // References
    public int? SalesOrderId { get; set; }
    public string? SalesOrderNumber { get; set; }
    public int? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    
    // Approval
    public int? ApprovedBy { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedAt { get; set; }
    
    // Metadata
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public string? SpecialInstructions { get; set; }
    
    // Navigation Properties
    public ICollection<WorkOrderOperation> Operations { get; set; } = new List<WorkOrderOperation>();
    public ICollection<WorkOrderMaterial> Materials { get; set; } = new List<WorkOrderMaterial>();
    public ICollection<ProductionEntry> ProductionEntries { get; set; } = new List<ProductionEntry>();
    
    // Business Methods
    public void Release()
    {
        if (Status != WorkOrderStatus.Approved)
            throw new InvalidOperationException("Work order must be approved before release");
        
        if (!BomId.HasValue)
            throw new InvalidOperationException("BOM is required to release work order");
        
        Status = WorkOrderStatus.Released;
        AddDomainEvent(new WorkOrderReleased(Id, WorkOrderNumber));
    }
    
    public void Start()
    {
        if (Status != WorkOrderStatus.Released)
            throw new InvalidOperationException("Work order must be released before starting");
        
        Status = WorkOrderStatus.InProgress;
        ActualStartDate = DateTime.UtcNow;
        AddDomainEvent(new WorkOrderStarted(Id, WorkOrderNumber));
    }
    
    public void Complete()
    {
        if (Status != WorkOrderStatus.InProgress)
            throw new InvalidOperationException("Only in-progress work orders can be completed");
        
        if (QuantityProduced == 0)
            throw new InvalidOperationException("No production recorded");
        
        Status = WorkOrderStatus.Completed;
        ActualEndDate = DateTime.UtcNow;
        AddDomainEvent(new WorkOrderCompleted(Id, WorkOrderNumber, QuantityProduced));
    }
    
    public void Close()
    {
        if (Status != WorkOrderStatus.Completed)
            throw new InvalidOperationException("Only completed work orders can be closed");
        
        Status = WorkOrderStatus.Closed;
        AddDomainEvent(new WorkOrderClosed(Id, WorkOrderNumber));
    }
    
    public void Cancel(string reason)
    {
        if (Status == WorkOrderStatus.Completed || Status == WorkOrderStatus.Closed)
            throw new InvalidOperationException($"Cannot cancel {Status.ToString().ToLower()} work order");
        
        Status = WorkOrderStatus.Cancelled;
        Notes = $"Cancelled: {reason}\n{Notes}";
        AddDomainEvent(new WorkOrderCancelled(Id, WorkOrderNumber, reason));
    }
}

public enum WorkOrderType
{
    Production,         // Regular production order
    Rework,            // Rework of defective items
    Prototype,         // Prototype/sample production
    MaintenanceOrder,  // For maintenance activities
    Assembly,          // Assembly only (no manufacturing)
    Disassembly        // Breaking down assembled products
}

public enum WorkOrderStatus
{
    Draft,
    Approved,
    Released,          // Released to shop floor
    InProgress,
    OnHold,
    Completed,
    Closed,
    Cancelled
}

