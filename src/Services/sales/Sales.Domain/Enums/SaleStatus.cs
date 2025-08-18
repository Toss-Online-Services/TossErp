namespace TossErp.Sales.Domain.Enums;

/// <summary>
/// Status of a sale transaction
/// </summary>
public enum SaleStatus
{
    /// <summary>
    /// Sale is being created/in progress
    /// </summary>
    Draft = 1,

    /// <summary>
    /// Sale is pending (created but not yet completed)
    /// </summary>
    Pending = 2,

    /// <summary>
    /// Sale is completed and paid
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Sale was cancelled before completion
    /// </summary>
    Cancelled = 4,

    /// <summary>
    /// Sale is on hold/suspended
    /// </summary>
    OnHold = 5,

    /// <summary>
    /// Sale is partially paid (layaway)
    /// </summary>
    PartiallyPaid = 6,

    /// <summary>
    /// Sale was refunded
    /// </summary>
    Refunded = 7
}
