using TossErp.Domain.Common;

namespace TossErp.Domain.Events.Manufacturing;

// BOM Events
public record BomActivated(int BomId, string BomNumber) : IDomainEvent;
public record BomApproved(int BomId, string BomNumber, int ApprovedBy) : IDomainEvent;
public record BomObsoleted(int BomId, string BomNumber) : IDomainEvent;

// Work Order Events
public record WorkOrderReleased(int WorkOrderId, string WorkOrderNumber) : IDomainEvent;
public record WorkOrderStarted(int WorkOrderId, string WorkOrderNumber) : IDomainEvent;
public record WorkOrderCompleted(int WorkOrderId, string WorkOrderNumber, decimal QuantityProduced) : IDomainEvent;
public record WorkOrderClosed(int WorkOrderId, string WorkOrderNumber) : IDomainEvent;
public record WorkOrderCancelled(int WorkOrderId, string WorkOrderNumber, string Reason) : IDomainEvent;

