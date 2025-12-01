using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Quality.Queries.GetChecklistRuns;

public record GetChecklistRunsQuery : IRequest<PaginatedList<ChecklistRunDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public int? ChecklistId { get; init; }
    public DateTimeOffset? FromDate { get; init; }
    public DateTimeOffset? ToDate { get; init; }
}

public class GetChecklistRunsQueryHandler : IRequestHandler<GetChecklistRunsQuery, PaginatedList<ChecklistRunDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetChecklistRunsQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<PaginatedList<ChecklistRunDto>> Handle(GetChecklistRunsQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var query = _context.ChecklistRuns
            .Include(r => r.QualityChecklist)
            .Include(r => r.Results)
                .ThenInclude(ri => ri.ChecklistItem)
            .Where(r => r.BusinessId == _businessContext.CurrentBusinessId!.Value)
            .AsQueryable();

        if (request.ChecklistId.HasValue)
        {
            query = query.Where(r => r.QualityChecklistId == request.ChecklistId.Value);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(r => r.RunDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(r => r.RunDate <= request.ToDate.Value);
        }

        query = query.OrderByDescending(r => r.RunDate);

        var runQuery = query
            .Select(r => new ChecklistRunDto
            {
                Id = r.Id,
                ChecklistId = r.QualityChecklistId,
                ChecklistName = r.QualityChecklist.Name,
                RunDate = r.RunDate,
                RunByUserId = r.RunByUserId,
                Notes = r.Notes,
                TotalItems = r.Results.Count,
                PassedItems = r.Results.Count(ri => ri.Passed),
                FailedItems = r.Results.Count(ri => !ri.Passed),
                Created = r.Created
            });

        return await PaginatedList<ChecklistRunDto>.CreateAsync(runQuery, request.PageNumber, request.PageSize, cancellationToken);
    }
}

public record ChecklistRunDto
{
    public int Id { get; init; }
    public int ChecklistId { get; init; }
    public string ChecklistName { get; init; } = string.Empty;
    public DateTimeOffset RunDate { get; init; }
    public string RunByUserId { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public int TotalItems { get; init; }
    public int PassedItems { get; init; }
    public int FailedItems { get; init; }
    public DateTimeOffset Created { get; init; }
}

