using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Common.Models;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Quality.Queries.GetActionItems;

public record GetActionItemsQuery : IRequest<PaginatedList<ActionItemDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public int? IncidentId { get; init; }
    public string? AssignedToId { get; init; }
    public ActionItemStatus? Status { get; init; }
    public bool? Overdue { get; init; }
}

public class GetActionItemsQueryHandler : IRequestHandler<GetActionItemsQuery, PaginatedList<ActionItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetActionItemsQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<PaginatedList<ActionItemDto>> Handle(GetActionItemsQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var query = _context.ActionItems
            .Include(a => a.Incident)
            .Where(a => a.BusinessId == _businessContext.CurrentBusinessId!.Value)
            .AsQueryable();

        if (request.IncidentId.HasValue)
        {
            query = query.Where(a => a.IncidentId == request.IncidentId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.AssignedToId))
        {
            query = query.Where(a => a.AssignedToId == request.AssignedToId);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(a => a.Status == request.Status.Value);
        }

        if (request.Overdue == true)
        {
            var now = DateTimeOffset.UtcNow;
            query = query.Where(a =>
                a.DueDate.HasValue &&
                a.DueDate.Value < now &&
                a.Status != ActionItemStatus.Completed &&
                a.Status != ActionItemStatus.Cancelled);
        }

        query = query.OrderBy(a => a.DueDate ?? DateTimeOffset.MaxValue);

        var actionItemQuery = query
            .Select(a => new ActionItemDto
            {
                Id = a.Id,
                IncidentId = a.IncidentId,
                Title = a.Title,
                Description = a.Description,
                AssignedToId = a.AssignedToId,
                DueDate = a.DueDate,
                Status = a.Status,
                CompletedAt = a.CompletedAt,
                Notes = a.Notes,
                Created = a.Created
            });

        return await PaginatedList<ActionItemDto>.CreateAsync(actionItemQuery, request.PageNumber, request.PageSize, cancellationToken);
    }
}

public record ActionItemDto
{
    public int Id { get; init; }
    public int? IncidentId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? AssignedToId { get; init; }
    public DateTimeOffset? DueDate { get; init; }
    public ActionItemStatus Status { get; init; }
    public DateTimeOffset? CompletedAt { get; init; }
    public string? Notes { get; init; }
    public DateTimeOffset Created { get; init; }
}

