using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Notifications;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Notifications.Commands.MarkNotificationRead;

public record MarkNotificationReadCommand : IRequest<bool>
{
    public int NotificationId { get; init; }
}

public class MarkNotificationReadCommandHandler : IRequestHandler<MarkNotificationReadCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;
    private readonly IUser _user;

    public MarkNotificationReadCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext,
        IUser user)
    {
        _context = context;
        _businessContext = businessContext;
        _user = user;
    }

    public async Task<bool> Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        if (string.IsNullOrWhiteSpace(_user.Id))
        {
            throw new ForbiddenAccessException("User must be authenticated.");
        }

        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == request.NotificationId
                && n.BusinessId == _businessContext.CurrentBusinessId!.Value
                && n.UserId == _user.Id, cancellationToken);

        if (notification == null)
        {
            return false;
        }

        notification.Status = NotificationStatus.Read;
        notification.ReadAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

