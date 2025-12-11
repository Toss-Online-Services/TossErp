using Toss.Application.Common.Interfaces.Notifications;
using Toss.Application.Support.Commands.ChangeTicketStatus;
using Toss.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Toss.Application.Support.EventHandlers;

/// <summary>
/// Event handler that creates notifications when a ticket status changes
/// </summary>
public class TicketStatusChangedNotificationHandler : INotificationHandler<TicketStatusChangedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<TicketStatusChangedNotificationHandler> _logger;

    public TicketStatusChangedNotificationHandler(
        INotificationService notificationService,
        ILogger<TicketStatusChangedNotificationHandler> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(TicketStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            // Only send notifications for significant status changes
            if (notification.NewStatus == TicketStatus.Closed || 
                notification.NewStatus == TicketStatus.Resolved ||
                notification.NewStatus == TicketStatus.InProgress)
            {
                var statusText = notification.NewStatus switch
                {
                    TicketStatus.Closed => "closed",
                    TicketStatus.Resolved => "resolved",
                    TicketStatus.InProgress => "in progress",
                    _ => notification.NewStatus.ToString().ToLower()
                };

                var title = $"Ticket #{notification.TicketId} {statusText}";
                var message = $"A ticket has been marked as {statusText}.";

                // Create notification for the ticket creator (if we have their ID)
                // Note: In a real implementation, you'd fetch the ticket to get CreatedById
                // For now, we'll create a generic notification that can be enhanced later
                _logger.LogInformation(
                    "Ticket status changed: TicketId={TicketId}, OldStatus={OldStatus}, NewStatus={NewStatus}",
                    notification.TicketId,
                    notification.OldStatus,
                    notification.NewStatus);

                // TODO: Fetch ticket to get CreatedById and AssignedToId, then create notifications
                // For MVP, we're logging the event but not creating notifications yet
                // This can be enhanced when user management is more complete
            }
        }
        catch (Exception ex)
        {
            // Log but don't fail the status change if notification fails
            _logger.LogError(ex,
                "Failed to create notification for ticket status change: TicketId={TicketId}",
                notification.TicketId);
        }

        await Task.CompletedTask;
    }
}

