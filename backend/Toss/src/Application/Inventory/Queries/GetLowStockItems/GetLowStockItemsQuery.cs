using Toss.Application.Common.Interfaces;

namespace Toss.Application.Inventory.Queries.GetLowStockItems;

public record GetLowStockItemsQuery : IRequest<List<LowStockItemDto>>
{
    public int ShopId { get; init; }
    public int Threshold { get; init; } = 10;
}

public record LowStockItemDto
{
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string Sku { get; init; } = string.Empty;
    public int CurrentStock { get; init; }
    public int ReorderPoint { get; init; }
    public decimal LastPurchasePrice { get; init; }
    public string? PreferredVendor { get; init; }
    public int VendorId { get; init; }
}

public class GetLowStockItemsQueryHandler : IRequestHandler<GetLowStockItemsQuery, List<LowStockItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLowStockItemsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<LowStockItemDto>> Handle(GetLowStockItemsQuery request, CancellationToken cancellationToken)
    {
        var lowStockItems = await _context.StockLevels
            .Include(s => s.Product)
            .Where(s =>
                s.ShopId == request.ShopId &&
                s.AvailableStock <= (s.Product != null ? s.Product.MinimumStockLevel : request.Threshold) &&
                s.Product != null &&
                s.Product.IsActive
            )
            .OrderBy(s => s.AvailableStock)
            .Select(s => new LowStockItemDto
            {
                ProductId = s.ProductId,
                ProductName = s.Product!.Name,
                Sku = s.Product.SKU,
                CurrentStock = s.AvailableStock,
                ReorderPoint = s.Product.MinimumStockLevel,
                LastPurchasePrice = s.Product.CostPrice ?? s.Product.BasePrice * 0.7m,
                PreferredVendor = "TBD", // TODO: Add vendor relationship to Product
                VendorId = 0 // TODO: Add vendor relationship to Product
            })
            .Take(50)
            .ToListAsync(cancellationToken);

        return lowStockItems;
    }
}

