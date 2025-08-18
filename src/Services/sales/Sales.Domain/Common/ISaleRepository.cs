using TossErp.Sales.Domain.Entities;
using TossErp.Sales.Domain.Enums;

namespace TossErp.Sales.Domain.Common;

/// <summary>
/// Repository interface for Sale aggregate
/// </summary>
public interface ISaleRepository : IRepository<Sale>
{
    /// <summary>
    /// Get sale by receipt number
    /// </summary>
    Task<Sale?> GetByReceiptNumberAsync(string receiptNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get sales by till
    /// </summary>
    Task<IEnumerable<Sale>> GetByTillAsync(Guid tillId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get sales by date range
    /// </summary>
    Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, Guid? tillId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get sales by status
    /// </summary>
    Task<IEnumerable<Sale>> GetByStatusAsync(SaleStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get sales by customer
    /// </summary>
    Task<IEnumerable<Sale>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get daily sales summary
    /// </summary>
    Task<DailySalesSummary> GetDailySalesSummaryAsync(DateTime date, Guid? tillId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get sales count by status
    /// </summary>
    Task<Dictionary<SaleStatus, int>> GetSalesCountByStatusAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get total revenue for date range
    /// </summary>
    Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate, Guid? tillId = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Daily sales summary
/// </summary>
public class DailySalesSummary
{
    public DateTime Date { get; set; }
    public int TotalSales { get; set; }
    public int TotalCancelled { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalTax { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal AverageTransactionValue { get; set; }
}
