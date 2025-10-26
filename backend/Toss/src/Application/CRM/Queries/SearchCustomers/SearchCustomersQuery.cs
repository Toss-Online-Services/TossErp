using Toss.Application.Common.Interfaces;

namespace Toss.Application.CRM.Queries.SearchCustomers;

public record CustomerSearchResultDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public int ShopId { get; init; }
}

public record SearchCustomersQuery : IRequest<List<CustomerSearchResultDto>>
{
    public string SearchTerm { get; init; } = string.Empty;
    public int ShopId { get; init; }
}

public class SearchCustomersQueryHandler : IRequestHandler<SearchCustomersQuery, List<CustomerSearchResultDto>>
{
    private readonly IApplicationDbContext _context;

    public SearchCustomersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CustomerSearchResultDto>> Handle(SearchCustomersQuery request, CancellationToken cancellationToken)
    {
        // Normalize search term
        var searchTerm = request.SearchTerm.ToLower().Trim();

        // Build query with text search across multiple fields
        var query = _context.Customers
            .Where(c => c.ShopId == request.ShopId)
            .Where(c =>
                c.Name.ToLower().Contains(searchTerm) ||
                c.Email.ToLower().Contains(searchTerm) ||
                (c.Phone != null && c.Phone.Contains(searchTerm)) ||
                (c.Address != null && c.Address.ToLower().Contains(searchTerm))
            );

        var customers = await query
            .Select(c => new CustomerSearchResultDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address,
                ShopId = c.ShopId
            })
            .Take(50) // Limit search results to 50
            .ToListAsync(cancellationToken);

        return customers;
    }
}

