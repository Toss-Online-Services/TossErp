using MediatR;
using Crm.Application.Commands;
using TossErp.CRM.Domain.Aggregates;
using TossErp.CRM.Domain.Repositories;
using TossErp.CRM.Domain.ValueObjects;

namespace Crm.Application.Handlers;

/// <summary>
/// Handler for creating new opportunities
/// </summary>
public class CreateOpportunityHandler : IRequestHandler<CreateOpportunityCommand, Guid>
{
    private readonly IOpportunityRepository _opportunityRepository;

    public CreateOpportunityHandler(IOpportunityRepository opportunityRepository)
    {
        _opportunityRepository = opportunityRepository;
    }

    public async Task<Guid> Handle(CreateOpportunityCommand request, CancellationToken cancellationToken)
    {
        // Create opportunity value object
        var money = new Money(request.EstimatedValue, "USD");
        var probability = request.Probability ?? 50.0m;
        var opportunityValue = new OpportunityValue(money, probability);

        // Create opportunity aggregate
        var opportunity = new Opportunity(
            id: Guid.NewGuid(),
            tenantId: Guid.Empty, // TODO: Get from user context
            name: request.Name,
            customerId: request.CustomerId,
            value: opportunityValue,
            expectedCloseDate: request.ExpectedCloseDate,
            createdBy: "System", // TODO: Get from user context
            description: request.Description,
            type: request.Type,
            priority: request.Priority,
            source: request.Source,
            leadId: request.LeadId,
            campaignName: request.CampaignName
        );

        // Save opportunity
        await _opportunityRepository.AddAsync(opportunity, cancellationToken);

        return opportunity.Id;
    }
}
