using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Mappings;
using Toss.Application.Common.Models;
using Toss.Domain.Entities.Catalog;

namespace Toss.Application.Inventory.Queries.GetProducts;

public record GetProductsQuery : IRequest<PaginatedList<ProductDto>>
{
    public string? SearchTerm { get; init; }
    public int? CategoryId { get; init; }
    public bool? IsActive { get; init; }
    public int? ShopId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PaginatedList<ProductDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        // Join with StockLevels to get available stock for the shop
        var query = from p in _context.Products
                    join sl in _context.StockLevels on p.Id equals sl.ProductId into stockLevels
                    from sl in stockLevels.Where(s => !request.ShopId.HasValue || s.ShopId == request.ShopId.Value).DefaultIfEmpty()
                    select new { Product = p, StockLevel = sl };

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(x => 
                x.Product.Name.ToLower().Contains(searchTerm) ||
                x.Product.SKU.ToLower().Contains(searchTerm) ||
                (x.Product.Barcode != null && x.Product.Barcode.ToLower().Contains(searchTerm)));
        }

        if (request.CategoryId.HasValue)
            query = query.Where(x => x.Product.CategoryId == request.CategoryId.Value);

        if (request.IsActive.HasValue)
            query = query.Where(x => x.Product.IsActive == request.IsActive.Value);

        var projectedQuery = query
            .OrderBy(x => x.Product.Name)
            .Select(x => new ProductDto
            {
                Id = x.Product.Id,
                SKU = x.Product.SKU,
                Barcode = x.Product.Barcode,
                Name = x.Product.Name,
                CategoryId = x.Product.CategoryId,
                CategoryName = x.Product.Category != null ? x.Product.Category.Name : null,
                BasePrice = x.Product.BasePrice,
                IsActive = x.Product.IsActive,
                AvailableStock = x.StockLevel != null ? x.StockLevel.CurrentStock : 0
            });

        return await projectedQuery.PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}

public class ProductDto
{
    public int Id { get; init; }
    public string SKU { get; init; } = string.Empty;
    public string? Barcode { get; init; }
    public string Name { get; init; } = string.Empty;
    public int? CategoryId { get; init; }
    public string? CategoryName { get; init; }
    public decimal BasePrice { get; init; }
    public bool IsActive { get; init; }
    public int AvailableStock { get; init; }
}

