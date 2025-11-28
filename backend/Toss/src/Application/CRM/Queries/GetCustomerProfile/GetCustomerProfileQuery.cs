using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.CRM;

namespace Toss.Application.CRM.Queries.GetCustomerProfile;

public record CustomerProfileDto
{
    public int Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public int? StoreId { get; init; }
    public string StoreName { get; init; } = string.Empty;
    public decimal TotalPurchases { get; init; }
    public DateTimeOffset? LastPurchaseDate { get; init; }
    public int PurchaseCount { get; init; }
    public DateTimeOffset CreatedDate { get; init; }
    public List<RecentPurchaseDto> RecentPurchases { get; init; } = new();
}

public record RecentPurchaseDto
{
    public int SaleId { get; init; }
    public DateTimeOffset PurchaseDate { get; init; }
    public decimal Amount { get; init; }
}

public record GetCustomerProfileQuery : IRequest<CustomerProfileDto>
{
    public int Id { get; init; }
}

public class GetCustomerProfileQueryHandler : IRequestHandler<GetCustomerProfileQuery, CustomerProfileDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetCustomerProfileQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<CustomerProfileDto> Handle(GetCustomerProfileQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No active business context available.");
        }

        var businessId = _businessContext.CurrentBusinessId!.Value;

        var customer = await _context.Customers
            .Include(c => c.Store)
            .Include(c => c.Purchases.OrderByDescending(p => p.PurchaseDate).Take(10))
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.BusinessId == businessId, cancellationToken);

        if (customer == null)
            throw new NotFoundException(nameof(Customer), request.Id.ToString());

        return new CustomerProfileDto
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email ?? string.Empty,
            PhoneNumber = customer.Phone?.Number,
            StoreId = customer.StoreId,
            StoreName = customer.Store?.Name ?? string.Empty,
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

