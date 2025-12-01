using Toss.Application.Common.Exceptions;
using Toss.Application.HR.Commands.CreateEmployee;
using Toss.Application.HR.Commands.RecordClockAttendance;
using Toss.Application.HR.Commands.RecordDaysAttendance;
using Toss.Domain.Entities.HR;

namespace Toss.Application.FunctionalTests.HR.Commands;

using static Testing;

public class RecordAttendanceTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRejectClockOutBeforeClockIn()
    {
        await RunAsDefaultUserAsync();

        var employeeId = await SendAsync(new CreateEmployeeCommand
        {
            Name = "Test Employee",
            RateType = Toss.Domain.Enums.EmployeeRateType.Hourly,
            Rate = 50.00m
        });

        var now = DateTimeOffset.UtcNow;

        var command = new RecordClockAttendanceCommand
        {
            EmployeeId = employeeId,
            ClockIn = now.AddHours(2),
            ClockOut = now // Clock-out before clock-in
        };

        await Should.ThrowAsync<ValidationException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldRejectDaysWorkedLessThanHalf()
    {
        await RunAsDefaultUserAsync();

        var employeeId = await SendAsync(new CreateEmployeeCommand
        {
            Name = "Test Employee",
            RateType = Toss.Domain.Enums.EmployeeRateType.Daily,
            Rate = 200.00m
        });

        var command = new RecordDaysAttendanceCommand
        {
            EmployeeId = employeeId,
            AttendanceDate = DateTimeOffset.UtcNow,
            DaysWorked = 0.3m // Less than 0.5
        };

        await Should.ThrowAsync<ValidationException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldRecordClockAttendance()
    {
        await RunAsDefaultUserAsync();

        var employeeId = await SendAsync(new CreateEmployeeCommand
        {
            Name = "Test Employee",
            RateType = Toss.Domain.Enums.EmployeeRateType.Hourly,
            Rate = 50.00m
        });

        var now = DateTimeOffset.UtcNow;

        var command = new RecordClockAttendanceCommand
        {
            EmployeeId = employeeId,
            ClockIn = now.AddHours(-8),
            ClockOut = now,
            Notes = "Full day"
        };

        var attendanceId = await SendAsync(command);

        var attendance = await FindAsync<Attendance>(attendanceId);

        attendance.ShouldNotBeNull();
        attendance!.ClockIn.ShouldNotBeNull();
        attendance.ClockOut.ShouldNotBeNull();
        attendance.HoursWorked.ShouldBe(8.0m);
    }

    [Test]
    public async Task ShouldRecordDaysAttendance()
    {
        await RunAsDefaultUserAsync();

        var employeeId = await SendAsync(new CreateEmployeeCommand
        {
            Name = "Test Employee",
            RateType = Toss.Domain.Enums.EmployeeRateType.Daily,
            Rate = 200.00m
        });

        var command = new RecordDaysAttendanceCommand
        {
            EmployeeId = employeeId,
            AttendanceDate = DateTimeOffset.UtcNow,
            DaysWorked = 1.0m,
            Notes = "Full day"
        };

        var attendanceId = await SendAsync(command);

        var attendance = await FindAsync<Attendance>(attendanceId);

        attendance.ShouldNotBeNull();
        attendance!.DaysWorked.ShouldBe(1.0m);
    }
}

