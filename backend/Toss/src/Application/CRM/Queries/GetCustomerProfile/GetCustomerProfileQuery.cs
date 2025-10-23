using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.CRM;

namespace Toss.Application.CRM.Queries.GetCustomerProfile;

public record CustomerProfileDto
{
    public int Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public int ShopId { get; init; }
    public string ShopName { get; init; } = string.Empty;
    public decimal TotalPurchases { get; init; }
    public DateTime? LastPurchaseDate { get; init; }
    public int PurchaseCount { get; init; }
    public DateTime CreatedDate { get; init; }
    public List<RecentPurchaseDto> RecentPurchases { get; init; } = new();
}

public record RecentPurchaseDto
{
    public int SaleId { get; init; }
    public DateTime PurchaseDate { get; init; }
    public decimal Amount { get; init; }
}

public record GetCustomerProfileQuery : IRequest<CustomerProfileDto>
{
    public int Id { get; init; }
}

public class GetCustomerProfileQueryHandler : IRequestHandler<GetCustomerProfileQuery, CustomerProfileDto>
{
    private readonly IApplicationDbContext _context;

    public GetCustomerProfileQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerProfileDto> Handle(GetCustomerProfileQuery request, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers
            .Include(c => c.Shop)
            .Include(c => c.Purchases.OrderByDescending(p => p.PurchaseDate).Take(10))
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (customer == null)
            throw new NotFoundException(nameof(Customer), request.Id.ToString());

        return new CustomerProfileDto
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email ?? string.Empty,
            PhoneNumber = customer.PhoneNumber?.Value,
            ShopId = customer.ShopId,
            ShopName = customer.Shop.Name,
            TotalPurchases = customer.TotalPurchases,
            LastPurchaseDate = customer.LastPurchaseDate,
            PurchaseCount = customer.Purchases.Count,
            CreatedDate = customer.Created,
            RecentPurchases = customer.Purchases.Select(p => new RecentPurchaseDto
            {
                SaleId = p.SaleId,
                PurchaseDate = p.PurchaseDate,
                Amount = p.TotalAmount
            }).ToList()
        };
    }
}

