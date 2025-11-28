using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;

namespace Toss.Application.CRM.Queries.SearchCustomers;

public record CustomerSearchResultDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public int? StoreId { get; init; }
}

public record SearchCustomersQuery : IRequest<List<CustomerSearchResultDto>>
{
    public string SearchTerm { get; init; } = string.Empty;
    public int? StoreId { get; init; }
}

public class SearchCustomersQueryHandler : IRequestHandler<SearchCustomersQuery, List<CustomerSearchResultDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public SearchCustomersQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<List<CustomerSearchResultDto>> Handle(SearchCustomersQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            return new List<CustomerSearchResultDto>();
        }

        var businessId = _businessContext.CurrentBusinessId!.Value;

        // Normalize search term
        var searchTerm = request.SearchTerm.ToLower().Trim();

        // Build query with text search across multiple fields
        var query = _context.Customers
            .Where(c => c.BusinessId == businessId)
            .Where(c =>
                (c.FirstName + " " + c.LastName).ToLower().Contains(searchTerm) ||
                (c.Email != null && c.Email.ToLower().Contains(searchTerm)) ||
                (c.Phone != null && c.Phone.Number != null && c.Phone.Number.ToLower().Contains(searchTerm))
            );

        if (request.StoreId.HasValue)
        {
            query = query.Where(c => c.StoreId == request.StoreId);
        }

        var customers = await query
            .Select(c => new CustomerSearchResultDto
            {
                Id = c.Id,
                Name = c.FirstName + " " + c.LastName,
                Email = c.Email ?? string.Empty,
                Phone = c.Phone != null ? c.Phone.Number : null,
                Address = c.Address != null ? $"{c.Address.Street}, {c.Address.City}" : null,
                StoreId = c.StoreId
            })
            .Take(50) // Limit search results to 50
            .ToListAsync(cancellationToken);

        return customers;
    }
}

