using Toss.Application.Common.Exceptions;
using Toss.Application.HR.Commands.CreateEmployee;
using Toss.Application.HR.Commands.RecordClockAttendance;
using Toss.Application.HR.Commands.RecordDaysAttendance;
using Toss.Application.HR.Commands.RunPayroll;
using Toss.Domain.Entities.HR;
using Toss.Domain.Enums;

namespace Toss.Application.FunctionalTests.HR.Commands;

using static Testing;

public class RunPayrollTests : BaseTestFixture
{
    [Test]
    public async Task ShouldCalculateHourlyPayroll()
    {
        await RunAsDefaultUserAsync();

        // Create hourly employee
        var employeeId = await SendAsync(new CreateEmployeeCommand
        {
            Name = "Hourly Worker",
            RateType = EmployeeRateType.Hourly,
            Rate = 50.00m
        });

        var now = DateTimeOffset.UtcNow;
        var periodStart = now.AddDays(-7);
        var periodEnd = now;

        // Record attendance (8 hours per day for 5 days = 40 hours)
        for (int i = 0; i < 5; i++)
        {
            var date = periodStart.AddDays(i);
            await SendAsync(new RecordClockAttendanceCommand
            {
                EmployeeId = employeeId,
                ClockIn = date.AddHours(8), // 8 AM
                ClockOut = date.AddHours(16) // 4 PM = 8 hours
            });
        }

        // Run payroll
        var result = await SendAsync(new RunPayrollCommand
        {
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            EmployeeIds = new List<int> { employeeId }
        });

        result.ProcessedEmployees.ShouldBe(1);
        result.Runs.Count.ShouldBe(1);
        result.Runs[0].Gross.ShouldBe(2000.00m); // 40 hours * 50 = 2000
        result.Runs[0].Deductions.ShouldBe(0);
        result.Runs[0].Net.ShouldBe(2000.00m);
    }

    [Test]
    public async Task ShouldCalculateDailyPayroll()
    {
        await RunAsDefaultUserAsync();

        // Create daily employee
        var employeeId = await SendAsync(new CreateEmployeeCommand
        {
            Name = "Daily Worker",
            RateType = EmployeeRateType.Daily,
            Rate = 200.00m
        });

        var now = DateTimeOffset.UtcNow;
        var periodStart = now.AddDays(-7);
        var periodEnd = now;

        // Record attendance (5 days)
        for (int i = 0; i < 5; i++)
        {
            var date = periodStart.AddDays(i);
            await SendAsync(new RecordDaysAttendanceCommand
            {
                EmployeeId = employeeId,
                AttendanceDate = date,
                DaysWorked = 1.0m
            });
        }

        // Run payroll
        var result = await SendAsync(new RunPayrollCommand
        {
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            EmployeeIds = new List<int> { employeeId }
        });

        result.ProcessedEmployees.ShouldBe(1);
        result.Runs[0].Gross.ShouldBe(1000.00m); // 5 days * 200 = 1000
        result.Runs[0].Net.ShouldBe(1000.00m);
    }

    [Test]
    public async Task ShouldBeIdempotent()
    {
        await RunAsDefaultUserAsync();

        var employeeId = await SendAsync(new CreateEmployeeCommand
        {
            Name = "Test Employee",
            RateType = EmployeeRateType.Hourly,
            Rate = 50.00m
        });

        var now = DateTimeOffset.UtcNow;
        var periodStart = now.AddDays(-7);
        var periodEnd = now;

        // Record some attendance
        await SendAsync(new RecordClockAttendanceCommand
        {
            EmployeeId = employeeId,
            ClockIn = periodStart.AddHours(8),
            ClockOut = periodStart.AddHours(16)
        });

        // Run payroll twice
        var result1 = await SendAsync(new RunPayrollCommand
        {
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            EmployeeIds = new List<int> { employeeId }
        });

        var result2 = await SendAsync(new RunPayrollCommand
        {
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            EmployeeIds = new List<int> { employeeId }
        });

        // Should return same results (idempotent)
        result1.Runs[0].Gross.ShouldBe(result2.Runs[0].Gross);
        result1.Runs[0].PayrollRunId.ShouldBe(result2.Runs[0].PayrollRunId);
    }
}

