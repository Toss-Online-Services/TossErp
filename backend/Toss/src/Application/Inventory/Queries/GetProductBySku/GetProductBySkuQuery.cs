using Toss.Application.Common.Interfaces;
using Toss.Application.Inventory.Queries.GetProductById;

namespace Toss.Application.Inventory.Queries.GetProductBySku;

public record GetProductBySkuQuery : IRequest<ProductDetailDto?>
{
    public string Sku { get; init; } = string.Empty;
    public int ShopId { get; init; }
}

public class GetProductBySkuQueryHandler : IRequestHandler<GetProductBySkuQuery, ProductDetailDto?>
{
    private readonly IApplicationDbContext _context;

    public GetProductBySkuQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDetailDto?> Handle(GetProductBySkuQuery request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .Include(p => p.ProductCategory)
            .FirstOrDefaultAsync(p =>
                p.Sku == request.Sku && p.ShopId == request.ShopId,
                cancellationToken);

        if (product == null)
            return null;

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

