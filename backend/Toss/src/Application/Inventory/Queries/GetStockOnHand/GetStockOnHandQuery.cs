using Toss.Application.Common.Interfaces;

namespace Toss.Application.Inventory.Queries.GetStockOnHand;

public record StockOnHandDto
{
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string ProductSKU { get; init; } = string.Empty;
    public int ShopId { get; init; }
    public string ShopName { get; init; } = string.Empty;
    public int CurrentStock { get; init; }
    public int ReservedStock { get; init; }
    public int AvailableStock { get; init; }
    public decimal AverageCost { get; init; }
    public DateTimeOffset LastStockDate { get; init; }
}

public record GetStockOnHandQuery : IRequest<List<StockOnHandDto>>
{
    public int? ProductId { get; init; }
    public int? ShopId { get; init; }
}

public class GetStockOnHandQueryHandler : IRequestHandler<GetStockOnHandQuery, List<StockOnHandDto>>
{
    private readonly IApplicationDbContext _context;

    public GetStockOnHandQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<StockOnHandDto>> Handle(GetStockOnHandQuery request, CancellationToken cancellationToken)
    {
        var query = _context.StockLevels
            .Include(sl => sl.Product)
            .Include(sl => sl.Shop)
            .AsQueryable();

        if (request.ProductId.HasValue)
        {
            query = query.Where(sl => sl.ProductId == request.ProductId.Value);
        }

        if (request.ShopId.HasValue)
        {
            query = query.Where(sl => sl.ShopId == request.ShopId.Value);
        }

        var stockLevels = await query
            .OrderBy(sl => sl.Product.Name)
            .ThenBy(sl => sl.Shop.Name)
            .ToListAsync(cancellationToken);

        return stockLevels.Select(sl => new StockOnHandDto
        {
            ProductId = sl.ProductId,
            ProductName = sl.Product.Name,
            ProductSKU = sl.Product.SKU,
            ShopId = sl.ShopId,
            ShopName = sl.Shop.Name,
            CurrentStock = sl.CurrentStock,
            ReservedStock = sl.ReservedStock,
            AvailableStock = sl.AvailableStock,
            AverageCost = sl.AverageCost,
            LastStockDate = sl.LastStockDate
        }).ToList();
    }
}

