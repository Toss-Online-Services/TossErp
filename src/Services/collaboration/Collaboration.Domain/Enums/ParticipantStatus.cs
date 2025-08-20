namespace Collaboration.Domain.Enums;

/// <summary>
/// Represents the status of a participant in a group-buy campaign
/// </summary>
public enum ParticipantStatus
{
    /// <summary>
    /// Participant has joined the campaign
    /// </summary>
    Joined = 0,

    /// <summary>
    /// Participant has left the campaign
    /// </summary>
    Left = 1,

    /// <summary>
    /// Participant has confirmed payment
    /// </summary>
    PaymentConfirmed = 2,

    /// <summary>
    /// Participant has been removed from the campaign
    /// </summary>
    Removed = 3,

    /// <summary>
    /// Participant is on hold (e.g., payment issues)
    /// </summary>
    OnHold = 4
}
