using Crm.Application.DTOs;

using TossErp.CRM.Domain.Repositories;

namespace Crm.Application.Queries;

public record GetTopCustomersQuery(int Count = 10) : IRequest<IEnumerable<CustomerDto>>;

public class GetTopCustomersQueryHandler : IRequestHandler<GetTopCustomersQuery, IEnumerable<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<GetTopCustomersQueryHandler> _logger;

    public GetTopCustomersQueryHandler(ICustomerRepository customerRepository, ILogger<GetTopCustomersQueryHandler> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<CustomerDto>> Handle(GetTopCustomersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting top {Count} customers", request.Count);
        
        var customers = await _customerRepository.GetTopCustomersAsync(request.Count, cancellationToken);
        
        return customers.Select(c => new CustomerDto
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Email = c.Email,
            Phone = c.Phone,
            Address = c.Address,
            DateOfBirth = c.DateOfBirth,
            Status = c.Status,
            Segment = c.Segment,
            CreatedAt = c.CreatedAt,
            LastPurchaseDate = c.LastPurchaseDate,
            TotalSpent = c.TotalSpent,
            PurchaseCount = c.PurchaseCount,
            LoyaltyPoints = c.LoyaltyPoints,
            FullName = c.FullName,
            IsLapsed = c.IsLapsed,
            IsHighValue = c.IsHighValue
        });
    }
}
