using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.HR.Queries.GetEmployees;

public record GetEmployeesQuery : IRequest<PaginatedList<EmployeeDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
}

public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, PaginatedList<EmployeeDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetEmployeesQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<PaginatedList<EmployeeDto>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var query = _context.Employees
            .Where(e => e.BusinessId == _businessContext.CurrentBusinessId!.Value)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(e =>
                e.Name.ToLower().Contains(searchTerm) ||
                (e.Email != null && e.Email.ToLower().Contains(searchTerm)) ||
                (e.Phone != null && e.Phone.ToLower().Contains(searchTerm)) ||
                (e.IdNumber != null && e.IdNumber.ToLower().Contains(searchTerm)));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(e => e.IsActive == request.IsActive.Value);
        }

        query = query.OrderBy(e => e.Name);

        var employeeQuery = query
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                Name = e.Name,
                Email = e.Email,
                Phone = e.Phone,
                IdNumber = e.IdNumber,
                RateType = e.RateType,
                Rate = e.Rate,
                IsActive = e.IsActive,
                Notes = e.Notes,
                Created = e.Created
            });

        return await PaginatedList<EmployeeDto>.CreateAsync(employeeQuery, request.PageNumber, request.PageSize, cancellationToken);
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

