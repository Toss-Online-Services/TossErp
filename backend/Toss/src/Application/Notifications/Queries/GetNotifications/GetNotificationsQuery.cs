using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Notifications;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Notifications.Queries.GetNotifications;

public record GetNotificationsQuery : IRequest<List<NotificationDto>>
{
    public NotificationStatus? Status { get; init; }
    public NotificationType? Type { get; init; }
    public int? Limit { get; init; } = 50;
}

public record NotificationDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public NotificationType Type { get; init; }
    public NotificationStatus Status { get; init; }
    public string? LinkedType { get; init; }
    public int? LinkedId { get; init; }
    public string? ActionUrl { get; init; }
    public DateTimeOffset? ReadAt { get; init; }
    public DateTimeOffset Created { get; init; }
}

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, List<NotificationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;
    private readonly IUser _user;

    public GetNotificationsQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext,
        IUser user)
    {
        _context = context;
        _businessContext = businessContext;
        _user = user;
    }

    public async Task<List<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        if (string.IsNullOrWhiteSpace(_user.Id))
        {
            throw new ForbiddenAccessException("User must be authenticated.");
        }

        var query = _context.Notifications
            .Where(n => n.BusinessId == _businessContext.CurrentBusinessId!.Value
                && n.UserId == _user.Id)
            .AsQueryable();

        if (request.Status.HasValue)
        {
            query = query.Where(n => n.Status == request.Status.Value);
        }

        if (request.Type.HasValue)
        {
            query = query.Where(n => n.Type == request.Type.Value);
        }

        var notifications = await query
            .OrderByDescending(n => n.Created)
            .Take(request.Limit ?? 50)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                Status = n.Status,
                LinkedType = n.LinkedType,
                LinkedId = n.LinkedId,
                ActionUrl = n.ActionUrl,
                ReadAt = n.ReadAt,
                Created = n.Created
            })
            .ToListAsync(cancellationToken);

        return notifications;
    }
}

