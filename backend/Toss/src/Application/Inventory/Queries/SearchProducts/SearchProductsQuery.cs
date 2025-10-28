using Toss.Application.Common.Interfaces;

namespace Toss.Application.Inventory.Queries.SearchProducts;

public record SearchProductsQuery : IRequest<SearchProductsResult>
{
    public int ShopId { get; init; }
    public string? SearchTerm { get; init; }
    public int? CategoryId { get; init; }
    public bool? InStock { get; init; }
    public bool? LowStock { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public record ProductSearchDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Sku { get; init; } = string.Empty;
    public string? Barcode { get; init; }
    public decimal BasePrice { get; init; }
    public string? ImageUrl { get; init; }
    public int CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public int AvailableStock { get; init; }
    public bool IsActive { get; init; }
    public bool IsTaxable { get; init; }
}

public record SearchProductsResult
{
    public List<ProductSearchDto> Products { get; init; } = new();
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
    public int CurrentPage { get; init; }
}

public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, SearchProductsResult>
{
    private readonly IApplicationDbContext _context;

    public SearchProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SearchProductsResult> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.StockLevels)
            .Where(p => p.IsActive)
            .AsQueryable();

        // Apply search term filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(searchTerm) ||
                p.SKU.ToLower().Contains(searchTerm) ||
                (p.Barcode != null && p.Barcode.ToLower().Contains(searchTerm)) ||
                (p.Description != null && p.Description.ToLower().Contains(searchTerm))
            );
        }

        // Apply category filter
        if (request.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);
        }

        // Apply price filters
        if (request.MinPrice.HasValue)
        {
            query = query.Where(p => p.BasePrice >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(p => p.BasePrice <= request.MaxPrice.Value);
        }

        // Apply stock filters
        if (request.InStock.HasValue && request.InStock.Value)
        {
            query = query.Where(p => p.StockLevels.Any(s => s.ShopId == request.ShopId && s.AvailableStock > 0));
        }

        if (request.LowStock.HasValue && request.LowStock.Value)
        {
            query = query.Where(p => p.StockLevels.Any(s =>
                s.ShopId == request.ShopId &&
                s.AvailableStock > 0 &&
                s.AvailableStock <= p.MinimumStockLevel
            ));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var products = await query
            .OrderBy(p => p.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProductSearchDto
            {
                Id = p.Id,
                Name = p.Name,
                Sku = p.SKU,
                Barcode = p.Barcode,
                BasePrice = p.BasePrice,
                ImageUrl = null, // TODO: Add image support to Product entity
                CategoryId = p.CategoryId ?? 0,
                CategoryName = p.Category != null ? p.Category.Name : string.Empty,
                AvailableStock = p.StockLevels
                    .Where(s => s.ShopId == request.ShopId)
                    .Select(s => s.AvailableStock)
                    .FirstOrDefault(),
                IsActive = p.IsActive,
                IsTaxable = p.IsTaxable
            })
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return new SearchProductsResult
        {
            Products = products,
            TotalCount = totalCount,
            TotalPages = totalPages,
            CurrentPage = request.PageNumber
        };
    }
}

