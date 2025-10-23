using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Mappings;
using Toss.Application.Common.Models;
using Toss.Domain.Entities.CRM;

namespace Toss.Application.CRM.Queries.GetCustomers;

public record CustomerDto
{
    public int Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public decimal TotalPurchases { get; init; }
    public DateTime? LastPurchaseDate { get; init; }
}

public record GetCustomersQuery : IRequest<PaginatedList<CustomerDto>>
{
    public int ShopId { get; init; }
    public string? SearchTerm { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, PaginatedList<CustomerDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCustomersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Customers
            .Where(c => c.ShopId == request.ShopId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(c =>
                c.FirstName.Contains(request.SearchTerm) ||
                c.LastName.Contains(request.SearchTerm) ||
                (c.Email != null && c.Email.Contains(request.SearchTerm)) ||
                (c.PhoneNumber != null && c.PhoneNumber.Value.Contains(request.SearchTerm)));
        }

        var customers = await query
            .OrderByDescending(c => c.LastPurchaseDate)
            .Select(c => new CustomerDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email ?? string.Empty,
                PhoneNumber = c.PhoneNumber != null ? c.PhoneNumber.Value : null,
                TotalPurchases = c.TotalPurchases,
                LastPurchaseDate = c.LastPurchaseDate
            })
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return customers;
    }
}

