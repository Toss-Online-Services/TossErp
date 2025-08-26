using TossErp.CRM.Domain.Repositories;

namespace Crm.Application.Commands;

public record UpdateCustomerCommand : IRequest
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
}

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<UpdateCustomerCommandHandler> _logger;

    public UpdateCustomerCommandHandler(ICustomerRepository customerRepository, ILogger<UpdateCustomerCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating customer with ID: {CustomerId}", request.Id);

        var customer = await _customerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (customer == null)
        {
            throw new InvalidOperationException($"Customer with ID {request.Id} not found");
        }

        // Check if email is being changed and if the new email already exists
        if (!customer.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase))
        {
            if (await _customerRepository.EmailExistsAsync(request.Email, cancellationToken))
            {
                throw new InvalidOperationException($"Customer with email {request.Email} already exists");
            }
        }

        customer.UpdateProfile(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.Address
        );

        await _customerRepository.UpdateAsync(customer, cancellationToken);

        _logger.LogInformation("Customer with ID: {CustomerId} updated successfully", request.Id);
    }
}