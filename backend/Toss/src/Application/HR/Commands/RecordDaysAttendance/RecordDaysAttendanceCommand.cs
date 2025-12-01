using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.HR;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.HR.Commands.RecordDaysAttendance;

public record RecordDaysAttendanceCommand : IRequest<int>
{
    public int EmployeeId { get; init; }
    public DateTimeOffset AttendanceDate { get; init; }
    public decimal DaysWorked { get; init; }
    public string? Notes { get; init; }
}

public class RecordDaysAttendanceCommandHandler : IRequestHandler<RecordDaysAttendanceCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public RecordDaysAttendanceCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<int> Handle(RecordDaysAttendanceCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        // Validate days worked
        if (request.DaysWorked < 0.5m)
        {
            throw new ValidationException("Days worked must be at least 0.5.");
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

        // Check for existing attendance on the same date
        var attendanceDate = request.AttendanceDate.Date;
        var existingAttendance = await _context.Attendances
            .FirstOrDefaultAsync(a => a.EmployeeId == request.EmployeeId
                && a.AttendanceDate.Date == attendanceDate
                && a.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (existingAttendance != null)
        {
            // Update existing attendance
            existingAttendance.DaysWorked = request.DaysWorked;
            existingAttendance.Notes = request.Notes;
            await _context.SaveChangesAsync(cancellationToken);
            return existingAttendance.Id;
        }

        var attendance = new Attendance
        {
            BusinessId = _businessContext.CurrentBusinessId!.Value,
            EmployeeId = request.EmployeeId,
            AttendanceDate = attendanceDate,
            DaysWorked = request.DaysWorked,
            Notes = request.Notes
        };

        _context.Attendances.Add(attendance);
        await _context.SaveChangesAsync(cancellationToken);

        return attendance.Id;
    }
}

