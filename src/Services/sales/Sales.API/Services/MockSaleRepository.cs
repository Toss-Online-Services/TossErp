using TossErp.Sales.Application.Common.Interfaces;
using TossErp.Sales.Domain.Common;
using TossErp.Sales.Domain.Entities;
using TossErp.Sales.Domain.Enums;

namespace TossErp.Sales.API.Services;

/// <summary>
/// Mock implementation of ISaleRepository for MVP
/// </summary>
public class MockSaleRepository : ISaleRepository
{
    private readonly List<Sale> _sales = new();
    private readonly ILogger<MockSaleRepository> _logger;

    public MockSaleRepository(ILogger<MockSaleRepository> logger)
    {
        _logger = logger;
    }

    public Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = _sales.FirstOrDefault(s => s.Id == id);
        return Task.FromResult(sale);
    }

    public Task<IEnumerable<Sale>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_sales.AsEnumerable());
    }

    public Task<Sale> AddAsync(Sale entity, CancellationToken cancellationToken = default)
    {
        _sales.Add(entity);
        _logger.LogInformation("Added sale {SaleId} to mock repository", entity.Id);
        return Task.FromResult(entity);
    }

    public Task<Sale> UpdateAsync(Sale entity, CancellationToken cancellationToken = default)
    {
        var existingSale = _sales.FirstOrDefault(s => s.Id == entity.Id);
        if (existingSale != null)
        {
            var index = _sales.IndexOf(existingSale);
            _sales[index] = entity;
            _logger.LogInformation("Updated sale {SaleId} in mock repository", entity.Id);
        }
        return Task.FromResult(entity);
    }

    public Task DeleteAsync(Sale entity, CancellationToken cancellationToken = default)
    {
        _sales.RemoveAll(s => s.Id == entity.Id);
        _logger.LogInformation("Deleted sale {SaleId} from mock repository", entity.Id);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _sales.RemoveAll(s => s.Id == id);
        _logger.LogInformation("Deleted sale {SaleId} from mock repository", id);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_sales.Any(s => s.Id == id));
    }

    public Task<long> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult((long)_sales.Count);
    }

    public Task<Sale?> GetByReceiptNumberAsync(string receiptNumber, CancellationToken cancellationToken = default)
    {
        var sale = _sales.FirstOrDefault(s => s.ReceiptNumber.Value == receiptNumber);
        return Task.FromResult(sale);
    }

    public Task<IEnumerable<Sale>> GetByTillAsync(Guid tillId, CancellationToken cancellationToken = default)
    {
        var sales = _sales.Where(s => s.TillId == tillId);
        return Task.FromResult(sales);
    }

    public Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, Guid? tillId = null, CancellationToken cancellationToken = default)
    {
        var sales = _sales.Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate);
        
        if (tillId.HasValue)
        {
            sales = sales.Where(s => s.TillId == tillId.Value);
        }
        
        return Task.FromResult(sales);
    }

    public Task<IEnumerable<Sale>> GetByStatusAsync(SaleStatus status, CancellationToken cancellationToken = default)
    {
        var sales = _sales.Where(s => s.Status == status);
        return Task.FromResult(sales);
    }

    public Task<IEnumerable<Sale>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var sales = _sales.Where(s => s.CustomerId == customerId);
        return Task.FromResult(sales);
    }

    public Task<DailySalesSummary> GetDailySalesSummaryAsync(DateTime date, Guid? tillId = null, CancellationToken cancellationToken = default)
    {
        var startDate = date.Date;
        var endDate = startDate.AddDays(1).AddTicks(-1);
        
        var sales = _sales.Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate);
        
        if (tillId.HasValue)
        {
            sales = sales.Where(s => s.TillId == tillId.Value);
        }
        
        var salesList = sales.ToList();
        var completedSales = salesList.Where(s => s.Status == SaleStatus.Completed).ToList();
        var cancelledSales = salesList.Where(s => s.Status == SaleStatus.Cancelled).ToList();

        var summary = new DailySalesSummary
        {
            Date = date,
            TotalSales = completedSales.Count,
            TotalCancelled = cancelledSales.Count,
            TotalRevenue = completedSales.Sum(s => s.Total.Amount),
            TotalTax = completedSales.Sum(s => s.TaxAmount.Amount),
            TotalDiscount = completedSales.Sum(s => s.DiscountAmount.Amount),
            AverageTransactionValue = completedSales.Any() ? completedSales.Average(s => s.Total.Amount) : 0
        };

        return Task.FromResult(summary);
    }

    public Task<Dictionary<SaleStatus, int>> GetSalesCountByStatusAsync(CancellationToken cancellationToken = default)
    {
        var counts = _sales.GroupBy(s => s.Status)
                          .ToDictionary(g => g.Key, g => g.Count());
        return Task.FromResult(counts);
    }

    public Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate, Guid? tillId = null, CancellationToken cancellationToken = default)
    {
        var sales = _sales.Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate && s.Status == SaleStatus.Completed);
        
        if (tillId.HasValue)
        {
            sales = sales.Where(s => s.TillId == tillId.Value);
        }
        
        var totalRevenue = sales.Sum(s => s.Total.Amount);
        return Task.FromResult(totalRevenue);
    }
}
