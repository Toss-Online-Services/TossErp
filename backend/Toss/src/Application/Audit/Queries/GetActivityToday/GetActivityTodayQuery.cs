using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Audit;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Audit.Queries.GetActivityToday;

public record GetActivityTodayQuery : IRequest<List<ActivityEntryDto>>
{
    public DateTimeOffset? Date { get; init; } // Optional: defaults to today
}

public record ActivityEntryDto
{
    public int Id { get; init; }
    public string EntityType { get; init; } = string.Empty;
    public int EntityId { get; init; }
    public string Action { get; init; } = string.Empty;
    public string? UserId { get; init; }
    public string? UserName { get; init; }
    public string? Changes { get; init; }
    public string? Notes { get; init; }
    public DateTimeOffset Created { get; init; }
}

public class GetActivityTodayQueryHandler : IRequestHandler<GetActivityTodayQuery, List<ActivityEntryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetActivityTodayQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<List<ActivityEntryDto>> Handle(GetActivityTodayQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var targetDate = request.Date ?? DateTimeOffset.UtcNow;
        var startOfDay = new DateTimeOffset(targetDate.Year, targetDate.Month, targetDate.Day, 0, 0, 0, targetDate.Offset);
        var endOfDay = startOfDay.AddDays(1);

        var activities = await _context.AuditEntries
            .Where(a => a.BusinessId == _businessContext.CurrentBusinessId!.Value
                && a.Created >= startOfDay
                && a.Created < endOfDay)
            .OrderByDescending(a => a.Created)
            .Select(a => new ActivityEntryDto
            {
                Id = a.Id,
                EntityType = a.EntityType,
                EntityId = a.EntityId,
                Action = a.Action,
                UserId = a.UserId,
                UserName = a.UserName,
                Changes = a.Changes,
                Notes = a.Notes,
                Created = a.Created
            })
            .ToListAsync(cancellationToken);

        return activities;
    }
}

