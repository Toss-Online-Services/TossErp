using Microsoft.EntityFrameworkCore;
using TossErp.Sales.Domain.Common;
using TossErp.Sales.Domain.Entities;
using TossErp.Sales.Infrastructure.Persistence;

namespace TossErp.Sales.Infrastructure.Persistence.Repositories;

/// <summary>
/// Entity Framework implementation of ISaleRepository
/// </summary>
public class SaleRepository : ISaleRepository
{
    private readonly SalesDbContext _context;

    public SaleRepository(SalesDbContext context)
    {
        _context = context;
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Sale>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sale>> GetByTillAsync(Guid tillId, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .Where(s => s.TillId == tillId)
            .ToListAsync(cancellationToken);
    }

    public async Task<DailySalesSummaryDto> GetDailySalesSummaryAsync(DateTime date, Guid? tillId = null, CancellationToken cancellationToken = default)
    {
        var startDate = date.Date;
        var endDate = startDate.AddDays(1);

        var query = _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .Where(s => s.CreatedAt >= startDate && s.CreatedAt < endDate && s.Status == Domain.Enums.SaleStatus.Completed);

        if (tillId.HasValue)
        {
            query = query.Where(s => s.TillId == tillId.Value);
        }

        var sales = await query.ToListAsync(cancellationToken);

        var summary = new DailySalesSummaryDto
        {
            Date = date,
            TotalSales = sales.Count,
            TotalRevenue = sales.Sum(s => s.TotalAmount.Amount),
            TotalItems = sales.Sum(s => s.Items.Count),
            AverageTransactionValue = sales.Any() ? sales.Average(s => s.TotalAmount.Amount) : 0,
            TopSellingItems = GetTopSellingItems(sales),
            SalesByHour = GetSalesByHour(sales, date),
            PaymentMethodBreakdown = GetPaymentMethodBreakdown(sales)
        };

        return summary;
    }

    public async Task<Sale> AddAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Sales.Add(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Sales.Update(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await _context.Sales.FindAsync(new object[] { id }, cancellationToken);
        if (sale != null)
        {
            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    private List<TopSellingItemDto> GetTopSellingItems(IEnumerable<Sale> sales)
    {
        var itemGroups = sales
            .SelectMany(s => s.Items)
            .GroupBy(item => new { item.ItemId, item.ItemName, item.ItemSku })
            .Select(g => new TopSellingItemDto
            {
                ItemId = g.Key.ItemId,
                ItemName = g.Key.ItemName,
                ItemSku = g.Key.ItemSku,
                QuantitySold = g.Sum(item => item.Quantity),
                TotalRevenue = g.Sum(item => item.TotalAmount.Amount)
            })
            .OrderByDescending(item => item.QuantitySold)
            .Take(10)
            .ToList();

        return itemGroups;
    }

    private List<SalesByHourDto> GetSalesByHour(IEnumerable<Sale> sales, DateTime date)
    {
        var salesByHour = new List<SalesByHourDto>();

        for (int hour = 0; hour < 24; hour++)
        {
            var hourStart = date.AddHours(hour);
            var hourEnd = hourStart.AddHours(1);

            var hourSales = sales.Where(s => s.CreatedAt >= hourStart && s.CreatedAt < hourEnd);

            salesByHour.Add(new SalesByHourDto
            {
                Hour = hour,
                SalesCount = hourSales.Count(),
                Revenue = hourSales.Sum(s => s.TotalAmount.Amount)
            });
        }

        return salesByHour;
    }

    private List<PaymentMethodBreakdownDto> GetPaymentMethodBreakdown(IEnumerable<Sale> sales)
    {
        var paymentGroups = sales
            .SelectMany(s => s.Payments)
            .GroupBy(p => p.Method)
            .Select(g => new PaymentMethodBreakdownDto
            {
                Method = g.Key,
                Count = g.Count(),
                TotalAmount = g.Sum(p => p.Amount.Amount)
            })
            .OrderByDescending(p => p.TotalAmount)
            .ToList();

        return paymentGroups;
    }
}
