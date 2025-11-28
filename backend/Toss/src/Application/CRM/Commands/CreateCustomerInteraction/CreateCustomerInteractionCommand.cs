using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.CRM;

namespace Toss.Application.CRM.Commands.CreateCustomerInteraction;

public record CreateCustomerInteractionCommand : IRequest<int>
{
    public int CustomerId { get; init; }
    public string InteractionType { get; init; } = string.Empty;
    public string? Subject { get; init; }
    public string? Description { get; init; }
    public DateTimeOffset InteractionDate { get; init; }
    public string? InteractionBy { get; init; }
    public bool RequiresFollowUp { get; init; }
    public DateTimeOffset? FollowUpDate { get; init; }
}

public class CreateCustomerInteractionCommandHandler : IRequestHandler<CreateCustomerInteractionCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public CreateCustomerInteractionCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<int> Handle(CreateCustomerInteractionCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No active business context available.");
        }

        var businessId = _businessContext.CurrentBusinessId!.Value;

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == request.CustomerId, cancellationToken);

        if (customer == null)
        {
            throw new NotFoundException(nameof(Customer), request.CustomerId.ToString());
        }

        if (customer.BusinessId != businessId)
        {
            throw new ForbiddenAccessException("Customer does not belong to the current business.");
        }

        var interaction = new CustomerInteraction
        {
            BusinessId = businessId,
            CustomerId = request.CustomerId,
            Customer = customer,
            InteractionType = request.InteractionType,
            Subject = request.Subject,
            Description = request.Description,
            InteractionDate = request.InteractionDate,
            InteractionBy = request.InteractionBy,
            RequiresFollowUp = request.RequiresFollowUp,
            FollowUpDate = request.FollowUpDate,
            IsFollowedUp = false
        };

        _context.CustomerInteractions.Add(interaction);
        await _context.SaveChangesAsync(cancellationToken);

        return interaction.Id;
    }
}

