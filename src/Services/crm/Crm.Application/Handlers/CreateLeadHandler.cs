using MediatR;
using Crm.Application.Commands;
using TossErp.CRM.Domain.Aggregates;
using TossErp.CRM.Domain.Repositories;
using TossErp.CRM.Domain.ValueObjects;

namespace Crm.Application.Handlers;

/// <summary>
/// Handler for creating new leads
/// </summary>
public class CreateLeadHandler : IRequestHandler<CreateLeadCommand, Guid>
{
    private readonly ILeadRepository _leadRepository;

    public CreateLeadHandler(ILeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<Guid> Handle(CreateLeadCommand request, CancellationToken cancellationToken)
    {
        // Check if lead with this email already exists
        if (await _leadRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            throw new InvalidOperationException($"Lead with email {request.Email} already exists");
        }

        // Create lead aggregate
        var lead = new Lead(
            id: Guid.NewGuid(),
            tenantId: Guid.Empty, // TODO: Get from user context
            firstName: request.FirstName,
            lastName: request.LastName,
            company: request.Company,
            email: new EmailAddress(request.Email),
            source: request.Source,
            createdBy: "System", // TODO: Get from user context
            jobTitle: request.JobTitle,
            phone: !string.IsNullOrEmpty(request.Phone) ? new PhoneNumber(request.Phone) : null,
            industry: request.Industry,
            companySize: request.CompanySize,
            campaignName: request.CampaignName
        );

        // Save lead
        await _leadRepository.AddAsync(lead, cancellationToken);

        return lead.Id;
    }
}
