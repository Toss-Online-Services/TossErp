using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Audit;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Audit.Queries.GetEntityTimeline;

public record GetEntityTimelineQuery : IRequest<List<ActivityEntryDto>>
{
    public string EntityType { get; init; } = string.Empty;
    public int EntityId { get; init; }
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

public class GetEntityTimelineQueryHandler : IRequestHandler<GetEntityTimelineQuery, List<ActivityEntryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetEntityTimelineQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<List<ActivityEntryDto>> Handle(GetEntityTimelineQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        if (string.IsNullOrWhiteSpace(request.EntityType))
        {
            throw new ValidationException("Entity type is required.");
        }

        var activities = await _context.AuditEntries
            .Where(a => a.BusinessId == _businessContext.CurrentBusinessId!.Value
                && a.EntityType == request.EntityType
                && a.EntityId == request.EntityId)
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

