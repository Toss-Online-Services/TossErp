using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Notifications;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Notifications.Commands.UpdateNotificationPreference;

public record UpdateNotificationPreferenceCommand : IRequest<bool>
{
    public NotificationType NotificationType { get; init; }
    public bool IsEnabled { get; init; } = true;
    public bool SendEmail { get; init; } = false;
    public bool SendSms { get; init; } = false;
}

public class UpdateNotificationPreferenceCommandHandler : IRequestHandler<UpdateNotificationPreferenceCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;
    private readonly IUser _user;

    public UpdateNotificationPreferenceCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext,
        IUser user)
    {
        _context = context;
        _businessContext = businessContext;
        _user = user;
    }

    public async Task<bool> Handle(UpdateNotificationPreferenceCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        if (string.IsNullOrWhiteSpace(_user.Id))
        {
            throw new ForbiddenAccessException("User must be authenticated.");
        }

        var preference = await _context.NotificationPreferences
            .FirstOrDefaultAsync(p => p.BusinessId == _businessContext.CurrentBusinessId!.Value
                && p.UserId == _user.Id
                && p.NotificationType == request.NotificationType, cancellationToken);

        if (preference == null)
        {
            // Create new preference
            preference = new NotificationPreference
            {
                BusinessId = _businessContext.CurrentBusinessId!.Value,
                UserId = _user.Id,
                NotificationType = request.NotificationType,
                IsEnabled = request.IsEnabled,
                SendEmail = request.SendEmail,
                SendSms = request.SendSms
            };
            _context.NotificationPreferences.Add(preference);
        }
        else
        {
            // Update existing preference
            preference.IsEnabled = request.IsEnabled;
            preference.SendEmail = request.SendEmail;
            preference.SendSms = request.SendSms;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

