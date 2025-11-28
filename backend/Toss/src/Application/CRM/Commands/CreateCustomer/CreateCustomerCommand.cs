using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities;
using Toss.Domain.Entities.CRM;
using Toss.Domain.Entities.Stores;
using Toss.Domain.ValueObjects;

namespace Toss.Application.CRM.Commands.CreateCustomer;

public record CreateCustomerCommand : IRequest<int>
{
    public int? StoreId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public bool AllowsMarketing { get; init; }
    public string? Notes { get; init; }
    public decimal? CreditLimit { get; init; }
    public string? Tags { get; init; }
}

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public CreateCustomerCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<int> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No active business context available.");
        }

        var businessId = _businessContext.CurrentBusinessId!.Value;

        Store? store = null;
        if (request.StoreId.HasValue)
        {
            store = await _context.Stores.FindAsync(new object[] { request.StoreId.Value }, cancellationToken);

            if (store == null)
            {
                throw new NotFoundException(nameof(Store), request.StoreId.Value.ToString());
            }

            if (store.BusinessId != businessId)
            {
                throw new ForbiddenAccessException("Store does not belong to the current business.");
            }
        }

        var customer = new Customer
        {
            BusinessId = businessId,
            StoreId = request.StoreId,
            Store = store,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = !string.IsNullOrWhiteSpace(request.PhoneNumber) 
                ? new PhoneNumber(request.PhoneNumber) 
                : null,
            Email = request.Email,
            AllowsMarketing = request.AllowsMarketing,
            CreditLimit = request.CreditLimit ?? 0,
            Tags = string.IsNullOrWhiteSpace(request.Tags) ? null : request.Tags,
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

