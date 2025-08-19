using MediatR;
using TossErp.Accounting.Application.Common.Interfaces;

namespace TossErp.Accounting.Application.Queries.GetStockValuationSummary;

/// <summary>
/// Query to get stock valuation summary for P&L reporting
/// </summary>
public class GetStockValuationSummaryQuery : IRequest<StockValuationSummaryDto>
{
    public DateTime AsOfDate { get; init; }
}

