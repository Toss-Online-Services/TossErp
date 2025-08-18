namespace TossErp.Procurement.Domain.Enums;

/// <summary>
/// Status of a purchase order
/// </summary>
public enum PurchaseOrderStatus
{
    /// <summary>
    /// Purchase order is being created/drafted
    /// </summary>
    Draft = 1,

    /// <summary>
    /// Purchase order has been submitted for approval
    /// </summary>
    Submitted = 2,

    /// <summary>
    /// Purchase order has been approved
    /// </summary>
    Approved = 3,

    /// <summary>
    /// Purchase order has been sent to supplier
    /// </summary>
    Sent = 4,

    /// <summary>
    /// Purchase order has been acknowledged by supplier
    /// </summary>
    Acknowledged = 5,

    /// <summary>
    /// Purchase order is partially received
    /// </summary>
    PartiallyReceived = 6,

    /// <summary>
    /// Purchase order has been fully received
    /// </summary>
    Received = 7,

    /// <summary>
    /// Purchase order has been cancelled
    /// </summary>
    Cancelled = 8,

    /// <summary>
    /// Purchase order is on hold
    /// </summary>
    OnHold = 9
}
