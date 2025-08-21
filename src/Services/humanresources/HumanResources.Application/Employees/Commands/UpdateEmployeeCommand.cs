using MediatR;
using TossErp.HumanResources.Domain.ValueObjects;
using TossErp.HumanResources.Domain.Enums;
using TossErp.HumanResources.Domain.Aggregates.EmployeeAggregate;
using Microsoft.Extensions.Logging;

namespace TossErp.HumanResources.Application.Employees.Commands.UpdateEmployee;

public record UpdateEmployeeCommand(
    Guid EmployeeId,
    string? FirstName = null,
    string? LastName = null,
    string? PhoneNumber = null,
    string? Email = null,
    string? Department = null,
    string? JobTitle = null,
    decimal? SalaryAmount = null,
    string? SalaryCurrency = null,
    int? HoursPerDay = null,
    int? DaysPerWeek = null,
    Guid? ManagerId = null,
    string? Address = null) : IRequest<bool>;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, bool>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<UpdateEmployeeCommandHandler> _logger;

    public UpdateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        ILogger<UpdateEmployeeCommandHandler> logger)
    {
        _employeeRepository = employeeRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee == null)
        {
            _logger.LogWarning("Employee not found with ID: {EmployeeId}", request.EmployeeId);
            return false;
        }

        // Update personal information
        if (!string.IsNullOrWhiteSpace(request.FirstName) || !string.IsNullOrWhiteSpace(request.LastName))
        {
            employee.UpdatePersonalInfo(
                request.FirstName ?? employee.FirstName,
                request.LastName ?? employee.LastName);
        }

        // Update contact information
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber) || !string.IsNullOrWhiteSpace(request.Email))
        {
            PhoneNumber? phoneNumber = null;
            EmailAddress? email = null;

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
                phoneNumber = new PhoneNumber(request.PhoneNumber);

            if (!string.IsNullOrWhiteSpace(request.Email))
                email = new EmailAddress(request.Email);

            employee.UpdateContactInfo(phoneNumber, email);
        }

        // Update department
        if (!string.IsNullOrWhiteSpace(request.Department))
        {
            employee.UpdateDepartment(request.Department);
        }

        // Update job title
        if (!string.IsNullOrWhiteSpace(request.JobTitle))
        {
            employee.UpdateJobTitle(request.JobTitle);
        }

        // Update salary
        if (request.SalaryAmount.HasValue && !string.IsNullOrWhiteSpace(request.SalaryCurrency))
        {
            var newSalary = new Salary(request.SalaryAmount.Value, request.SalaryCurrency);
            employee.UpdateSalary(newSalary);
        }

        // Update work hours
        if (request.HoursPerDay.HasValue && request.DaysPerWeek.HasValue)
        {
            var workHours = new WorkHours(request.HoursPerDay.Value, request.DaysPerWeek.Value);
            employee.UpdateWorkHours(workHours);
        }

        // Update manager
        if (request.ManagerId.HasValue)
        {
            employee.UpdateManager(request.ManagerId.Value);
        }

        // Update address
        if (!string.IsNullOrWhiteSpace(request.Address))
        {
            var address = new Address(request.Address, string.Empty, string.Empty, string.Empty, string.Empty);
            employee.UpdateAddress(address);
        }

        _employeeRepository.Update(employee);
        await _employeeRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        _logger.LogInformation("Employee updated with ID: {EmployeeId}", employee.Id);

        return true;
    }
}
