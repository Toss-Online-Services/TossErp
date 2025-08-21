using MediatR;
using TossErp.HumanResources.Domain.Enums;
using TossErp.HumanResources.Domain.Aggregates.EmployeeAggregate;
using Microsoft.Extensions.Logging;

namespace TossErp.HumanResources.Application.Employees.Queries.GetEmployees;

public record GetEmployeesQuery(
    string? Department = null,
    EmployeeStatus? Status = null,
    Guid? ManagerId = null,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedResult<EmployeeListDto>>;

public record EmployeeListDto(
    Guid Id,
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string Department,
    string JobTitle,
    string Status,
    DateTime HireDate,
    Guid? ManagerId);

public record PagedResult<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);

public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, PagedResult<EmployeeListDto>>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<GetEmployeesQueryHandler> _logger;

    public GetEmployeesQueryHandler(
        IEmployeeRepository employeeRepository,
        ILogger<GetEmployeesQueryHandler> logger)
    {
        _employeeRepository = employeeRepository;
        _logger = logger;
    }

    public async Task<PagedResult<EmployeeListDto>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        // Get employees based on filters
        IEnumerable<Employee> employees;

        if (!string.IsNullOrWhiteSpace(request.Department))
        {
            employees = await _employeeRepository.GetByDepartmentAsync(request.Department, cancellationToken);
        }
        else if (request.ManagerId.HasValue)
        {
            employees = await _employeeRepository.GetByManagerAsync(request.ManagerId.Value, cancellationToken);
        }
        else
        {
            employees = await _employeeRepository.GetActiveEmployeesAsync(cancellationToken);
        }

        // Apply status filter if specified
        if (request.Status.HasValue)
        {
            employees = employees.Where(e => e.Status == request.Status.Value);
        }

        // Convert to DTOs
        var employeeDtos = employees.Select(e => new EmployeeListDto(
            e.Id,
            e.EmployeeNumber.Value,
            e.FirstName,
            e.LastName,
            e.Department,
            e.JobTitle,
            e.Status.ToString(),
            e.HireDate,
            e.ManagerId)).ToList();

        // Apply pagination
        var totalCount = employeeDtos.Count;
        var pagedEmployees = employeeDtos
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        _logger.LogInformation("Retrieved {Count} employees for page {Page}", pagedEmployees.Count, request.Page);

        return new PagedResult<EmployeeListDto>(
            pagedEmployees,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages);
    }
}
