using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Catalog;

namespace Toss.Application.Inventory.Queries.GetProductById;

public record ProductDetailDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Sku { get; init; } = string.Empty;
    public string? Barcode { get; init; }
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public int? CategoryId { get; init; }
    public string? CategoryName { get; init; }
    public int StockQuantity { get; init; }
    public int ReorderLevel { get; init; }
    public bool Published { get; init; }
}

public record GetProductByIdQuery : IRequest<ProductDetailDto>
{
    public int Id { get; init; }
}

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDetailDto>
{
    private readonly IApplicationDbContext _context;

    public GetProductByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDetailDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .Include(p => p.ProductCategory)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null)
            throw new NotFoundException(nameof(Product), request.Id.ToString());

        return new ProductDetailDto
        {
            Id = product.Id,
            Name = product.Name,
            Sku = product.Sku,
            Barcode = product.Barcode,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.ProductCategoryId,
            CategoryName = product.ProductCategory?.Name,
            StockQuantity = product.StockQuantity,
            ReorderLevel = product.ReorderLevel,
            Published = product.Published
        };
    }
}

