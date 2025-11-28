namespace Toss.Domain.Enums;

/// <summary>
/// Status of a purchase request in the procurement workflow
/// </summary>
public enum PurchaseRequestStatus
{
    /// <summary>
    /// Request is being drafted and can be modified
    /// </summary>
    Draft = 0,
    
    /// <summary>
    /// Request has been submitted for approval
    /// </summary>
    Submitted = 1,
    
    /// <summary>
    /// Request has been approved and is ready to be converted to a purchase order
    /// </summary>
    Approved = 2,
    
    /// <summary>
    /// Request has been converted to a purchase order
    /// </summary>
    ConvertedToPO = 3,
    
    /// <summary>
    /// Request has been cancelled
    /// </summary>
    Cancelled = 4
}

