using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;

namespace Toss.Domain.Entities.HR;

/// <summary>
/// Represents a payroll run for an employee for a specific period
/// </summary>
public class PayrollRun : BaseAuditableEntity, IBusinessScopedEntity
{
    public PayrollRun()
    {
    }

    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the employee ID
    /// </summary>
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    /// <summary>
    /// Gets or sets the period start date
    /// </summary>
    public DateTimeOffset PeriodStart { get; set; }

    /// <summary>
    /// Gets or sets the period end date
    /// </summary>
    public DateTimeOffset PeriodEnd { get; set; }

    /// <summary>
    /// Gets or sets the gross pay amount
    /// </summary>
    public decimal Gross { get; set; }

    /// <summary>
    /// Gets or sets the total deductions amount
    /// </summary>
    public decimal Deductions { get; set; }

    /// <summary>
    /// Gets or sets the net pay amount (Gross - Deductions)
    /// </summary>
    public decimal Net { get; set; }

    /// <summary>
    /// Gets or sets when the payroll was generated
    /// </summary>
    public DateTimeOffset GeneratedAt { get; set; }

    /// <summary>
    /// Gets or sets optional notes
    /// </summary>
    public string? Notes { get; set; }
}

