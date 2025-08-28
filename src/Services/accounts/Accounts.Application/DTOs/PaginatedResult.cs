using System.Collections.Generic;

namespace TossErp.Accounts.Application.DTOs;

/// <summary>
/// Paginated result for collections with township SMME support
/// </summary>
public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; set; } = null!;
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }

    public PaginatedResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        HasPreviousPage = pageNumber > 1;
        HasNextPage = pageNumber < TotalPages;
    }

    public PaginatedResult()
    {
        Items = new List<T>();
    }
}
