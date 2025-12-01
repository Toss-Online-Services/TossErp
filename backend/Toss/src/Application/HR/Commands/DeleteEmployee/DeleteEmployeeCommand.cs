using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.HR.Commands.DeleteEmployee;

public record DeleteEmployeeCommand : IRequest<bool>
{
    public int Id { get; init; }
}

public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public DeleteEmployeeCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<bool> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == request.Id
                && e.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (employee == null)
        {
            return false;
        }

        // Check if employee has attendance or payroll records
        var hasAttendance = await _context.Attendances
            .AnyAsync(a => a.EmployeeId == request.Id
                && a.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        var hasPayroll = await _context.PayrollRuns
            .AnyAsync(p => p.EmployeeId == request.Id
                && p.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (hasAttendance || hasPayroll)
        {
            // Soft delete: mark as inactive instead of deleting
            employee.IsActive = false;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        // Hard delete if no records exist
        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

