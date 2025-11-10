namespace Toss.Domain.Enums;

/// <summary>
/// Represents the type of sale transaction
/// </summary>
public enum SaleType
{
    /// <summary>
    /// Immediate POS sale (walk-in, pay and go)
    /// </summary>
    POS = 0,
    
    /// <summary>
    /// Queue-based order requiring preparation (kota, chisa nyama)
    /// </summary>
    QueueOrder = 1,
    
    /// <summary>
    /// Delivery order
    /// </summary>
    Delivery = 2,
    
    /// <summary>
    /// Pre-order for future pickup
    /// </summary>
    PreOrder = 3
}
