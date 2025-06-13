using TossErp.POS.Domain.AggregatesModel.SaleAggregate;
using TossErp.POS.Domain.Common;
using TossErp.POS.Domain.Repositories;
using TossErp.POS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TossErp.POS.Infrastructure.Repositories;

public class SaleAnalyticsRepository : ISaleAnalyticsRepository
{
    private readonly POSContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public SaleAnalyticsRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task RecordSaleCompleted(string storeId, string staffId, decimal total, bool isOffline, DateTime completedAt, CancellationToken cancellationToken = default)
    {
        var sale = new Sale
        {
            StoreId = storeId,
            StaffId = staffId,
            Total = total,
            IsOffline = isOffline,
            Status = SaleStatus.Completed,
            CompletedAt = completedAt
        };

        await _context.Sales.AddAsync(sale, cancellationToken);
    }

    public async Task RecordSaleRefunded(string storeId, string staffId, decimal amount, string reason, DateTime refundedAt, CancellationToken cancellationToken = default)
    {
        var sale = new Sale
        {
            StoreId = storeId,
            StaffId = staffId,
            Total = -amount,
            Status = SaleStatus.Refunded,
            RefundReason = reason,
            RefundedAt = refundedAt
        };

        await _context.Sales.AddAsync(sale, cancellationToken);
    }

    public async Task RecordPayment(string storeId, string staffId, PaymentMethod method, decimal amount, DateTime paymentDate, CancellationToken cancellationToken = default)
    {
        var sale = await _context.Sales
            .FirstOrDefaultAsync(s => s.StoreId == storeId && s.StaffId == staffId && s.CreatedAt.Date == paymentDate.Date, cancellationToken);

        if (sale != null)
        {
            sale.AddPayment(method, amount);
        }
    }

    public async Task RecordDiscount(string storeId, string staffId, DiscountType type, decimal amount, DateTime createdAt, CancellationToken cancellationToken = default)
    {
        var sale = await _context.Sales
            .FirstOrDefaultAsync(s => s.StoreId == storeId && s.StaffId == staffId && s.CreatedAt.Date == createdAt.Date, cancellationToken);

        if (sale != null)
        {
            sale.ApplyDiscount(type, amount);
        }
    }

    public async Task RecordSaleSynced(string storeId, string staffId, DateTime syncedAt, CancellationToken cancellationToken = default)
    {
        var sale = await _context.Sales
            .FirstOrDefaultAsync(s => s.StoreId == storeId && s.StaffId == staffId && !s.IsSynced, cancellationToken);

        if (sale != null)
        {
            sale.MarkAsSynced(syncedAt);
        }
    }

    public async Task<decimal> GetTotalSalesAsync(string storeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Where(s => s.StoreId == storeId && 
                       s.Status == SaleStatus.Completed && 
                       s.CreatedAt >= startDate && 
                       s.CreatedAt <= endDate)
            .SumAsync(s => s.Total, cancellationToken);
    }

    public async Task<decimal> GetTotalRefundsAsync(string storeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Where(s => s.StoreId == storeId && 
                       s.Status == SaleStatus.Refunded && 
                       s.CreatedAt >= startDate && 
                       s.CreatedAt <= endDate)
            .SumAsync(s => Math.Abs(s.Total), cancellationToken);
    }

    public async Task<IDictionary<PaymentMethod, decimal>> GetPaymentMethodTotalsAsync(string storeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var sales = await _context.Sales
            .Include(s => s.Payments)
            .Where(s => s.StoreId == storeId && 
                       s.Status == SaleStatus.Completed && 
                       s.CreatedAt >= startDate && 
                       s.CreatedAt <= endDate)
            .ToListAsync(cancellationToken);

        return sales
            .SelectMany(s => s.Payments)
            .GroupBy(p => p.Method)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(p => p.Amount)
            );
    }

    public async Task<IDictionary<string, decimal>> GetStaffPerformanceAsync(string storeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var sales = await _context.Sales
            .Where(s => s.StoreId == storeId && 
                       s.Status == SaleStatus.Completed && 
                       s.CreatedAt >= startDate && 
                       s.CreatedAt <= endDate)
            .ToListAsync(cancellationToken);

        return sales
            .GroupBy(s => s.StaffId)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(s => s.Total)
            );
    }

    public async Task<IDictionary<string, int>> GetProductSalesCountAsync(string storeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var sales = await _context.Sales
            .Include(s => s.Items)
            .Where(s => s.StoreId == storeId && 
                       s.Status == SaleStatus.Completed && 
                       s.CreatedAt >= startDate && 
                       s.CreatedAt <= endDate)
            .ToListAsync(cancellationToken);

        return sales
            .SelectMany(s => s.Items)
            .GroupBy(i => i.ProductId)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(i => i.Quantity)
            );
    }

    public async Task<IDictionary<string, decimal>> GetProductSalesRevenueAsync(string storeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var sales = await _context.Sales
            .Include(s => s.Items)
            .Where(s => s.StoreId == storeId && 
                       s.Status == SaleStatus.Completed && 
                       s.CreatedAt >= startDate && 
                       s.CreatedAt <= endDate)
            .ToListAsync(cancellationToken);

        return sales
            .SelectMany(s => s.Items)
            .GroupBy(i => i.ProductId)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(i => i.Total)
            );
    }
} 
