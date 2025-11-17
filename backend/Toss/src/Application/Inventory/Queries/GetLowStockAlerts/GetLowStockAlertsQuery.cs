using Toss.Application.Common.Interfaces;

namespace Toss.Application.Inventory.Queries.GetLowStockAlerts;

public record GetLowStockAlertsQuery : IRequest<List<StockAlertDto>>
{
    public int ShopId { get; init; }
    public bool OnlyUnacknowledged { get; init; } = true;
}

public class GetLowStockAlertsQueryHandler : IRequestHandler<GetLowStockAlertsQuery, List<StockAlertDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLowStockAlertsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<StockAlertDto>> Handle(GetLowStockAlertsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.StockAlerts
            .Include(a => a.Product)
            .Where(a => a.ShopId == request.ShopId);

        if (request.OnlyUnacknowledged)
            query = query.Where(a => !a.IsAcknowledged);

        var alerts = await query
            .OrderByDescending(a => a.Created)
            .ToListAsync(cancellationToken);

        return alerts.Select(a => new StockAlertDto
        {
            Id = a.Id,
            ProductId = a.ProductId,
            ProductName = a.Product.Name,
            ProductSKU = a.Product.SKU,
            CurrentStock = a.CurrentStock,
            MinimumStock = a.MinimumStock,
            ReorderQuantity = a.Product.ReorderQuantity,
            CreatedDate = a.Created
        }).ToList();
    }
}

public class StockAlertDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string ProductSKU { get; init; } = string.Empty;
    public int CurrentStock { get; init; }
    public int MinimumStock { get; init; }
    public int? ReorderQuantity { get; init; }
    public DateTimeOffset CreatedDate { get; init; }
}

