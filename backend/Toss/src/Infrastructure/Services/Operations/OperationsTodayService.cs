using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Toss.Application.Common.Interfaces;
using Toss.Application.Operations.Queries.GetTodayView;
using Toss.Domain.Enums;

namespace Toss.Infrastructure.Services.Operations;

public sealed class OperationsTodayService : IOperationsTodayService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(45);
    private readonly IApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<OperationsTodayService> _logger;

    public OperationsTodayService(
        IApplicationDbContext context,
        IMemoryCache cache,
        TimeProvider timeProvider,
        ILogger<OperationsTodayService> logger)
    {
        _context = context;
        _cache = cache;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public Task<OperationsTodayDto> GetTodayAsync(int businessId, CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
        var cacheKey = $"ops:today:{businessId}:{today:yyyyMMdd}";

        return _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            return await BuildTodaySnapshotAsync(businessId, today, cancellationToken);
        })!;
    }

    private async Task<OperationsTodayDto> BuildTodaySnapshotAsync(
        int businessId,
        DateOnly today,
        CancellationToken cancellationToken)
    {
        var startOfDay = new DateTimeOffset(today.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc), TimeSpan.Zero);
        var endOfDay = startOfDay.AddDays(1);

        var storeIds = await _context.Stores
            .AsNoTracking()
            .Where(store => store.BusinessId == businessId && store.IsActive)
            .Select(store => store.Id)
            .ToListAsync(cancellationToken);

        if (storeIds.Count == 0)
        {
            _logger.LogDebug("No stores found for business {BusinessId}. Returning empty today view.", businessId);
            return OperationsTodayDto.Empty(today);
        }

        var totals = await GetTodayTotalsAsync(storeIds, startOfDay, endOfDay, cancellationToken);
        var cash = await GetCashSummaryAsync(storeIds, startOfDay, endOfDay, cancellationToken);
        var lowStock = await GetLowStockAsync(storeIds, cancellationToken);
        var purchaseOrders = await GetPendingPurchaseOrdersAsync(storeIds, cancellationToken);
        var customerOrders = await GetPendingCustomerOrdersAsync(storeIds, cancellationToken);
        var alerts = BuildAlerts(lowStock, purchaseOrders);

        return new OperationsTodayDto(
            today,
            totals,
            cash,
            lowStock,
            purchaseOrders,
            customerOrders,
            alerts);
    }

    private async Task<TodayTotalsDto> GetTodayTotalsAsync(
        IReadOnlyCollection<int> storeIds,
        DateTimeOffset startOfDay,
        DateTimeOffset endOfDay,
        CancellationToken cancellationToken)
    {
        var summary = await _context.Sales
            .AsNoTracking()
            .Where(sale =>
                storeIds.Contains(sale.ShopId) &&
                sale.Status == SaleStatus.Completed &&
                sale.SaleDate >= startOfDay &&
                sale.SaleDate < endOfDay)
            .GroupBy(_ => 1)
            .Select(group => new
            {
                Total = group.Sum(sale => sale.Total),
                Count = group.Count()
            })
            .FirstOrDefaultAsync(cancellationToken);

        var total = summary?.Total ?? 0m;
        var count = summary?.Count ?? 0;
        var average = count > 0
            ? Math.Round(total / count, 2, MidpointRounding.AwayFromZero)
            : 0m;

        return new TodayTotalsDto(total, count, average);
    }

    private async Task<TodayCashDto> GetCashSummaryAsync(
        IReadOnlyCollection<int> storeIds,
        DateTimeOffset startOfDay,
        DateTimeOffset endOfDay,
        CancellationToken cancellationToken)
    {
        var payments = await _context.Payments
            .AsNoTracking()
            .Where(payment =>
                storeIds.Contains(payment.ShopId) &&
                payment.PaymentDate >= startOfDay &&
                payment.PaymentDate < endOfDay &&
                (payment.Status == PaymentStatus.Captured || payment.Status == PaymentStatus.Completed))
            .Select(payment => new
            {
                payment.Amount,
                payment.SourceType
            })
            .ToListAsync(cancellationToken);

        decimal cashIn = 0m;
        decimal cashOut = 0m;

        foreach (var payment in payments)
        {
            if (IsOutflow(payment.SourceType, payment.Amount))
            {
                cashOut += Math.Abs(payment.Amount);
            }
            else
            {
                cashIn += payment.Amount;
            }
        }

        return new TodayCashDto(cashIn, cashOut);
    }

    private static bool IsOutflow(string? sourceType, decimal amount)
    {
        if (!string.IsNullOrWhiteSpace(sourceType) &&
            sourceType.Equals("PurchaseOrder", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return amount < 0;
    }

    private async Task<IReadOnlyList<TodayLowStockItemDto>> GetLowStockAsync(
        IReadOnlyCollection<int> storeIds,
        CancellationToken cancellationToken)
    {
        return await _context.StockAlerts
            .AsNoTracking()
            .Where(alert =>
                storeIds.Contains(alert.ShopId) &&
                !alert.IsAcknowledged)
            .OrderBy(alert => alert.CurrentStock)
            .ThenBy(alert => alert.Created)
            .Take(5)
            .Select(alert => new TodayLowStockItemDto(
                alert.ProductId,
                alert.Product.Name,
                alert.Product.SKU,
                alert.CurrentStock,
                alert.MinimumStock,
                alert.Product.ReorderQuantity))
            .ToListAsync(cancellationToken);
    }

    private async Task<IReadOnlyList<TodayPurchaseOrderDto>> GetPendingPurchaseOrdersAsync(
        IReadOnlyCollection<int> storeIds,
        CancellationToken cancellationToken)
    {
        var pendingStatuses = new[]
        {
            PurchaseOrderStatus.Pending,
            PurchaseOrderStatus.Approved,
            PurchaseOrderStatus.Confirmed,
            PurchaseOrderStatus.PartiallyReceived
        };

        return await _context.PurchaseOrders
            .AsNoTracking()
            .Where(po =>
                storeIds.Contains(po.ShopId) &&
                pendingStatuses.Contains(po.Status))
            .OrderBy(po => po.ExpectedDeliveryDate ?? po.OrderDate)
            .Take(5)
            .Select(po => new TodayPurchaseOrderDto(
                po.Id,
                po.PONumber,
                po.Vendor.Name,
                po.Status.ToString(),
                po.Created,
                po.ExpectedDeliveryDate))
            .ToListAsync(cancellationToken);
    }

    private async Task<IReadOnlyList<TodayCustomerOrderDto>> GetPendingCustomerOrdersAsync(
        IReadOnlyCollection<int> storeIds,
        CancellationToken cancellationToken)
    {
        var pendingOrderStatuses = new[]
        {
            OrderStatus.Pending,
            OrderStatus.Processing
        };

        return await _context.Orders
            .AsNoTracking()
            .Where(order =>
                !order.Deleted &&
                pendingOrderStatuses.Contains(order.OrderStatus))
            .Join(
                _context.Customers.AsNoTracking(),
                order => order.CustomerId,
                customer => customer.Id,
                (order, customer) => new { order, customer })
            .Where(joined => joined.customer.StoreId.HasValue && storeIds.Contains(joined.customer.StoreId.Value))
            .OrderByDescending(joined => joined.order.Created)
            .Take(5)
            .Select(joined => new TodayCustomerOrderDto(
                joined.order.Id,
                $"ORD-{joined.order.Id:D6}",
                string.IsNullOrWhiteSpace(joined.customer.FullName)
                    ? joined.customer.Email ?? "Customer"
                    : joined.customer.FullName,
                joined.order.OrderStatus.ToString(),
                joined.order.Created))
            .ToListAsync(cancellationToken);
    }

    private static IReadOnlyList<TodayAlertDto> BuildAlerts(
        IReadOnlyList<TodayLowStockItemDto> lowStock,
        IReadOnlyList<TodayPurchaseOrderDto> purchaseOrders)
    {
        var alerts = new List<TodayAlertDto>();

        alerts.AddRange(lowStock.Select(item =>
            new TodayAlertDto(
                "low-stock",
                $"{item.ProductName} only has {item.CurrentQuantity} left (min {item.MinimumQuantity}).")));

        alerts.AddRange(purchaseOrders
            .Where(po => po.ExpectedAt.HasValue && po.ExpectedAt.Value < DateTimeOffset.UtcNow.AddDays(1))
            .Select(po => new TodayAlertDto(
                "delivery",
                $"PO {po.PurchaseOrderNumber} from {po.SupplierName} expected by {po.ExpectedAt:MMM d HH:mm}.")));

        return alerts.Take(5).ToList();
    }
}

