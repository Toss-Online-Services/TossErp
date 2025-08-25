using Crm.Application.DTOs;

namespace Crm.Application.Queries;

public record GetCustomerByIdQuery(Guid CustomerId) : IRequest<CustomerDto?>;

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<GetCustomerByIdQueryHandler> _logger;

    public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository, ILogger<GetCustomerByIdQueryHandler> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<CustomerDto?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting customer with ID: {CustomerId}", request.CustomerId);
        
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        
        if (customer == null)
        {
            _logger.LogWarning("Customer with ID {CustomerId} not found", request.CustomerId);
            return null;
        }
        
        return new CustomerDto
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address,
            DateOfBirth = customer.DateOfBirth,
            Status = customer.Status,
            Segment = customer.Segment,
            CreatedAt = customer.CreatedAt,
            LastPurchaseDate = customer.LastPurchaseDate,
            TotalSpent = customer.TotalSpent,
            PurchaseCount = customer.PurchaseCount,
            LoyaltyPoints = customer.LoyaltyPoints,
            FullName = customer.FullName,
            IsLapsed = customer.IsLapsed,
            IsHighValue = customer.IsHighValue
        };
    }
}
