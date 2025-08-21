using Microsoft.EntityFrameworkCore;
using TossErp.Sales.Domain.Common;
using TossErp.Sales.Domain.Entities;
using TossErp.Sales.Domain.ValueObjects;
using TossErp.Sales.Infrastructure.Data;

namespace TossErp.Sales.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Sale aggregates
/// </summary>
public class SaleRepository : ISaleRepository
{
    private readonly SalesDbContext _context;

    public SaleRepository(SalesDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Sale?> GetByReceiptNumberAsync(ReceiptNumber receiptNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .FirstOrDefaultAsync(s => s.ReceiptNumber.Value == receiptNumber.Value, cancellationToken);
    }

    public async Task<IReadOnlyList<Sale>> GetByTillIdAsync(Guid tillId, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .Where(s => s.TillId == tillId);

        if (fromDate.HasValue)
            query = query.Where(s => EF.Property<DateTime>(s, "CreatedDate") >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(s => EF.Property<DateTime>(s, "CreatedDate") <= toDate.Value);

        return await query
            .OrderByDescending(s => EF.Property<DateTime>(s, "CreatedDate"))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Sale>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .Where(s => s.CustomerId == customerId)
            .OrderByDescending(s => EF.Property<DateTime>(s, "CreatedDate"))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Sale>> GetPendingSalesAsync(Guid? tillId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .Where(s => s.Status == Domain.Enums.SaleStatus.Pending || s.Status == Domain.Enums.SaleStatus.OnHold);

        if (tillId.HasValue)
            query = query.Where(s => s.TillId == tillId.Value);

        return await query
            .OrderBy(s => EF.Property<DateTime>(s, "CreatedDate"))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Sale>> GetSalesForDateRangeAsync(DateTime fromDate, DateTime toDate, Guid? tillId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .Where(s => EF.Property<DateTime>(s, "CreatedDate") >= fromDate && EF.Property<DateTime>(s, "CreatedDate") <= toDate);

        if (tillId.HasValue)
            query = query.Where(s => s.TillId == tillId.Value);

        return await query
            .OrderByDescending(s => EF.Property<DateTime>(s, "CreatedDate"))
            .ToListAsync(cancellationToken);
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
        var sale = await GetByIdAsync(id, cancellationToken);
        if (sale != null)
        {
            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales.AnyAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<bool> ReceiptNumberExistsAsync(ReceiptNumber receiptNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .AnyAsync(s => s.ReceiptNumber.Value == receiptNumber.Value, cancellationToken);
    }

    public async Task<string> GetNextReceiptNumberAsync(CancellationToken cancellationToken = default)
    {
        var lastReceiptNumber = await _context.Sales
            .OrderByDescending(s => EF.Property<DateTime>(s, "CreatedDate"))
            .Select(s => s.ReceiptNumber.Value)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrEmpty(lastReceiptNumber))
        {
            return "RC-00000001";
        }

        // Extract number part and increment
        var numberPart = lastReceiptNumber.Split('-').LastOrDefault();
        if (int.TryParse(numberPart, out var number))
        {
            return $"RC-{(number + 1):D8}";
        }

        return "RC-00000001";
    }

    public async Task<decimal> GetDailySalesAmountAsync(DateTime date, Guid? tillId = null, CancellationToken cancellationToken = default)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        var query = _context.Sales
            .Where(s => s.Status == Domain.Enums.SaleStatus.Completed
                && EF.Property<DateTime>(s, "CreatedDate") >= startOfDay 
                && EF.Property<DateTime>(s, "CreatedDate") < endOfDay);

        if (tillId.HasValue)
            query = query.Where(s => s.TillId == tillId.Value);

        var sales = await query.ToListAsync(cancellationToken);
        return sales.Sum(s => s.Total.Amount);
    }

    public async Task<int> GetDailySalesCountAsync(DateTime date, Guid? tillId = null, CancellationToken cancellationToken = default)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        var query = _context.Sales
            .Where(s => s.Status == Domain.Enums.SaleStatus.Completed
                && EF.Property<DateTime>(s, "CreatedDate") >= startOfDay 
                && EF.Property<DateTime>(s, "CreatedDate") < endOfDay);

        if (tillId.HasValue)
            query = query.Where(s => s.TillId == tillId.Value);

        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Sale>> SearchAsync(string searchTerm, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var query = _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .Where(s => s.ReceiptNumber.Value.Contains(searchTerm) 
                || s.CustomerName.Contains(searchTerm)
                || s.Items.Any(i => i.ItemName.Contains(searchTerm)));

        return await query
            .OrderByDescending(s => EF.Property<DateTime>(s, "CreatedDate"))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public IQueryable<Sale> Query()
    {
        return _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments);
    }
}
