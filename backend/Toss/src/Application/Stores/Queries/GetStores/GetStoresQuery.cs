using Toss.Application.Common.Interfaces;

namespace Toss.Application.Stores.Queries.GetStores;

public record StoreListDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Url { get; init; }
    public bool IsActive { get; init; }
    public string? AreaGroup { get; init; }
    public string? CompanyName { get; init; }
    public int DisplayOrder { get; init; }
    public int CustomerCount { get; init; }
    public int ProductCount { get; init; }
}

public record GetStoresQuery : IRequest<List<StoreListDto>>
{
    public string? SearchTerm { get; init; }
    public bool? ActiveOnly { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}

public class GetStoresQueryHandler : IRequestHandler<GetStoresQuery, List<StoreListDto>>
{
    private readonly IApplicationDbContext _context;

    public GetStoresQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<StoreListDto>> Handle(GetStoresQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Stores.AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(s => 
                s.Name.ToLower().Contains(searchTerm) ||
                (s.Description != null && s.Description.ToLower().Contains(searchTerm)) ||
                (s.CompanyName != null && s.CompanyName.ToLower().Contains(searchTerm))
            );
        }

        // Apply active filter
        if (request.ActiveOnly.HasValue && request.ActiveOnly.Value)
        {
            query = query.Where(s => s.IsActive);
        }

        // Order by display order, then name
        query = query.OrderBy(s => s.DisplayOrder).ThenBy(s => s.Name);

        // Paginate
        var stores = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new StoreListDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Url = s.Url,
                IsActive = s.IsActive,
                AreaGroup = s.AreaGroup,
                CompanyName = s.CompanyName,
                DisplayOrder = s.DisplayOrder,
                CustomerCount = _context.Customers.Count(c => c.ShopId == s.Id),
                ProductCount = _context.StockLevels.Count(sl => sl.ShopId == s.Id)
            })
            .ToListAsync(cancellationToken);

        return stores;
    }
}

