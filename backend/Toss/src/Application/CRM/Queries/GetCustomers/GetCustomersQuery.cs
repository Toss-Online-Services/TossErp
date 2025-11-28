using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
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
    public DateTimeOffset? LastPurchaseDate { get; init; }
    public int? StoreId { get; init; }
    public string StoreName { get; init; } = string.Empty;
    public string? Tags { get; init; }
    public decimal CreditLimit { get; init; }
}

public record GetCustomersQuery : IRequest<PaginatedList<CustomerDto>>
{
    public int? StoreId { get; init; }
    public string? SearchTerm { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, PaginatedList<CustomerDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetCustomersQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<PaginatedList<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            return new PaginatedList<CustomerDto>(new List<CustomerDto>(), 0, request.PageNumber, request.PageSize);
        }

        var businessId = _businessContext.CurrentBusinessId!.Value;

        var query = _context.Customers
            .Where(c => c.BusinessId == businessId);

        if (request.StoreId.HasValue)
        {
            query = query.Where(c => c.StoreId == request.StoreId);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(c =>
                c.FirstName.Contains(request.SearchTerm) ||
                c.LastName.Contains(request.SearchTerm) ||
                (c.Email != null && c.Email.Contains(request.SearchTerm)) ||
                (c.Phone != null && c.Phone.Number.Contains(request.SearchTerm)));
        }

        var customers = await query
            .OrderByDescending(c => c.LastPurchaseDate)
            .Select(c => new CustomerDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email ?? string.Empty,
                PhoneNumber = c.Phone != null ? c.Phone.Number : null,
                TotalPurchases = c.TotalPurchases,
                LastPurchaseDate = c.LastPurchaseDate,
                StoreId = c.StoreId,
                StoreName = c.Store != null ? c.Store.Name : string.Empty,
                Tags = c.Tags,
                CreditLimit = c.CreditLimit
            })
            .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

        return customers;
    }
}

