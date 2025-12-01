using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;
using Toss.Domain.Enums;

namespace Toss.Domain.Entities.HR;

/// <summary>
/// Represents an employee
/// </summary>
public class Employee : BaseAuditableEntity, IBusinessScopedEntity
{
    public Employee()
    {
        Name = string.Empty;
        RateType = EmployeeRateType.Hourly;
    }

    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the employee name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the employee email (optional)
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the employee phone number (optional)
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets the employee ID number (optional, for South African context)
    /// </summary>
    public string? IdNumber { get; set; }

    /// <summary>
    /// Gets or sets the rate type (Hourly, Daily, Monthly)
    /// </summary>
    public EmployeeRateType RateType { get; set; }

    /// <summary>
    /// Gets or sets the rate amount
    /// </summary>
    public decimal Rate { get; set; }

    /// <summary>
    /// Gets or sets whether the employee is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets optional notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the collection of attendance records
    /// </summary>
    public ICollection<Attendance> AttendanceRecords { get; private set; } = new List<Attendance>();

    /// <summary>
    /// Gets or sets the collection of payroll runs
    /// </summary>
    public ICollection<PayrollRun> PayrollRuns { get; private set; } = new List<PayrollRun>();
}

