using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.HR;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.HR.Commands.CreateEmployee;

public record CreateEmployeeCommand : IRequest<int>
{
    public string Name { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? IdNumber { get; init; }
    public EmployeeRateType RateType { get; init; } = EmployeeRateType.Hourly;
    public decimal Rate { get; init; }
    public string? Notes { get; init; }
}

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public CreateEmployeeCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<int> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ValidationException("Employee name is required.");
        }

        if (request.Rate <= 0)
        {
            throw new ValidationException("Employee rate must be greater than zero.");
        }

        var employee = new Employee
        {
            BusinessId = _businessContext.CurrentBusinessId!.Value,
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            IdNumber = request.IdNumber,
            RateType = request.RateType,
            Rate = request.Rate,
            IsActive = true,
            Notes = request.Notes
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync(cancellationToken);

        return employee.Id;
    }
}

