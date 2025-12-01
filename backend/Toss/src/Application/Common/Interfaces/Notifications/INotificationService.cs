using Toss.Domain.Enums;

namespace Toss.Application.Common.Interfaces.Notifications;

/// <summary>
/// Service for creating and managing notifications
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Creates a notification for a user
    /// </summary>
    Task<int> CreateNotificationAsync(
        int businessId,
        string userId,
        string title,
        string message,
        NotificationType type,
        string? linkedType = null,
        int? linkedId = null,
        string? actionUrl = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates notifications for multiple users
    /// </summary>
    Task<List<int>> CreateNotificationsAsync(
        int businessId,
        List<string> userIds,
        string title,
        string message,
        NotificationType type,
        string? linkedType = null,
        int? linkedId = null,
        string? actionUrl = null,
        CancellationToken cancellationToken = default);
}

