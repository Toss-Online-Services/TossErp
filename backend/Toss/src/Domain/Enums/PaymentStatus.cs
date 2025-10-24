namespace Toss.Domain.Enums;

public enum PaymentStatus
{
    Pending = 0,
    Authorized = 1,
    Captured = 2,
    Completed = 3, // Alias for Captured, used by handlers
    Failed = 4,
    Refunded = 5,
    Cancelled = 6
}

