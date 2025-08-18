namespace TossErp.Sales.Domain.Enums;

/// <summary>
/// Status of a POS till/register
/// </summary>
public enum TillStatus
{
    /// <summary>
    /// Till is closed
    /// </summary>
    Closed = 1,

    /// <summary>
    /// Till is open and active
    /// </summary>
    Open = 2,

    /// <summary>
    /// Till is suspended/on break
    /// </summary>
    Suspended = 3,

    /// <summary>
    /// Till is being reconciled
    /// </summary>
    Reconciling = 4,

    /// <summary>
    /// Till has been reconciled and ready to close
    /// </summary>
    Reconciled = 5,

    /// <summary>
    /// Till is out of order/maintenance
    /// </summary>
    OutOfOrder = 6
}
