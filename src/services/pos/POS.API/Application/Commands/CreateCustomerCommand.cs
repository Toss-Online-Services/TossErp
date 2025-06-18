using MediatR;
using POS.Domain.AggregatesModel.CustomerAggregate;
using POS.Domain.SeedWork;

namespace POS.API.Application.Commands;

public record CreateCustomerCommand(string FirstName, string LastName, string Email, string PhoneNumber) : IRequest<Guid>;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Guid>
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly ILogger<CreateCustomerCommandHandler> _logger;

    public CreateCustomerCommandHandler(
        IRepository<Customer> customerRepository,
        ILogger<CreateCustomerCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating customer: {CustomerName}", $"{request.FirstName} {request.LastName}");
        var customer = new Customer(request.FirstName, request.LastName, request.Email, request.PhoneNumber);
        await _customerRepository.AddAsync(customer);
        await _customerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Customer created successfully with ID: {CustomerId}", customer.Id);
        return customer.Id;
    }
} 
