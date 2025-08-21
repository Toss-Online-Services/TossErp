using Microsoft.AspNetCore.Mvc;
using MediatR;
using TossErp.HumanResources.Application.Employees.Commands.CreateEmployee;
using TossErp.HumanResources.Application.Employees.Commands.UpdateEmployee;
using TossErp.HumanResources.Application.Employees.Commands.ChangeEmployeeStatus;
using TossErp.HumanResources.Application.Employees.Queries.GetEmployee;
using TossErp.HumanResources.Application.Employees.Queries.GetEmployees;
using TossErp.HumanResources.Domain.Enums;

namespace TossErp.HumanResources.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(IMediator mediator, ILogger<EmployeesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all employees with optional filtering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<EmployeeListDto>>> GetEmployees(
        [FromQuery] string? department = null,
        [FromQuery] EmployeeStatus? status = null,
        [FromQuery] Guid? managerId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetEmployeesQuery(department, status, managerId, page, pageSize);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get employee by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeDto>> GetEmployee(Guid id)
    {
        var query = new GetEmployeeQuery(id);
        var result = await _mediator.Send(query);
        
        if (result == null)
        {
            return NotFound($"Employee with ID {id} not found");
        }

        return Ok(result);
    }

    /// <summary>
    /// Create a new employee
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateEmployee([FromBody] CreateEmployeeRequest request)
    {
        var command = new CreateEmployeeCommand(
            request.EmployeeNumber,
            request.FirstName,
            request.LastName,
            request.DateOfBirth,
            request.Gender,
            request.PhoneNumber,
            request.Email,
            request.Department,
            request.JobTitle,
            request.SalaryAmount,
            request.SalaryCurrency,
            request.HireDate,
            request.HoursPerDay,
            request.DaysPerWeek,
            request.ManagerId,
            request.Address);

        try
        {
            var employeeId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetEmployee), new { id = employeeId }, employeeId);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request for creating employee");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating employee");
            return StatusCode(500, "An error occurred while creating the employee");
        }
    }

    /// <summary>
    /// Update an existing employee
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateEmployee(Guid id, [FromBody] UpdateEmployeeRequest request)
    {
        var command = new UpdateEmployeeCommand(
            id,
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.Email,
            request.Department,
            request.JobTitle,
            request.SalaryAmount,
            request.SalaryCurrency,
            request.HoursPerDay,
            request.DaysPerWeek,
            request.ManagerId,
            request.Address);

        try
        {
            var success = await _mediator.Send(command);
            
            if (!success)
            {
                return NotFound($"Employee with ID {id} not found");
            }

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request for updating employee {EmployeeId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating employee {EmployeeId}", id);
            return StatusCode(500, "An error occurred while updating the employee");
        }
    }

    /// <summary>
    /// Change employee status
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult> ChangeEmployeeStatus(Guid id, [FromBody] ChangeEmployeeStatusRequest request)
    {
        var command = new ChangeEmployeeStatusCommand(id, request.NewStatus, request.Reason);

        try
        {
            var success = await _mediator.Send(command);
            
            if (!success)
            {
                return NotFound($"Employee with ID {id} not found");
            }

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request for changing employee status {EmployeeId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing employee status {EmployeeId}", id);
            return StatusCode(500, "An error occurred while changing the employee status");
        }
    }
}

// DTOs for API requests
public record CreateEmployeeRequest(
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
    string? Address = null);

public record UpdateEmployeeRequest(
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
    string? Address = null);

public record ChangeEmployeeStatusRequest(
    EmployeeStatus NewStatus,
    string? Reason = null);
