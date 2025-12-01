using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Quality.Queries.GetChecklists;

public record GetChecklistsQuery : IRequest<PaginatedList<QualityChecklistDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
}

public class GetChecklistsQueryHandler : IRequestHandler<GetChecklistsQuery, PaginatedList<QualityChecklistDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetChecklistsQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<PaginatedList<QualityChecklistDto>> Handle(GetChecklistsQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var query = _context.QualityChecklists
            .Include(c => c.Items)
            .Where(c => c.BusinessId == _businessContext.CurrentBusinessId!.Value)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(c =>
                c.Name.ToLower().Contains(searchTerm) ||
                (c.Description != null && c.Description.ToLower().Contains(searchTerm)));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(c => c.IsActive == request.IsActive.Value);
        }

        query = query.OrderBy(c => c.Name);

        var checklistQuery = query
            .Select(c => new QualityChecklistDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                IsActive = c.IsActive,
                ItemCount = c.Items.Count,
                Created = c.Created
            });

        return await PaginatedList<QualityChecklistDto>.CreateAsync(checklistQuery, request.PageNumber, request.PageSize, cancellationToken);
    }
}

public record QualityChecklistDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; }
    public int ItemCount { get; init; }
    public DateTimeOffset Created { get; init; }
}

