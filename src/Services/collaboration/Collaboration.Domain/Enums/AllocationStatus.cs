namespace Collaboration.Domain.Enums;

/// <summary>
/// Represents the status of an allocation in a group-buy campaign
/// </summary>
public enum AllocationStatus
{
    /// <summary>
    /// Allocation has been created but not yet delivered
    /// </summary>
    Allocated = 0,

    /// <summary>
    /// Allocation has been delivered to the participant
    /// </summary>
    Delivered = 1,

    /// <summary>
    /// Allocation has been fully settled
    /// </summary>
    Settled = 2,

    /// <summary>
    /// Allocation has been cancelled
    /// </summary>
    Cancelled = 3,

    /// <summary>
    /// Allocation is on hold (e.g., delivery issues)
    /// </summary>
    OnHold = 4
}
