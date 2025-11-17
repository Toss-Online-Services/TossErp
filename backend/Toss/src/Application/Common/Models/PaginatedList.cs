namespace Toss.Application.Common.Models;

/// <summary>
/// Represents a paginated list of items with metadata about pagination state.
/// </summary>
/// <typeparam name="T">The type of items in the list.</typeparam>
/// <remarks>
/// This is the standard pagination pattern used throughout the Application layer for queries.
/// Provides:
/// <list type="bullet">
/// <item><description>Current page items</description></item>
/// <item><description>Total count for calculating pages</description></item>
/// <item><description>Navigation helpers (HasPreviousPage, HasNextPage)</description></item>
/// </list>
/// Use <see cref="CreateAsync"/> to create from an IQueryable for efficient database pagination.
/// </remarks>
public class PaginatedList<T>
{
    /// <summary>
    /// Gets the items for the current page.
    /// </summary>
    public IReadOnlyCollection<T> Items { get; }
    
    /// <summary>
    /// Gets the current page number (1-based).
    /// </summary>
    public int PageNumber { get; }
    
    /// <summary>
    /// Gets the total number of pages based on page size and total count.
    /// </summary>
    public int TotalPages { get; }
    
    /// <summary>
    /// Gets the total number of items across all pages.
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="PaginatedList{T}"/>.
    /// </summary>
    /// <param name="items">The items for this page.</param>
    /// <param name="count">The total count of items across all pages.</param>
    /// <param name="pageNumber">The current page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    public PaginatedList(IReadOnlyCollection<T> items, int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Items = items;
    }

    /// <summary>
    /// Gets a value indicating whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Gets a value indicating whether there is a next page.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Creates a paginated list from an IQueryable by executing a count and skip/take query.
    /// </summary>
    /// <param name="source">The queryable source to paginate.</param>
    /// <param name="pageNumber">The page number to retrieve (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A paginated list with the requested page of items.</returns>
    /// <remarks>
    /// This method executes two database queries:
    /// <list type="number">
    /// <item><description>COUNT query to get total items</description></item>
    /// <item><description>Skip/Take query to get the current page items</description></item>
    /// </list>
    /// </remarks>
    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken);
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}
