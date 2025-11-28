using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.CRM;

namespace Toss.Application.CRM.Commands.DeleteCustomer;

public record DeleteCustomerCommand : IRequest<bool>
{
    public int Id { get; init; }
}

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public DeleteCustomerCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
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

        // Check if customer has active sales or interactions
        var hasSales = await _context.Sales
            .AnyAsync(s => s.CustomerId == request.Id, cancellationToken);

        var hasInteractions = await _context.CustomerInteractions
            .AnyAsync(ci => ci.CustomerId == request.Id, cancellationToken);

        if (hasSales || hasInteractions)
        {
            throw new BadRequestException(
                "Cannot delete customer with existing sales or interactions. " +
                "Please deactivate the customer instead.");
        }

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

