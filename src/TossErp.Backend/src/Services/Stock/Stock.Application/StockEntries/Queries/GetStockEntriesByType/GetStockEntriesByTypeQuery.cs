using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.StockEntries.Queries.GetStockEntriesByType;

public record GetStockEntriesByTypeQuery : IRequest<List<StockEntryDto>>
{
    public string EntryType { get; init; } = string.Empty;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchString { get; init; }
    public string? SortBy { get; init; }
    public bool IsDescending { get; init; } = false;
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
} 
