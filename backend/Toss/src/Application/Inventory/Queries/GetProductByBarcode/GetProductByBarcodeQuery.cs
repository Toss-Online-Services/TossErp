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
            .Include(p => p.ProductCategory)
            .FirstOrDefaultAsync(p =>
                p.Barcode == request.Barcode && p.ShopId == request.ShopId,
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

