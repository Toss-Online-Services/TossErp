using TossErp.Shared.SeedWork;
using Notifications.Domain.Entities;

namespace Notifications.Domain.Services;

public interface INotificationChannelProvider
{
    NotificationChannel Channel { get; }
    Task<NotificationResult> SendAsync(Notification notification, CancellationToken cancellationToken = default);
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
}

public record NotificationResult(bool Success, string? Message = null, string? ExternalId = null);
