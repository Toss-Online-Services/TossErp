namespace Toss.Domain.Enums;

/// <summary>
/// Status of a support ticket
/// </summary>
public enum TicketStatus
{
    New = 0,
    Open = 1,
    InProgress = 2,
    Resolved = 3,
    Closed = 4,
    Cancelled = 5
}

