namespace TossErp.Procurement.Domain.Enums;

/// <summary>
/// Payment terms for purchase orders
/// </summary>
public enum PaymentTerms
{
    /// <summary>
    /// Payment due immediately
    /// </summary>
    Immediate = 1,

    /// <summary>
    /// Payment due in 7 days
    /// </summary>
    Net7 = 2,

    /// <summary>
    /// Payment due in 15 days
    /// </summary>
    Net15 = 3,

    /// <summary>
    /// Payment due in 30 days
    /// </summary>
    Net30 = 4,

    /// <summary>
    /// Payment due in 45 days
    /// </summary>
    Net45 = 5,

    /// <summary>
    /// Payment due in 60 days
    /// </summary>
    Net60 = 6,

    /// <summary>
    /// Payment due at end of month
    /// </summary>
    EndOfMonth = 7
}
