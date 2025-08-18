namespace TossErp.Procurement.Domain.Enums;

/// <summary>
/// Status of a supplier
/// </summary>
public enum SupplierStatus
{
    /// <summary>
    /// Supplier is active and can be used for purchases
    /// </summary>
    Active = 1,

    /// <summary>
    /// Supplier is inactive and cannot be used for purchases
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Supplier is on hold (temporarily suspended)
    /// </summary>
    OnHold = 3,

    /// <summary>
    /// Supplier has been blacklisted
    /// </summary>
    Blacklisted = 4,

    /// <summary>
    /// Supplier is pending approval
    /// </summary>
    PendingApproval = 5
}
