using Toss.Application.Common.Interfaces;

namespace Toss.Application.Inventory.Queries.GetStockLevels;

public record GetStockLevelsQuery : IRequest<List<StockLevelDto>>
{
    public int ShopId { get; init; }
    public bool OnlyLowStock { get; init; }
}

public class GetStockLevelsQueryHandler : IRequestHandler<GetStockLevelsQuery, List<StockLevelDto>>
{
    private readonly IApplicationDbContext _context;

    public GetStockLevelsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<StockLevelDto>> Handle(GetStockLevelsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.StockLevels
            .Include(sl => sl.Product)
            .Where(sl => sl.ShopId == request.ShopId);

        if (request.OnlyLowStock)
        {
            query = query.Where(sl => sl.CurrentStock <= sl.Product.MinimumStockLevel);
        }

        var stockLevels = await query
            .OrderBy(sl => sl.Product.Name)
            .ToListAsync(cancellationToken);

        return stockLevels.Select(sl => new StockLevelDto
        {
            ProductId = sl.ProductId,
            ProductName = sl.Product.Name,
            ProductSKU = sl.Product.SKU,
            CurrentStock = sl.CurrentStock,
            ReservedStock = sl.ReservedStock,
            AvailableStock = sl.AvailableStock,
            MinimumStock = sl.Product.MinimumStockLevel,
            ReorderQuantity = sl.Product.ReorderQuantity,
            AverageCost = sl.AverageCost,
            IsLowStock = sl.CurrentStock <= sl.Product.MinimumStockLevel
        }).ToList();
    }
}

public class StockLevelDto
{
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string ProductSKU { get; init; } = string.Empty;
    public int CurrentStock { get; init; }
    public int ReservedStock { get; init; }
    public int AvailableStock { get; init; }
    public int MinimumStock { get; init; }
    public int? ReorderQuantity { get; init; }
    public decimal AverageCost { get; init; }
    public bool IsLowStock { get; init; }
}

