using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Common.Models;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Quality.Queries.GetIncidents;

public record GetIncidentsQuery : IRequest<PaginatedList<IncidentDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public IncidentType? Type { get; init; }
    public IncidentSeverity? Severity { get; init; }
    public DateTimeOffset? FromDate { get; init; }
    public DateTimeOffset? ToDate { get; init; }
}

public class GetIncidentsQueryHandler : IRequestHandler<GetIncidentsQuery, PaginatedList<IncidentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetIncidentsQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<PaginatedList<IncidentDto>> Handle(GetIncidentsQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var query = _context.Incidents
            .Include(i => i.QualityChecklist)
            .Include(i => i.ChecklistItem)
            .Include(i => i.ActionItems)
            .Where(i => i.BusinessId == _businessContext.CurrentBusinessId!.Value)
            .AsQueryable();

        if (request.Type.HasValue)
        {
            query = query.Where(i => i.Type == request.Type.Value);
        }

        if (request.Severity.HasValue)
        {
            query = query.Where(i => i.Severity == request.Severity.Value);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(i => i.OccurredAt >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(i => i.OccurredAt <= request.ToDate.Value);
        }

        query = query.OrderByDescending(i => i.OccurredAt);

        var incidentQuery = query
            .Select(i => new IncidentDto
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                Type = i.Type,
                Severity = i.Severity,
                OccurredAt = i.OccurredAt,
                QualityChecklistId = i.QualityChecklistId,
                ChecklistItemId = i.ChecklistItemId,
                Notes = i.Notes,
                ActionItemCount = i.ActionItems.Count,
                OpenActionItemCount = i.ActionItems.Count(a => a.Status != ActionItemStatus.Completed && a.Status != ActionItemStatus.Cancelled),
                Created = i.Created
            });

        return await PaginatedList<IncidentDto>.CreateAsync(incidentQuery, request.PageNumber, request.PageSize, cancellationToken);
    }
}

public record IncidentDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public IncidentType Type { get; init; }
    public IncidentSeverity Severity { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
    public int? QualityChecklistId { get; init; }
    public int? ChecklistItemId { get; init; }
    public string? Notes { get; init; }
    public int ActionItemCount { get; init; }
    public int OpenActionItemCount { get; init; }
    public DateTimeOffset Created { get; init; }
}

