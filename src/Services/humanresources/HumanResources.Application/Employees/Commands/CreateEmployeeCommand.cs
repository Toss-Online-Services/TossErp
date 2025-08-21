using MediatR;
using TossErp.HumanResources.Domain.ValueObjects;
using TossErp.HumanResources.Domain.Enums;
using TossErp.HumanResources.Domain.Aggregates.EmployeeAggregate;
using Microsoft.Extensions.Logging;

namespace TossErp.HumanResources.Application.Employees.Commands.CreateEmployee;

public record CreateEmployeeCommand(
    string EmployeeNumber,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string Gender,
    string PhoneNumber,
    string Email,
    string Department,
    string JobTitle,
    decimal SalaryAmount,
    string SalaryCurrency,
    DateTime HireDate,
    int HoursPerDay,
    int DaysPerWeek,
    Guid? ManagerId = null,
    string? Address = null) : IRequest<Guid>;

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Guid>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<CreateEmployeeCommandHandler> _logger;

    public CreateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        ILogger<CreateEmployeeCommandHandler> logger)
    {
        _employeeRepository = employeeRepository;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        // Create value objects
        var employeeNumber = new EmployeeNumber(request.EmployeeNumber);
        var phoneNumber = new PhoneNumber(request.PhoneNumber);
        var email = new EmailAddress(request.Email);
        var salary = new Salary(request.SalaryAmount, request.SalaryCurrency);
        var workHours = new WorkHours(request.HoursPerDay, request.DaysPerWeek);

        Address? address = null;
        if (!string.IsNullOrWhiteSpace(request.Address))
        {
            address = new Address(request.Address, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        // Parse gender enum
        if (!Enum.TryParse<Gender>(request.Gender, true, out var gender))
        {
            throw new ArgumentException($"Invalid gender: {request.Gender}");
        }

        // Create employee aggregate
        var employee = new Employee(
            employeeNumber,
            request.FirstName,
            request.LastName,
            request.DateOfBirth,
            gender,
            phoneNumber,
            email,
            request.Department,
            request.JobTitle,
            salary,
            request.HireDate,
            workHours,
            request.ManagerId,
            address);

        // Add to repository
        await _employeeRepository.AddAsync(employee, cancellationToken);
        await _employeeRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        _logger.LogInformation("Employee created with ID: {EmployeeId}, Number: {EmployeeNumber}", 
            employee.Id, employee.EmployeeNumber.Value);

        return employee.Id;
    }
}

// Repository interface
public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Employee?> GetByEmployeeNumberAsync(EmployeeNumber employeeNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Employee>> GetByDepartmentAsync(string department, CancellationToken cancellationToken = default);
    Task<IEnumerable<Employee>> GetByManagerAsync(Guid managerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Employee>> GetActiveEmployeesAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Employee employee, CancellationToken cancellationToken = default);
    void Update(Employee employee);
    void Remove(Employee employee);
    IUnitOfWork UnitOfWork { get; }
}

// Unit of Work interface
public interface IUnitOfWork
{
    Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
}
