using Toss.Application.Common.Interfaces;
using Toss.Application.Inventory.Queries.GetProductById;

namespace Toss.Application.Inventory.Queries.GetProductByBarcode;

public record GetProductByBarcodeQuery : IRequest<ProductDetailDto?>
{
    public string Barcode { get; init; } = string.Empty;
    public int ShopId { get; init; }
}

public class GetProductByBarcodeQueryHandler : IRequestHandler<GetProductByBarcodeQuery, ProductDetailDto?>
{
    private readonly IApplicationDbContext _context;

    public GetProductByBarcodeQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDetailDto?> Handle(GetProductByBarcodeQuery request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.StockLevels)
            .FirstOrDefaultAsync(p => p.Barcode == request.Barcode, cancellationToken);

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

