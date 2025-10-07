using TossErp.Domain.Common;

namespace TossErp.Domain.Events.Manufacturing;

// BOM Events
public class BomActivated : DomainEvent
{
    public int BomId { get; }
    public string BomNumber { get; }
    
    public BomActivated(int bomId, string bomNumber)
    {
        BomId = bomId;
        BomNumber = bomNumber;
    }
}

public class BomApproved : DomainEvent
{
    public int BomId { get; }
    public string BomNumber { get; }
    public int ApprovedBy { get; }
    
    public BomApproved(int bomId, string bomNumber, int approvedBy)
    {
        BomId = bomId;
        BomNumber = bomNumber;
        ApprovedBy = approvedBy;
    }
}

public class BomObsoleted : DomainEvent
{
    public int BomId { get; }
    public string BomNumber { get; }
    
    public BomObsoleted(int bomId, string bomNumber)
    {
        BomId = bomId;
        BomNumber = bomNumber;
    }
}

// Work Order Events
public class WorkOrderReleased : DomainEvent
{
    public int WorkOrderId { get; }
    public string WorkOrderNumber { get; }
    
    public WorkOrderReleased(int workOrderId, string workOrderNumber)
    {
        WorkOrderId = workOrderId;
        WorkOrderNumber = workOrderNumber;
    }
}

public class WorkOrderStarted : DomainEvent
{
    public int WorkOrderId { get; }
    public string WorkOrderNumber { get; }
    
    public WorkOrderStarted(int workOrderId, string workOrderNumber)
    {
        WorkOrderId = workOrderId;
        WorkOrderNumber = workOrderNumber;
    }
}

public class WorkOrderCompleted : DomainEvent
{
    public int WorkOrderId { get; }
    public string WorkOrderNumber { get; }
    public decimal QuantityProduced { get; }
    
    public WorkOrderCompleted(int workOrderId, string workOrderNumber, decimal quantityProduced)
    {
        WorkOrderId = workOrderId;
        WorkOrderNumber = workOrderNumber;
        QuantityProduced = quantityProduced;
    }
}

public class WorkOrderClosed : DomainEvent
{
    public int WorkOrderId { get; }
    public string WorkOrderNumber { get; }
    
    public WorkOrderClosed(int workOrderId, string workOrderNumber)
    {
        WorkOrderId = workOrderId;
        WorkOrderNumber = workOrderNumber;
    }
}

public class WorkOrderCancelled : DomainEvent
{
    public int WorkOrderId { get; }
    public string WorkOrderNumber { get; }
    public string Reason { get; }
    
    public WorkOrderCancelled(int workOrderId, string workOrderNumber, string reason)
    {
        WorkOrderId = workOrderId;
        WorkOrderNumber = workOrderNumber;
        Reason = reason;
    }
}

