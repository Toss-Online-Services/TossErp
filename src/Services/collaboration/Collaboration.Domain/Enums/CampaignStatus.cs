namespace Collaboration.Domain.Enums;

/// <summary>
/// Represents the current status of a group-buy campaign
/// </summary>
public enum CampaignStatus
{
    /// <summary>
    /// Campaign is in draft mode and can be edited
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Campaign is active and accepting participants
    /// </summary>
    Active = 1,

    /// <summary>
    /// Campaign is temporarily paused
    /// </summary>
    Paused = 2,

    /// <summary>
    /// Campaign has been completed successfully
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Campaign has been cancelled
    /// </summary>
    Cancelled = 4,

    /// <summary>
    /// Campaign has expired without reaching minimum participants
    /// </summary>
    Expired = 5
}
