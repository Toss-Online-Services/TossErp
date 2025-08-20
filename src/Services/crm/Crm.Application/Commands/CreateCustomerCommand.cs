namespace Crm.Application.Commands;

public record CreateCustomerCommand : IRequest<Guid>
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
}

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Guid>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<CreateCustomerCommandHandler> _logger;

    public CreateCustomerCommandHandler(ICustomerRepository customerRepository, ILogger<CreateCustomerCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating customer with email: {Email}", request.Email);

        // Check if email already exists
        if (await _customerRepository.EmailExistsAsync(request.Email, cancellationToken))
        {
            throw new InvalidOperationException($"Customer with email {request.Email} already exists");
        }

        var customer = new Customer(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.Address,
            request.DateOfBirth
        );

        await _customerRepository.AddAsync(customer, cancellationToken);

        _logger.LogInformation("Customer created with ID: {CustomerId}", customer.Id);
        return customer.Id;
    }
}
