using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TossErp.Domain.Entities.HR;
using TossErp.Infrastructure.Data;

namespace TossErp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HRController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HRController> _logger;

    public HRController(ApplicationDbContext context, ILogger<HRController> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region Employees

    /// <summary>
    /// Get all employees
    /// </summary>
    [HttpGet("employees")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees(
        [FromQuery] EmploymentStatus? status = null,
        [FromQuery] int? departmentId = null,
        [FromQuery] string? search = null)
    {
        var query = _context.Employees.AsQueryable();

        if (status.HasValue)
            query = query.Where(e => e.Status == status.Value);

        if (departmentId.HasValue)
            query = query.Where(e => e.DepartmentId == departmentId.Value);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(e =>
                e.FirstName.Contains(search) ||
                e.LastName.Contains(search) ||
                e.EmployeeNumber.Contains(search) ||
                (e.Email != null && e.Email.Contains(search)));
        }

        var employees = await query
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync();

        return Ok(employees);
    }

    /// <summary>
    /// Get employee by ID
    /// </summary>
    [HttpGet("employees/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Employee>> GetEmployee(int id)
    {
        var employee = await _context.Employees
            .Include(e => e.LeaveRequests.OrderByDescending(l => l.StartDate).Take(10))
            .Include(e => e.AttendanceRecords.OrderByDescending(a => a.Date).Take(30))
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
            return NotFound();

        return Ok(employee);
    }

    /// <summary>
    /// Create a new employee
    /// </summary>
    [HttpPost("employees")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Employee>> CreateEmployee([FromBody] CreateEmployeeRequest request)
    {
        try
        {
            var employee = new Employee
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                MiddleName = request.MiddleName,
                EmployeeNumber = $"EMP-{DateTime.UtcNow:yyyyMMddHHmmss}",
                IdNumber = request.IdNumber,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                Email = request.Email,
                Phone = request.Phone,
                AddressLine1 = request.AddressLine1,
                AddressLine2 = request.AddressLine2,
                City = request.City,
                Province = request.Province,
                PostalCode = request.PostalCode,
                Type = request.Type,
                DepartmentId = request.DepartmentId,
                DepartmentName = request.DepartmentName,
                JobTitle = request.JobTitle,
                HireDate = request.HireDate,
                Salary = request.Salary,
                SalaryFrequency = request.SalaryFrequency,
                PaymentMethod = request.PaymentMethod,
                BankName = request.BankName,
                BankAccountNumber = request.BankAccountNumber,
                AnnualLeaveDays = request.AnnualLeaveDays,
                SickLeaveDays = request.SickLeaveDays,
                EmergencyContactName = request.EmergencyContactName,
                EmergencyContactPhone = request.EmergencyContactPhone,
                EmergencyContactRelationship = request.EmergencyContactRelationship,
                ManagerId = request.ManagerId,
                CreatedBy = User.Identity?.Name ?? "System"
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created employee {EmployeeName} ({EmployeeNumber})", employee.FullName, employee.EmployeeNumber);

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating employee");
            return BadRequest(new { error = "Failed to create employee", message = ex.Message });
        }
    }

    #endregion

    #region Leave Management

    /// <summary>
    /// Get leave requests
    /// </summary>
    [HttpGet("leave")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LeaveRequest>>> GetLeaveRequests(
        [FromQuery] int? employeeId = null,
        [FromQuery] LeaveStatus? status = null)
    {
        var query = _context.LeaveRequests
            .Include(l => l.Employee)
            .AsQueryable();

        if (employeeId.HasValue)
            query = query.Where(l => l.EmployeeId == employeeId.Value);

        if (status.HasValue)
            query = query.Where(l => l.Status == status.Value);

        var requests = await query
            .OrderByDescending(l => l.StartDate)
            .ToListAsync();

        return Ok(requests);
    }

    /// <summary>
    /// Create a leave request
    /// </summary>
    [HttpPost("leave")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LeaveRequest>> CreateLeaveRequest([FromBody] CreateLeaveRequest request)
    {
        try
        {
            var employee = await _context.Employees.FindAsync(request.EmployeeId);
            if (employee == null)
                return BadRequest(new { error = "Employee not found" });

            // Validate leave balance
            employee.RequestLeave(request.Days, request.LeaveType);

            var leaveRequest = new LeaveRequest
            {
                EmployeeId = request.EmployeeId,
                LeaveType = request.LeaveType,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Days = request.Days,
                Reason = request.Reason,
                CreatedBy = User.Identity?.Name ?? "System"
            };

            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created leave request for employee {EmployeeId} for {Days} days", request.EmployeeId, request.Days);

            return CreatedAtAction(nameof(GetLeaveRequests), leaveRequest);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating leave request");
            return BadRequest(new { error = "Failed to create leave request", message = ex.Message });
        }
    }

    #endregion

    #region Attendance

    /// <summary>
    /// Record employee attendance
    /// </summary>
    [HttpPost("attendance")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AttendanceRecord>> RecordAttendance([FromBody] RecordAttendanceRequest request)
    {
        try
        {
            var attendance = new AttendanceRecord
            {
                EmployeeId = request.EmployeeId,
                Date = request.Date,
                CheckInTime = request.CheckInTime,
                CheckOutTime = request.CheckOutTime,
                Status = request.Status,
                Notes = request.Notes,
                CreatedBy = User.Identity?.Name ?? "System"
            };

            // Calculate hours worked
            if (attendance.CheckInTime.HasValue && attendance.CheckOutTime.HasValue)
            {
                var duration = attendance.CheckOutTime.Value - attendance.CheckInTime.Value;
                attendance.HoursWorked = (decimal)duration.TotalHours;
                
                // Simple overtime calculation (over 8 hours)
                if (attendance.HoursWorked > 8)
                    attendance.OvertimeHours = attendance.HoursWorked - 8;
            }

            _context.AttendanceRecords.Add(attendance);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Recorded attendance for employee {EmployeeId} on {Date}", request.EmployeeId, request.Date);

            return CreatedAtAction(nameof(GetLeaveRequests), attendance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording attendance");
            return BadRequest(new { error = "Failed to record attendance", message = ex.Message });
        }
    }

    /// <summary>
    /// Get attendance records
    /// </summary>
    [HttpGet("attendance")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AttendanceRecord>>> GetAttendance(
        [FromQuery] int? employeeId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var query = _context.AttendanceRecords
            .Include(a => a.Employee)
            .AsQueryable();

        if (employeeId.HasValue)
            query = query.Where(a => a.EmployeeId == employeeId.Value);

        if (startDate.HasValue)
            query = query.Where(a => a.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(a => a.Date <= endDate.Value);

        var records = await query
            .OrderByDescending(a => a.Date)
            .ToListAsync();

        return Ok(records);
    }

    #endregion
}

// Request DTOs
public record CreateEmployeeRequest(
    string FirstName,
    string LastName,
    string? MiddleName,
    string? IdNumber,
    DateTime? DateOfBirth,
    string? Gender,
    string? Email,
    string? Phone,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? Province,
    string? PostalCode,
    EmploymentType Type,
    int? DepartmentId,
    string? DepartmentName,
    string? JobTitle,
    DateTime? HireDate,
    decimal Salary,
    string? SalaryFrequency,
    string? PaymentMethod,
    string? BankName,
    string? BankAccountNumber,
    decimal AnnualLeaveDays,
    decimal SickLeaveDays,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    string? EmergencyContactRelationship,
    int? ManagerId
);

public record CreateLeaveRequest(
    int EmployeeId,
    string LeaveType,
    DateTime StartDate,
    DateTime EndDate,
    decimal Days,
    string? Reason
);

public record RecordAttendanceRequest(
    int EmployeeId,
    DateTime Date,
    DateTime? CheckInTime,
    DateTime? CheckOutTime,
    string Status,
    string? Notes
);

