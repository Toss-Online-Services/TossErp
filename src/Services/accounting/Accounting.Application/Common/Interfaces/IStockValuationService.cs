using TossErp.Accounting.Domain.Common;
using TossErp.Accounting.Domain.Enums;

namespace TossErp.Accounting.Application.Common.Interfaces;

/// <summary>
/// Service for calculating stock valuations for P&L reporting
/// </summary>
public interface IStockValuationService
{
    /// <summary>
    /// Calculate stock valuation for a specific date using the specified method
    /// </summary>
    Task<Money> CalculateStockValuationAsync(DateTime valuationDate, ValuationMethod method, string tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Calculate stock valuation for a specific warehouse
    /// </summary>
    Task<Money> CalculateWarehouseValuationAsync(string warehouseCode, DateTime valuationDate, ValuationMethod method, string tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Calculate stock valuation for a specific item
    /// </summary>
    Task<Money> CalculateItemValuationAsync(string itemCode, DateTime valuationDate, ValuationMethod method, string tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create a stock valuation snapshot for P&L reporting
    /// </summary>
    Task<Guid> CreateStockValuationSnapshotAsync(DateTime snapshotDate, ValuationMethod method, string tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get the total stock value for P&L calculations
    /// </summary>
    Task<Money> GetTotalStockValueForPLAsync(DateTime asOfDate, string tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get stock valuation summary for reporting
    /// </summary>
    Task<StockValuationSummary> GetStockValuationSummaryAsync(DateTime asOfDate, string tenantId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Summary of stock valuation for reporting
/// </summary>
public record StockValuationSummary
{
    public DateTime AsOfDate { get; init; }
    public Money TotalValue { get; init; } = Money.Zero();
    public int ItemCount { get; init; }
    public int WarehouseCount { get; init; }
    public ValuationMethod Method { get; init; }
    public Dictionary<string, Money> WarehouseValues { get; init; } = new();
    public Dictionary<string, Money> CategoryValues { get; init; } = new();
}
