using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Stores;
using Toss.Domain.Entities;
using Toss.Domain.Entities.CRM;
using Toss.Domain.ValueObjects;

namespace Toss.Application.CRM.Commands.CreateCustomer;

public record CreateCustomerCommand : IRequest<int>
{
    public int ShopId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public bool AllowsMarketing { get; init; }
    public string? Notes { get; init; }
}

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateCustomerCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        // Validate shop exists
        var shop = await _context.Shops.FindAsync(new object[] { request.ShopId }, cancellationToken);
        if (shop == null)
            throw new NotFoundException(nameof(Store), request.ShopId.ToString());

        var customer = new Customer
        {
            ShopId = request.ShopId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = !string.IsNullOrWhiteSpace(request.PhoneNumber) 
                ? new PhoneNumber(request.PhoneNumber) 
                : null,
            Email = request.Email,
            AllowsMarketing = request.AllowsMarketing,
            IsActive = true,
            Notes = request.Notes,
            TotalPurchaseAmount = 0,
            TotalPurchaseCount = 0
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }
}

