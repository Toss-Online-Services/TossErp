using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.HR;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.HR.Commands.RecordClockAttendance;

public record RecordClockAttendanceCommand : IRequest<int>
{
    public int EmployeeId { get; init; }
    public DateTimeOffset ClockIn { get; init; }
    public DateTimeOffset? ClockOut { get; init; }
    public string? Notes { get; init; }
}

public class RecordClockAttendanceCommandHandler : IRequestHandler<RecordClockAttendanceCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public RecordClockAttendanceCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<int> Handle(RecordClockAttendanceCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        // Validate employee exists and is active
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId
                && e.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (employee == null)
        {
            throw new NotFoundException("Employee", request.EmployeeId.ToString());
        }

        if (!employee.IsActive)
        {
            throw new ValidationException("Cannot record attendance for an inactive employee.");
        }

        // Validate clock-out is after clock-in if provided
        if (request.ClockOut.HasValue && request.ClockOut.Value <= request.ClockIn)
        {
            throw new ValidationException("Clock-out time must be after clock-in time.");
        }

        // Check for existing attendance on the same date
        var attendanceDate = request.ClockIn.Date;
        var existingAttendance = await _context.Attendances
            .FirstOrDefaultAsync(a => a.EmployeeId == request.EmployeeId
                && a.AttendanceDate.Date == attendanceDate
                && a.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (existingAttendance != null)
        {
            // Update existing attendance
            existingAttendance.ClockIn = request.ClockIn;
            existingAttendance.ClockOut = request.ClockOut;
            existingAttendance.Notes = request.Notes;
            await _context.SaveChangesAsync(cancellationToken);
            return existingAttendance.Id;
        }

        // Check for overlapping clock intervals (if clock-out is provided)
        if (request.ClockOut.HasValue)
        {
            var overlapping = await _context.Attendances
                .AnyAsync(a => a.EmployeeId == request.EmployeeId
                    && a.BusinessId == _businessContext.CurrentBusinessId!.Value
                    && a.ClockIn.HasValue
                    && a.ClockOut.HasValue
                    && ((a.ClockIn.Value <= request.ClockIn && a.ClockOut.Value > request.ClockIn) ||
                        (a.ClockIn.Value < request.ClockOut.Value && a.ClockOut.Value >= request.ClockOut.Value) ||
                        (a.ClockIn.Value >= request.ClockIn && a.ClockOut.Value <= request.ClockOut.Value)), cancellationToken);

            if (overlapping)
            {
                throw new ValidationException("Clock-in/out times overlap with existing attendance record.");
            }
        }

        var attendance = new Attendance
        {
            BusinessId = _businessContext.CurrentBusinessId!.Value,
            EmployeeId = request.EmployeeId,
            AttendanceDate = attendanceDate,
            ClockIn = request.ClockIn,
            ClockOut = request.ClockOut,
            Notes = request.Notes
        };

        _context.Attendances.Add(attendance);
        await _context.SaveChangesAsync(cancellationToken);

        return attendance.Id;
    }
}

