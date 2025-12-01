using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Quality.Queries.GetChecklistById;

public record GetChecklistByIdQuery : IRequest<QualityChecklistDetailDto?>
{
    public int Id { get; init; }
}

public class GetChecklistByIdQueryHandler : IRequestHandler<GetChecklistByIdQuery, QualityChecklistDetailDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetChecklistByIdQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<QualityChecklistDetailDto?> Handle(GetChecklistByIdQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var checklist = await _context.QualityChecklists
            .Include(c => c.Items.OrderBy(i => i.Order))
            .FirstOrDefaultAsync(c => c.Id == request.Id
                && c.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (checklist == null)
        {
            return null;
        }

        return new QualityChecklistDetailDto
        {
            Id = checklist.Id,
            Name = checklist.Name,
            Description = checklist.Description,
            IsActive = checklist.IsActive,
            Items = checklist.Items.Select(i => new ChecklistItemDto
            {
                Id = i.Id,
                Title = i.Title,
                IsRequired = i.IsRequired,
                Order = i.Order
            }).ToList(),
            Created = checklist.Created
        };
    }
}

public record QualityChecklistDetailDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; }
    public List<ChecklistItemDto> Items { get; init; } = new();
    public DateTimeOffset Created { get; init; }
}

public record ChecklistItemDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public bool IsRequired { get; init; }
    public int Order { get; init; }
}

