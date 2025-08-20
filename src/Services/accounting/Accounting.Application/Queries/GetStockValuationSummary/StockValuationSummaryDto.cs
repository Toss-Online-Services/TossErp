using TossErp.Accounting.Domain.Enums;

namespace TossErp.Accounting.Application.Queries.GetStockValuationSummary;

/// <summary>
/// DTO for stock valuation summary data
/// </summary>
public class StockValuationSummaryDto
{
    public DateTime AsOfDate { get; init; }
    public decimal TotalValue { get; init; }
    public string Currency { get; init; } = string.Empty;
    public int ItemCount { get; init; }
    public int WarehouseCount { get; init; }
    public ValuationMethod Method { get; init; }
    public Dictionary<string, decimal> WarehouseValues { get; init; } = new();
    public Dictionary<string, decimal> CategoryValues { get; init; } = new();
}


