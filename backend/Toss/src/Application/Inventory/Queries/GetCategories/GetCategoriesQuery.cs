using Toss.Application.Common.Interfaces;

namespace Toss.Application.Inventory.Queries.GetCategories;

public record CategoryDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int? ParentCategoryId { get; init; }
    public int ProductCount { get; init; }
}

public record GetCategoriesQuery : IRequest<List<CategoryDto>>
{
    public int? ShopId { get; init; }
}

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCategoriesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        // Categories are global, not shop-specific
        // ShopId is optional and can be used for future filtering if needed
        var categories = await _context.ProductCategories
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ParentCategoryId = c.ParentCategoryId,
                ProductCount = c.Products.Count
            })
            .ToListAsync(cancellationToken);

        return categories;
    }
}

