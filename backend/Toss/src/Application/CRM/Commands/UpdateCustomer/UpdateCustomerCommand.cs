using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.CRM;
using Toss.Domain.Entities.Stores;
using Toss.Domain.ValueObjects;

namespace Toss.Application.CRM.Commands.UpdateCustomer;

public record UpdateCustomerCommand : IRequest<int>
{
    public int Id { get; init; }
    public int? StoreId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public bool AllowsMarketing { get; init; }
    public string? Notes { get; init; }
    public decimal? CreditLimit { get; init; }
    public string? Tags { get; init; }
    public bool? IsActive { get; init; }
}

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public UpdateCustomerCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<int> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No active business context available.");
        }

        var businessId = _businessContext.CurrentBusinessId!.Value;

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (customer == null)
        {
            throw new NotFoundException(nameof(Customer), request.Id.ToString());
        }

        if (customer.BusinessId != businessId)
        {
            throw new ForbiddenAccessException("Customer does not belong to the current business.");
        }

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

        customer.StoreId = request.StoreId;
        customer.Store = store;
        customer.FirstName = request.FirstName;
        customer.LastName = request.LastName;
        customer.Phone = !string.IsNullOrWhiteSpace(request.PhoneNumber)
            ? new PhoneNumber(request.PhoneNumber)
            : null;
        customer.Email = request.Email;
        customer.AllowsMarketing = request.AllowsMarketing;
        customer.Notes = request.Notes;
        
        if (request.CreditLimit.HasValue)
        {
            customer.CreditLimit = request.CreditLimit.Value;
        }
        
        customer.Tags = string.IsNullOrWhiteSpace(request.Tags) ? null : request.Tags;
        
        if (request.IsActive.HasValue)
        {
            customer.IsActive = request.IsActive.Value;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }
}

