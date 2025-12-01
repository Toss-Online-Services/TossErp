using Toss.Application.Common.Interfaces.Notifications;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Enums;
using MediatR;

namespace Toss.Application.Notifications.EventHandlers;

/// <summary>
/// Creates a notification when a comment is added to an entity
/// </summary>
public class CommentAddedEventHandler : INotificationHandler<CommentAddedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly IBusinessContext _businessContext;

    public CommentAddedEventHandler(
        INotificationService notificationService,
        IBusinessContext businessContext)
    {
        _notificationService = notificationService;
        _businessContext = businessContext;
    }

    public async Task Handle(CommentAddedEvent notification, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness) return;

        // Create notification for relevant users (e.g., entity owner, assigned users)
        // This is a placeholder - in a full implementation, you'd determine who should be notified
        // based on the linked entity type and ID
        
        await _notificationService.CreateNotificationAsync(
            _businessContext.CurrentBusinessId!.Value,
            notification.TargetUserId, // User who should be notified
            "New Comment",
            $"A new comment was added to {notification.LinkedType} #{notification.LinkedId}",
            NotificationType.CommentAdded,
            notification.LinkedType,
            notification.LinkedId,
            $"/{notification.LinkedType.ToLower()}/{notification.LinkedId}",
            cancellationToken);
    }
}

/// <summary>
/// Domain event for when a comment is added
/// </summary>
public record CommentAddedEvent : INotification
{
    public string LinkedType { get; init; } = string.Empty;
    public int LinkedId { get; init; }
    public string TargetUserId { get; init; } = string.Empty; // User who should be notified
    public string CommentAuthorId { get; init; } = string.Empty;
}

