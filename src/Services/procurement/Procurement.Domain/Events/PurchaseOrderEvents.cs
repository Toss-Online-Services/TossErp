using TossErp.Procurement.Domain.Events;

namespace TossErp.Procurement.Domain.Events;

/// <summary>
/// Purchase order created event
/// </summary>
public class PurchaseOrderCreatedEvent : IDomainEvent
{
    public Guid PurchaseOrderId { get; }
    public string PurchaseOrderNumber { get; }
    public Guid SupplierId { get; }
    public string SupplierName { get; }
    public string TenantId { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public PurchaseOrderCreatedEvent(Guid purchaseOrderId, string purchaseOrderNumber, Guid supplierId, string supplierName, string tenantId)
    {
        PurchaseOrderId = purchaseOrderId;
        PurchaseOrderNumber = purchaseOrderNumber;
        SupplierId = supplierId;
        SupplierName = supplierName;
        TenantId = tenantId;
    }
}

/// <summary>
/// Purchase order item added event
/// </summary>
public class PurchaseOrderItemAddedEvent : IDomainEvent
{
    public Guid PurchaseOrderId { get; }
    public Guid ItemId { get; }
    public decimal Quantity { get; }
    public decimal UnitPrice { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public PurchaseOrderItemAddedEvent(Guid purchaseOrderId, Guid itemId, decimal quantity, decimal unitPrice)
    {
        PurchaseOrderId = purchaseOrderId;
        ItemId = itemId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}

/// <summary>
/// Purchase order item removed event
/// </summary>
public class PurchaseOrderItemRemovedEvent : IDomainEvent
{
    public Guid PurchaseOrderId { get; }
    public Guid ItemId { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public PurchaseOrderItemRemovedEvent(Guid purchaseOrderId, Guid itemId)
    {
        PurchaseOrderId = purchaseOrderId;
        ItemId = itemId;
    }
}

/// <summary>
/// Purchase order submitted event
/// </summary>
public class PurchaseOrderSubmittedEvent : IDomainEvent
{
    public Guid PurchaseOrderId { get; }
    public string SubmittedBy { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public PurchaseOrderSubmittedEvent(Guid purchaseOrderId, string submittedBy)
    {
        PurchaseOrderId = purchaseOrderId;
        SubmittedBy = submittedBy;
    }
}

/// <summary>
/// Purchase order approved event
/// </summary>
public class PurchaseOrderApprovedEvent : IDomainEvent
{
    public Guid PurchaseOrderId { get; }
    public string ApprovedBy { get; }
    public string? ApprovalNotes { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public PurchaseOrderApprovedEvent(Guid purchaseOrderId, string approvedBy, string? approvalNotes)
    {
        PurchaseOrderId = purchaseOrderId;
        ApprovedBy = approvedBy;
        ApprovalNotes = approvalNotes;
    }
}

/// <summary>
/// Purchase order sent event
/// </summary>
public class PurchaseOrderSentEvent : IDomainEvent
{
    public Guid PurchaseOrderId { get; }
    public string SentBy { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public PurchaseOrderSentEvent(Guid purchaseOrderId, string sentBy)
    {
        PurchaseOrderId = purchaseOrderId;
        SentBy = sentBy;
    }
}

/// <summary>
/// Purchase order acknowledged event
/// </summary>
public class PurchaseOrderAcknowledgedEvent : IDomainEvent
{
    public Guid PurchaseOrderId { get; }
    public string AcknowledgedBy { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public PurchaseOrderAcknowledgedEvent(Guid purchaseOrderId, string acknowledgedBy)
    {
        PurchaseOrderId = purchaseOrderId;
        AcknowledgedBy = acknowledgedBy;
    }
}

/// <summary>
/// Purchase order partially received event
/// </summary>
public class PurchaseOrderPartiallyReceivedEvent : IDomainEvent
{
    public Guid PurchaseOrderId { get; }
    public Guid ItemId { get; }
    public decimal ReceivedQuantity { get; }
    public string ReceivedBy { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public PurchaseOrderPartiallyReceivedEvent(Guid purchaseOrderId, Guid itemId, decimal receivedQuantity, string receivedBy)
    {
        PurchaseOrderId = purchaseOrderId;
        ItemId = itemId;
        ReceivedQuantity = receivedQuantity;
        ReceivedBy = receivedBy;
    }
}

/// <summary>
/// Purchase order fully received event
/// </summary>
public class PurchaseOrderFullyReceivedEvent : IDomainEvent
{
    public Guid PurchaseOrderId { get; }
    public string ReceivedBy { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public PurchaseOrderFullyReceivedEvent(Guid purchaseOrderId, string receivedBy)
    {
        PurchaseOrderId = purchaseOrderId;
        ReceivedBy = receivedBy;
    }
}

/// <summary>
/// Purchase order cancelled event
/// </summary>
public class PurchaseOrderCancelledEvent : IDomainEvent
{
    public Guid PurchaseOrderId { get; }
    public string CancellationReason { get; }
    public string CancelledBy { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public PurchaseOrderCancelledEvent(Guid purchaseOrderId, string cancellationReason, string cancelledBy)
    {
        PurchaseOrderId = purchaseOrderId;
        CancellationReason = cancellationReason;
        CancelledBy = cancelledBy;
    }
}

/// <summary>
/// Purchase order put on hold event
/// </summary>
public class PurchaseOrderPutOnHoldEvent : IDomainEvent
{
    public Guid PurchaseOrderId { get; }
    public string Reason { get; }
    public string UpdatedBy { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public PurchaseOrderPutOnHoldEvent(Guid purchaseOrderId, string reason, string updatedBy)
    {
        PurchaseOrderId = purchaseOrderId;
        Reason = reason;
        UpdatedBy = updatedBy;
    }
}

/// <summary>
/// Purchase order resumed from hold event
/// </summary>
public class PurchaseOrderResumedFromHoldEvent : IDomainEvent
{
    public Guid PurchaseOrderId { get; }
    public string UpdatedBy { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public PurchaseOrderResumedFromHoldEvent(Guid purchaseOrderId, string updatedBy)
    {
        PurchaseOrderId = purchaseOrderId;
        UpdatedBy = updatedBy;
    }
}

/// <summary>
/// Purchase order delivery date updated event
/// </summary>
public class PurchaseOrderDeliveryDateUpdatedEvent : IDomainEvent
{
    public Guid PurchaseOrderId { get; }
    public DateTime ExpectedDeliveryDate { get; }
    public string UpdatedBy { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public PurchaseOrderDeliveryDateUpdatedEvent(Guid purchaseOrderId, DateTime expectedDeliveryDate, string updatedBy)
    {
        PurchaseOrderId = purchaseOrderId;
        ExpectedDeliveryDate = expectedDeliveryDate;
        UpdatedBy = updatedBy;
    }
}

/// <summary>
/// Purchase order notes updated event
/// </summary>
public class PurchaseOrderNotesUpdatedEvent : IDomainEvent
{
    public Guid PurchaseOrderId { get; }
    public string Notes { get; }
    public string UpdatedBy { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public PurchaseOrderNotesUpdatedEvent(Guid purchaseOrderId, string notes, string updatedBy)
    {
        PurchaseOrderId = purchaseOrderId;
        Notes = notes;
        UpdatedBy = updatedBy;
    }
}
