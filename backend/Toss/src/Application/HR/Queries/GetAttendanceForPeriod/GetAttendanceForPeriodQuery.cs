using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.HR.Queries.GetAttendanceForPeriod;

public record GetAttendanceForPeriodQuery : IRequest<PaginatedList<AttendanceDto>>
{
    public int EmployeeId { get; init; }
    public DateTimeOffset FromDate { get; init; }
    public DateTimeOffset ToDate { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}

public class GetAttendanceForPeriodQueryHandler : IRequestHandler<GetAttendanceForPeriodQuery, PaginatedList<AttendanceDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetAttendanceForPeriodQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<PaginatedList<AttendanceDto>> Handle(GetAttendanceForPeriodQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        // Validate employee exists
        var employeeExists = await _context.Employees
            .AnyAsync(e => e.Id == request.EmployeeId
                && e.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (!employeeExists)
        {
            throw new NotFoundException("Employee", request.EmployeeId.ToString());
        }

        // Validate date range
        if (request.ToDate < request.FromDate)
        {
            throw new ValidationException("To date must be after from date.");
        }

        var query = _context.Attendances
            .Where(a => a.EmployeeId == request.EmployeeId
                && a.BusinessId == _businessContext.CurrentBusinessId!.Value
                && a.AttendanceDate >= request.FromDate.Date
                && a.AttendanceDate <= request.ToDate.Date)
            .OrderByDescending(a => a.AttendanceDate)
            .AsQueryable();

        var attendanceQuery = query
            .Select(a => new AttendanceDto
            {
                Id = a.Id,
                EmployeeId = a.EmployeeId,
                AttendanceDate = a.AttendanceDate,
                ClockIn = a.ClockIn,
                ClockOut = a.ClockOut,
                HoursWorked = a.HoursWorked,
                DaysWorked = a.DaysWorked,
                Notes = a.Notes,
                Created = a.Created
            });

        return await PaginatedList<AttendanceDto>.CreateAsync(attendanceQuery, request.PageNumber, request.PageSize, cancellationToken);
    }
}

public record AttendanceDto
{
    public int Id { get; init; }
    public int EmployeeId { get; init; }
    public DateTimeOffset AttendanceDate { get; init; }
    public DateTimeOffset? ClockIn { get; init; }
    public DateTimeOffset? ClockOut { get; init; }
    public decimal? HoursWorked { get; init; }
    public decimal? DaysWorked { get; init; }
    public string? Notes { get; init; }
    public DateTimeOffset Created { get; init; }
}

