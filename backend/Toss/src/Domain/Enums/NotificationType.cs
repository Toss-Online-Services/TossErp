namespace Toss.Domain.Enums;

/// <summary>
/// Type of notification
/// </summary>
public enum NotificationType
{
    Info = 0,
    Warning = 1,
    Error = 2,
    Success = 3,
    PaymentReceived = 4,
    StockLow = 5,
    OrderReceived = 6,
    DeliveryScheduled = 7,
    TaskAssigned = 8,
    CommentAdded = 9
}

