using Toss.Application.Common.Interfaces;

namespace Toss.Application.Catalog.Queries.GetProductAttributes;

public record ProductAttributeDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}

public record GetProductAttributesQuery : IRequest<List<ProductAttributeDto>>
{
}

public class GetProductAttributesQueryHandler : IRequestHandler<GetProductAttributesQuery, List<ProductAttributeDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProductAttributesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductAttributeDto>> Handle(GetProductAttributesQuery request, CancellationToken cancellationToken)
    {
        var attributes = await _context.ProductAttributes
            .OrderBy(a => a.Name)
            .Select(a => new ProductAttributeDto
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description
            })
            .ToListAsync(cancellationToken);

        return attributes;
    }
}

