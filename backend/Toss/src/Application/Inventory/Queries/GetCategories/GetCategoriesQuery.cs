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
    public int ShopId { get; init; }
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
        var categories = await _context.ProductCategories
            .Where(c => c.ShopId == request.ShopId)
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

