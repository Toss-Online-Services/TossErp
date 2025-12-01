using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Notifications;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Infrastructure.Services.Notifications;

public class NotificationService : Toss.Application.Common.Interfaces.Notifications.INotificationService
{
    private readonly IApplicationDbContext _context;

    public NotificationService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateNotificationAsync(
        int businessId,
        string userId,
        string title,
        string message,
        NotificationType type,
        string? linkedType = null,
        int? linkedId = null,
        string? actionUrl = null,
        CancellationToken cancellationToken = default)
    {
        // Check user notification preferences
        var preference = await _context.NotificationPreferences
            .FirstOrDefaultAsync(p => p.BusinessId == businessId
                && p.UserId == userId
                && p.NotificationType == type, cancellationToken);

        // If preference exists and is disabled, don't create notification
        if (preference != null && !preference.IsEnabled)
        {
            return 0; // Notification not created due to preference
        }

        var notification = new Notification
        {
            BusinessId = businessId,
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            Status = NotificationStatus.Unread,
            LinkedType = linkedType,
            LinkedId = linkedId,
            ActionUrl = actionUrl
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(cancellationToken);

        // TODO: Send email/SMS if preferences allow
        // if (preference?.SendEmail == true) { ... }
        // if (preference?.SendSms == true) { ... }

        return notification.Id;
    }

    public async Task<List<int>> CreateNotificationsAsync(
        int businessId,
        List<string> userIds,
        string title,
        string message,
        NotificationType type,
        string? linkedType = null,
        int? linkedId = null,
        string? actionUrl = null,
        CancellationToken cancellationToken = default)
    {
        var notificationIds = new List<int>();

        foreach (var userId in userIds)
        {
            var id = await CreateNotificationAsync(
                businessId,
                userId,
                title,
                message,
                type,
                linkedType,
                linkedId,
                actionUrl,
                cancellationToken);

            if (id > 0)
            {
                notificationIds.Add(id);
            }
        }

        return notificationIds;
    }
}

