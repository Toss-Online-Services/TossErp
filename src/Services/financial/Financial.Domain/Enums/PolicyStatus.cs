namespace Financial.Domain.Enums;

/// <summary>
/// Represents the status of an insurance policy
/// </summary>
public enum PolicyStatus
{
    /// <summary>
    /// Policy application is pending
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Policy is active and in good standing
    /// </summary>
    Active = 1,

    /// <summary>
    /// Policy has been suspended
    /// </summary>
    Suspended = 2,

    /// <summary>
    /// Policy has been cancelled
    /// </summary>
    Cancelled = 3,

    /// <summary>
    /// Policy has expired
    /// </summary>
    Expired = 4,

    /// <summary>
    /// Policy is up for renewal
    /// </summary>
    PendingRenewal = 5
}
