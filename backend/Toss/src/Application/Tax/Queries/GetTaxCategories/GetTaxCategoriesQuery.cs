using Toss.Application.Common.Interfaces;

namespace Toss.Application.Tax.Queries.GetTaxCategories;

public record TaxCategoryDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int DisplayOrder { get; init; }
}

public record GetTaxCategoriesQuery : IRequest<List<TaxCategoryDto>>
{
}

public class GetTaxCategoriesQueryHandler : IRequestHandler<GetTaxCategoriesQuery, List<TaxCategoryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTaxCategoriesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaxCategoryDto>> Handle(GetTaxCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _context.TaxCategories
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .Select(c => new TaxCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                DisplayOrder = c.DisplayOrder
            })
            .ToListAsync(cancellationToken);

        return categories;
    }
}

