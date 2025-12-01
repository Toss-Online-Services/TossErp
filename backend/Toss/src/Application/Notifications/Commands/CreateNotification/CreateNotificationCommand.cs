using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Notifications;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Notifications.Commands.CreateNotification;

public record CreateNotificationCommand : IRequest<int>
{
    public string UserId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public NotificationType Type { get; init; } = NotificationType.Info;
    public string? LinkedType { get; init; }
    public int? LinkedId { get; init; }
    public string? ActionUrl { get; init; }
}

public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public CreateNotificationCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<int> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            throw new ValidationException("User ID is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ValidationException("Notification title is required.");
        }

        // Check user notification preferences
        var preference = await _context.NotificationPreferences
            .FirstOrDefaultAsync(p => p.BusinessId == _businessContext.CurrentBusinessId!.Value
                && p.UserId == request.UserId
                && p.NotificationType == request.Type, cancellationToken);

        // If preference exists and is disabled, don't create notification
        if (preference != null && !preference.IsEnabled)
        {
            throw new ValidationException("This notification type is disabled for this user.");
        }

        var notification = new Notification
        {
            BusinessId = _businessContext.CurrentBusinessId!.Value,
            UserId = request.UserId,
            Title = request.Title,
            Message = request.Message,
            Type = request.Type,
            Status = NotificationStatus.Unread,
            LinkedType = request.LinkedType,
            LinkedId = request.LinkedId,
            ActionUrl = request.ActionUrl
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(cancellationToken);

        return notification.Id;
    }
}

