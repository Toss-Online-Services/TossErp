using MediatR;
using TossErp.HumanResources.Domain.Enums;
using TossErp.HumanResources.Domain.Aggregates.EmployeeAggregate;
using Microsoft.Extensions.Logging;

namespace TossErp.HumanResources.Application.Employees.Queries.GetEmployee;

public record GetEmployeeQuery(Guid EmployeeId) : IRequest<EmployeeDto?>;

public record EmployeeDto(
    Guid Id,
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
    string Status,
    Guid? ManagerId,
    string? Address,
    int AnnualLeaveBalance,
    int SickLeaveBalance,
    DateTime CreatedDate,
    DateTime? LastModifiedDate);

public class GetEmployeeQueryHandler : IRequestHandler<GetEmployeeQuery, EmployeeDto?>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<GetEmployeeQueryHandler> _logger;

    public GetEmployeeQueryHandler(
        IEmployeeRepository employeeRepository,
        ILogger<GetEmployeeQueryHandler> logger)
    {
        _employeeRepository = employeeRepository;
        _logger = logger;
    }

    public async Task<EmployeeDto?> Handle(GetEmployeeQuery request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee == null)
        {
            _logger.LogWarning("Employee not found with ID: {EmployeeId}", request.EmployeeId);
            return null;
        }

        return new EmployeeDto(
            employee.Id,
            employee.EmployeeNumber.Value,
            employee.FirstName,
            employee.LastName,
            employee.DateOfBirth,
            employee.Gender.ToString(),
            employee.PhoneNumber.Value,
            employee.Email.Value,
            employee.Department,
            employee.JobTitle,
            employee.Salary.Amount,
            employee.Salary.Currency,
            employee.HireDate,
            employee.WorkHours.HoursPerDay,
            employee.WorkHours.DaysPerWeek,
            employee.Status.ToString(),
            employee.ManagerId,
            employee.Address?.ToString(),
            employee.AnnualLeaveBalance,
            employee.SickLeaveBalance,
            employee.CreatedDate,
            employee.LastModifiedDate);
    }
}
