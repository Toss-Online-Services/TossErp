using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.HR.Commands.UpdateEmployee;

public record UpdateEmployeeCommand : IRequest<bool>
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? IdNumber { get; init; }
    public EmployeeRateType? RateType { get; init; }
    public decimal? Rate { get; init; }
    public bool? IsActive { get; init; }
    public string? Notes { get; init; }
}

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public UpdateEmployeeCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<bool> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
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

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            employee.Name = request.Name;
        }

        if (request.Email != null)
        {
            employee.Email = request.Email;
        }

        if (request.Phone != null)
        {
            employee.Phone = request.Phone;
        }

        if (request.IdNumber != null)
        {
            employee.IdNumber = request.IdNumber;
        }

        if (request.RateType.HasValue)
        {
            employee.RateType = request.RateType.Value;
        }

        if (request.Rate.HasValue)
        {
            if (request.Rate.Value <= 0)
            {
                throw new ValidationException("Employee rate must be greater than zero.");
            }
            employee.Rate = request.Rate.Value;
        }

        if (request.IsActive.HasValue)
        {
            employee.IsActive = request.IsActive.Value;
        }

        if (request.Notes != null)
        {
            employee.Notes = request.Notes;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

