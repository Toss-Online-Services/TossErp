using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.HR;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.HR.Commands.RunPayroll;

public record RunPayrollCommand : IRequest<PayrollRunResult>
{
    public DateTimeOffset PeriodStart { get; init; }
    public DateTimeOffset PeriodEnd { get; init; }
    public List<int>? EmployeeIds { get; init; } // If null, run for all active employees
}

public record PayrollRunResult
{
    public int TotalEmployees { get; init; }
    public int ProcessedEmployees { get; init; }
    public decimal TotalGross { get; init; }
    public decimal TotalDeductions { get; init; }
    public decimal TotalNet { get; init; }
    public List<PayrollRunSummaryDto> Runs { get; init; } = new();
}

public record PayrollRunSummaryDto
{
    public int PayrollRunId { get; init; }
    public int EmployeeId { get; init; }
    public string EmployeeName { get; init; } = string.Empty;
    public decimal Gross { get; init; }
    public decimal Deductions { get; init; }
    public decimal Net { get; init; }
}

public class RunPayrollCommandHandler : IRequestHandler<RunPayrollCommand, PayrollRunResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public RunPayrollCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<PayrollRunResult> Handle(RunPayrollCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        // Validate date range
        if (request.PeriodEnd < request.PeriodStart)
        {
            throw new ValidationException("Period end date must be after period start date.");
        }

        var businessId = _businessContext.CurrentBusinessId!.Value;

        // Get employees to process
        var employeesQuery = _context.Employees
            .Where(e => e.BusinessId == businessId && e.IsActive)
            .AsQueryable();

        if (request.EmployeeIds != null && request.EmployeeIds.Any())
        {
            employeesQuery = employeesQuery.Where(e => request.EmployeeIds.Contains(e.Id));
        }

        var employees = await employeesQuery.ToListAsync(cancellationToken);

        if (!employees.Any())
        {
            throw new ValidationException("No active employees found to process payroll.");
        }

        var runs = new List<PayrollRunSummaryDto>();
        decimal totalGross = 0;
        decimal totalDeductions = 0;
        decimal totalNet = 0;
        int processedCount = 0;

        foreach (var employee in employees)
        {
            // Check if payroll run already exists for this period (idempotency)
            var existingRun = await _context.PayrollRuns
                .FirstOrDefaultAsync(r => r.EmployeeId == employee.Id
                    && r.BusinessId == businessId
                    && r.PeriodStart == request.PeriodStart
                    && r.PeriodEnd == request.PeriodEnd, cancellationToken);

            if (existingRun != null)
            {
                // Use existing run
                runs.Add(new PayrollRunSummaryDto
                {
                    PayrollRunId = existingRun.Id,
                    EmployeeId = employee.Id,
                    EmployeeName = employee.Name,
                    Gross = existingRun.Gross,
                    Deductions = existingRun.Deductions,
                    Net = existingRun.Net
                });

                totalGross += existingRun.Gross;
                totalDeductions += existingRun.Deductions;
                totalNet += existingRun.Net;
                processedCount++;
                continue;
            }

            // Calculate payroll for this employee
            var attendanceRecords = await _context.Attendances
                .Where(a => a.EmployeeId == employee.Id
                    && a.BusinessId == businessId
                    && a.AttendanceDate >= request.PeriodStart.Date
                    && a.AttendanceDate <= request.PeriodEnd.Date)
                .ToListAsync(cancellationToken);

            decimal unitsWorked = 0;

            if (employee.RateType == EmployeeRateType.Hourly)
            {
                // Sum hours from clock-in/out or use hours directly if available
                unitsWorked = attendanceRecords
                    .Where(a => a.HoursWorked.HasValue)
                    .Sum(a => a.HoursWorked!.Value);
            }
            else if (employee.RateType == EmployeeRateType.Daily)
            {
                // Sum days worked
                unitsWorked = attendanceRecords
                    .Where(a => a.DaysWorked.HasValue)
                    .Sum(a => a.DaysWorked!.Value);
            }
            else if (employee.RateType == EmployeeRateType.Monthly)
            {
                // For monthly, count days in period (assuming full month)
                var daysInPeriod = (request.PeriodEnd.Date - request.PeriodStart.Date).Days + 1;
                unitsWorked = daysInPeriod;
            }

            // Calculate gross pay
            decimal gross = unitsWorked * employee.Rate;

            // Deductions = 0 for MVP (placeholder for future extension)
            decimal deductions = 0;

            // Calculate net pay
            decimal net = gross - deductions;

            // Create payroll run
            var payrollRun = new PayrollRun
            {
                BusinessId = businessId,
                EmployeeId = employee.Id,
                PeriodStart = request.PeriodStart,
                PeriodEnd = request.PeriodEnd,
                Gross = gross,
                Deductions = deductions,
                Net = net,
                GeneratedAt = DateTimeOffset.UtcNow
            };

            _context.PayrollRuns.Add(payrollRun);
            await _context.SaveChangesAsync(cancellationToken);

            runs.Add(new PayrollRunSummaryDto
            {
                PayrollRunId = payrollRun.Id,
                EmployeeId = employee.Id,
                EmployeeName = employee.Name,
                Gross = gross,
                Deductions = deductions,
                Net = net
            });

            totalGross += gross;
            totalDeductions += deductions;
            totalNet += net;
            processedCount++;
        }

        return new PayrollRunResult
        {
            TotalEmployees = employees.Count,
            ProcessedEmployees = processedCount,
            TotalGross = totalGross,
            TotalDeductions = totalDeductions,
            TotalNet = totalNet,
            Runs = runs
        };
    }
}

