namespace Collaboration.Domain.Enums;

/// <summary>
/// Represents the type of allocation in a group-buy campaign
/// </summary>
public enum AllocationType
{
    /// <summary>
    /// Equal distribution among all participants
    /// </summary>
    Equal = 0,

    /// <summary>
    /// Proportional to committed amount
    /// </summary>
    Proportional = 1,

    /// <summary>
    /// First-come-first-served basis
    /// </summary>
    FirstComeFirstServed = 2,

    /// <summary>
    /// Custom allocation based on specific rules
    /// </summary>
    Custom = 3,

    /// <summary>
    /// Lottery-based allocation
    /// </summary>
    Lottery = 4,

    /// <summary>
    /// Priority-based allocation (e.g., VIP members)
    /// </summary>
    Priority = 5
}
