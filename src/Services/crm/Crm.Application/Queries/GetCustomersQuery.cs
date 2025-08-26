using Crm.Application.DTOs;
using Crm.Application.Interfaces;

namespace Crm.Application.Queries;

public record GetCustomersQuery : IRequest<IEnumerable<CustomerDto>>
{
    public string? SearchTerm { get; init; }
    public CustomerSegment? Segment { get; init; }
}

public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, IEnumerable<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<GetCustomersQueryHandler> _logger;

    public GetCustomersQueryHandler(ICustomerRepository customerRepository, ILogger<GetCustomersQueryHandler> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving customers with search term: {SearchTerm}, segment: {Segment}", 
            request.SearchTerm, request.Segment);

        IEnumerable<Customer> customers;

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            customers = await _customerRepository.SearchAsync(request.SearchTerm, cancellationToken);
        }
        else if (request.Segment.HasValue)
        {
            customers = await _customerRepository.GetBySegmentAsync(request.Segment.Value, cancellationToken);
        }
        else
        {
            customers = await _customerRepository.GetAllAsync(cancellationToken);
        }

        return customers.Select(customer => new CustomerDto
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            FullName = customer.FullName,
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
            IsLapsed = customer.IsLapsed,
            IsHighValue = customer.IsHighValue
        });
    }
}