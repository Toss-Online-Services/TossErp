using Crm.Application.DTOs;
using Crm.Application.Interfaces;

namespace Crm.Application.Queries;

public record GetLapsedCustomersQuery(int DaysThreshold = 90) : IRequest<IEnumerable<CustomerDto>>;

public class GetLapsedCustomersQueryHandler : IRequestHandler<GetLapsedCustomersQuery, IEnumerable<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<GetLapsedCustomersQueryHandler> _logger;

    public GetLapsedCustomersQueryHandler(ICustomerRepository customerRepository, ILogger<GetLapsedCustomersQueryHandler> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<CustomerDto>> Handle(GetLapsedCustomersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting lapsed customers with {DaysThreshold} days threshold", request.DaysThreshold);
        
        var customers = await _customerRepository.GetLapsedCustomersAsync(request.DaysThreshold, cancellationToken);
        
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
