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
            .Include(p => p.Category)
            .Include(p => p.StockLevels)
            .FirstOrDefaultAsync(p => p.SKU == request.Sku, cancellationToken);

        if (product == null)
            return null;

        // Filter stock levels for the specified shop
        var shopStock = product.StockLevels
            .Where(sl => sl.ShopId == request.ShopId)
            .Sum(sl => sl.AvailableStock);

        return new ProductDetailDto
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            Barcode = product.Barcode,
            Description = product.Description,
            BasePrice = product.BasePrice,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name,
            TotalStock = shopStock,
            MinimumStockLevel = product.MinimumStockLevel,
            IsActive = product.IsActive
        };
    }
}

