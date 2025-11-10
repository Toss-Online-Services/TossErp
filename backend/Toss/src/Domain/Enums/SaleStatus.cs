namespace Toss.Domain.Enums;

public enum SaleStatus
{
    /// <summary>
    /// Order placed, awaiting preparation (queue entry)
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// Order being prepared/processed (cooking, assembling)
    /// </summary>
    InProgress = 1,
    
    /// <summary>
    /// Order ready for pickup/delivery
    /// </summary>
    Ready = 2,
    
    /// <summary>
    /// Sale completed and paid
    /// </summary>
    Completed = 3,
    
    /// <summary>
    /// Sale voided/cancelled
    /// </summary>
    Voided = 4,
    
    /// <summary>
    /// Sale refunded
    /// </summary>
    Refunded = 5,
    
    /// <summary>
    /// Sale temporarily on hold
    /// </summary>
    OnHold = 6
}

