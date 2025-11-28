using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Common.Mappings;
using Toss.Application.Common.Models;
using Toss.Domain.Entities.CRM;

namespace Toss.Application.CRM.Queries.GetCustomerInteractions;

public record CustomerInteractionDto
{
    public int Id { get; init; }
    public int CustomerId { get; init; }
    public string InteractionType { get; init; } = string.Empty;
    public string? Subject { get; init; }
    public string? Description { get; init; }
    public DateTimeOffset InteractionDate { get; init; }
    public string? InteractionBy { get; init; }
    public bool RequiresFollowUp { get; init; }
    public DateTimeOffset? FollowUpDate { get; init; }
    public bool IsFollowedUp { get; init; }
}

public record GetCustomerInteractionsQuery : IRequest<PaginatedList<CustomerInteractionDto>>
{
    public int CustomerId { get; init; }
    public string? InteractionType { get; init; }
    public DateTimeOffset? FromDate { get; init; }
    public DateTimeOffset? ToDate { get; init; }
    public bool? RequiresFollowUp { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetCustomerInteractionsQueryHandler : IRequestHandler<GetCustomerInteractionsQuery, PaginatedList<CustomerInteractionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetCustomerInteractionsQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<PaginatedList<CustomerInteractionDto>> Handle(GetCustomerInteractionsQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            return new PaginatedList<CustomerInteractionDto>(new List<CustomerInteractionDto>(), 0, request.PageNumber, request.PageSize);
        }

        var businessId = _businessContext.CurrentBusinessId!.Value;

        // Verify customer belongs to business
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == request.CustomerId, cancellationToken);

        if (customer == null || customer.BusinessId != businessId)
        {
            return new PaginatedList<CustomerInteractionDto>(new List<CustomerInteractionDto>(), 0, request.PageNumber, request.PageSize);
        }

        var query = _context.CustomerInteractions
            .Where(ci => ci.CustomerId == request.CustomerId && ci.BusinessId == businessId);

        if (!string.IsNullOrWhiteSpace(request.InteractionType))
        {
            query = query.Where(ci => ci.InteractionType == request.InteractionType);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(ci => ci.InteractionDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(ci => ci.InteractionDate <= request.ToDate.Value);
        }

        if (request.RequiresFollowUp.HasValue)
        {
            query = query.Where(ci => ci.RequiresFollowUp == request.RequiresFollowUp.Value);
        }

        query = query.OrderByDescending(ci => ci.InteractionDate);

        return await query
            .Select(ci => new CustomerInteractionDto
            {
                Id = ci.Id,
                CustomerId = ci.CustomerId,
                InteractionType = ci.InteractionType,
                Subject = ci.Subject,
                Description = ci.Description,
                InteractionDate = ci.InteractionDate,
                InteractionBy = ci.InteractionBy,
                RequiresFollowUp = ci.RequiresFollowUp,
                FollowUpDate = ci.FollowUpDate,
                IsFollowedUp = ci.IsFollowedUp
            })
            .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}

