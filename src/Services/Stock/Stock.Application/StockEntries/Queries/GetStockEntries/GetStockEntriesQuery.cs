using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.StockEntries.Queries.GetStockEntries;

/// <summary>
/// Query for retrieving stock entries with pagination
/// </summary>
public record GetStockEntriesQuery : IRequest<List<StockEntryDto>>
{
    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int? Page { get; init; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int? PageSize { get; init; }
} 
