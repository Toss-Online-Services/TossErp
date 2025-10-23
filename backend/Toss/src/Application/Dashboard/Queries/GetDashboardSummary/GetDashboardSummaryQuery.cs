using Toss.Application.Common.Interfaces;
using Toss.Domain.Enums;

namespace Toss.Application.Dashboard.Queries.GetDashboardSummary;

public record GetDashboardSummaryQuery : IRequest<DashboardSummaryDto>
{
    public int ShopId { get; init; }
}

public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    private readonly IApplicationDbContext _context;

    public GetDashboardSummaryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var today = DateTimeOffset.UtcNow.Date;
        var startOfDay = today;
        var endOfDay = today.AddDays(1);

        // Today's sales
        var todaySales = await _context.Sales
            .Where(s => s.ShopId == request.ShopId 
                && s.SaleDate >= startOfDay 
                && s.SaleDate < endOfDay
                && s.Status == SaleStatus.Completed)
            .ToListAsync(cancellationToken);

        var todayRevenue = todaySales.Sum(s => s.Total);
        var todayTransactions = todaySales.Count;

        // Low stock alerts
        var lowStockCount = await _context.StockAlerts
            .Where(a => a.ShopId == request.ShopId && !a.IsAcknowledged)
            .CountAsync(cancellationToken);

        // Active group buy pools
        var activePoolsCount = await _context.GroupBuyPools
            .Where(p => p.Status == PoolStatus.Open && p.CloseDate > DateTimeOffset.UtcNow)
            .CountAsync(cancellationToken);

        // Pending purchase orders
        var pendingPOCount = await _context.PurchaseOrders
            .Where(po => po.ShopId == request.ShopId 
                && (po.Status == PurchaseOrderStatus.Pending || po.Status == PurchaseOrderStatus.Approved))
            .CountAsync(cancellationToken);

        // This week's revenue
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
        var weekRevenue = await _context.Sales
            .Where(s => s.ShopId == request.ShopId 
                && s.SaleDate >= startOfWeek
                && s.Status == SaleStatus.Completed)
            .SumAsync(s => s.Total, cancellationToken);

        // Top products today
        var topProducts = await _context.SaleItems
            .Where(si => si.Sale.ShopId == request.ShopId 
                && si.Sale.SaleDate >= startOfDay 
                && si.Sale.SaleDate < endOfDay
                && si.Sale.Status == SaleStatus.Completed)
            .GroupBy(si => new { si.ProductId, si.ProductName })
            .Select(g => new TopProductDto
            {
                ProductName = g.Key.ProductName,
                QuantitySold = g.Sum(si => si.Quantity),
                Revenue = g.Sum(si => si.LineTotal)
            })
            .OrderByDescending(tp => tp.Revenue)
            .Take(5)
            .ToListAsync(cancellationToken);

        return new DashboardSummaryDto
        {
            TodayRevenue = todayRevenue,
            TodayTransactions = todayTransactions,
            WeekRevenue = weekRevenue,
            LowStockAlerts = lowStockCount,
            ActiveGroupBuyPools = activePoolsCount,
            PendingPurchaseOrders = pendingPOCount,
            TopProducts = topProducts
        };
    }
}

public class DashboardSummaryDto
{
    public decimal TodayRevenue { get; init; }
    public int TodayTransactions { get; init; }
    public decimal WeekRevenue { get; init; }
    public int LowStockAlerts { get; init; }
    public int ActiveGroupBuyPools { get; init; }
    public int PendingPurchaseOrders { get; init; }
    public List<TopProductDto> TopProducts { get; init; } = new();
}

public class TopProductDto
{
    public string ProductName { get; init; } = string.Empty;
    public int QuantitySold { get; init; }
    public decimal Revenue { get; init; }
}

