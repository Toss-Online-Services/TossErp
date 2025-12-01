using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;

namespace Toss.Domain.Entities.HR;

/// <summary>
/// Represents an attendance record for an employee
/// Supports both clock-in/out and days-worked tracking
/// </summary>
public class Attendance : BaseAuditableEntity, IBusinessScopedEntity
{
    public Attendance()
    {
        Notes = string.Empty;
    }

    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the employee ID
    /// </summary>
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    /// <summary>
    /// Gets or sets the date of attendance
    /// </summary>
    public DateTimeOffset AttendanceDate { get; set; }

    /// <summary>
    /// Gets or sets the clock-in time (optional, for hourly tracking)
    /// </summary>
    public DateTimeOffset? ClockIn { get; set; }

    /// <summary>
    /// Gets or sets the clock-out time (optional, for hourly tracking)
    /// </summary>
    public DateTimeOffset? ClockOut { get; set; }

    /// <summary>
    /// Gets or sets the number of days worked (optional, for daily tracking)
    /// </summary>
    public decimal? DaysWorked { get; set; }

    /// <summary>
    /// Gets or sets optional notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Calculates the hours worked from clock-in/out times
    /// </summary>
    public decimal? HoursWorked
    {
        get
        {
            if (ClockIn.HasValue && ClockOut.HasValue && ClockOut.Value > ClockIn.Value)
            {
                return (decimal)(ClockOut.Value - ClockIn.Value).TotalHours;
            }
            return null;
        }
    }
}

