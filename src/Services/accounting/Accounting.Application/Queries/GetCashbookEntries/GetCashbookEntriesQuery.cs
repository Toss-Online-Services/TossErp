using MediatR;
using TossErp.Accounting.Application.Common.DTOs;

namespace TossErp.Accounting.Application.Queries.GetCashbookEntries;

/// <summary>
/// Query to get cashbook entries with filtering and pagination
/// </summary>
public class GetCashbookEntriesQuery : IRequest<CashbookEntriesResponse>
{
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public string? Category { get; init; }
    public string? Type { get; init; }
    public decimal? MinAmount { get; init; }
    public decimal? MaxAmount { get; init; }
    public string? Reference { get; init; }
    public string? Description { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SortBy { get; init; }
    public string? SortDirection { get; init; } = "asc";
}

