using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.HR.Queries.GetEmployeeById;

public record GetEmployeeByIdQuery : IRequest<EmployeeDto?>
{
    public int Id { get; init; }
}

public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetEmployeeByIdQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<EmployeeDto?> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
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
            return null;
        }

        return new EmployeeDto
        {
            Id = employee.Id,
            Name = employee.Name,
            Email = employee.Email,
            Phone = employee.Phone,
            IdNumber = employee.IdNumber,
            RateType = employee.RateType,
            Rate = employee.Rate,
            IsActive = employee.IsActive,
            Notes = employee.Notes,
            Created = employee.Created
        };
    }
}

public record EmployeeDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? IdNumber { get; init; }
    public Toss.Domain.Enums.EmployeeRateType RateType { get; init; }
    public decimal Rate { get; init; }
    public bool IsActive { get; init; }
    public string? Notes { get; init; }
    public DateTimeOffset Created { get; init; }
}

