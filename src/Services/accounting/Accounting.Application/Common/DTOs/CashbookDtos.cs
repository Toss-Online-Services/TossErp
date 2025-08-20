using TossErp.Accounting.Domain.Enums;

namespace TossErp.Accounting.Application.Common.DTOs;

/// <summary>
/// DTO for cashbook entry data
/// </summary>
public class CashbookEntryDto
{
    public Guid Id { get; init; }
    public Guid CashbookId { get; init; }
    public decimal Amount { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Reference { get; init; }
    public DateTime TransactionDate { get; init; }
    public DateTime CreatedAt { get; init; }
    public string CreatedBy { get; init; } = string.Empty;
    public DateTime? UpdatedAt { get; init; }
    public string? UpdatedBy { get; init; }
}

/// <summary>
/// Response for cashbook entries query
/// </summary>
public class CashbookEntriesResponse
{
    public IEnumerable<CashbookEntryDto> Entries { get; init; } = new List<CashbookEntryDto>();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    public bool HasNextPage { get; init; }
    public bool HasPreviousPage { get; init; }
}

/// <summary>
/// Response for cashbook summary query
/// </summary>
public class CashbookSummaryResponse
{
    public DateTime AsOfDate { get; init; }
    public decimal TotalDebits { get; init; }
    public decimal TotalCredits { get; init; }
    public decimal NetBalance { get; init; }
    public string Currency { get; init; } = "ZAR";
    public int TotalEntries { get; init; }
    public Dictionary<string, decimal> CategoryTotals { get; init; } = new();
    public Dictionary<string, decimal> TypeTotals { get; init; } = new();
}

/// <summary>
/// Request for adding a cashbook entry
/// </summary>
public class AddCashbookEntryRequest
{
    public Guid CashbookId { get; init; }
    public decimal Amount { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Reference { get; init; }
    public DateTime TransactionDate { get; init; } = DateTime.Today;
}

/// <summary>
/// Response for cashbook filters
/// </summary>
public class CashbookFiltersResponse
{
    public string[] Categories { get; init; } = Array.Empty<string>();
    public string[] Types { get; init; } = Array.Empty<string>();
    public DateRangeFilter DateRange { get; init; } = new();
}

/// <summary>
/// Date range filter
/// </summary>
public class DateRangeFilter
{
    public DateTime MinDate { get; init; }
    public DateTime MaxDate { get; init; }
}

/// <summary>
/// Pagination parameters
/// </summary>
public class PaginationParameters
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SortBy { get; init; }
    public string? SortDirection { get; init; } = "asc";
}

/// <summary>
/// Cashbook entry filter parameters
/// </summary>
public class CashbookEntryFilters
{
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public string? Category { get; init; }
    public string? Type { get; init; }
    public decimal? MinAmount { get; init; }
    public decimal? MaxAmount { get; init; }
    public string? Reference { get; init; }
    public string? Description { get; init; }
}


