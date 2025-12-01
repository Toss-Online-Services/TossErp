using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.HR.Queries.GetPayrollRuns;

public record GetPayrollRunsQuery : IRequest<PaginatedList<PayrollRunDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public int? EmployeeId { get; init; }
    public DateTimeOffset? FromDate { get; init; }
    public DateTimeOffset? ToDate { get; init; }
}

public class GetPayrollRunsQueryHandler : IRequestHandler<GetPayrollRunsQuery, PaginatedList<PayrollRunDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetPayrollRunsQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<PaginatedList<PayrollRunDto>> Handle(GetPayrollRunsQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var query = _context.PayrollRuns
            .Include(r => r.Employee)
            .Where(r => r.BusinessId == _businessContext.CurrentBusinessId!.Value)
            .AsQueryable();

        if (request.EmployeeId.HasValue)
        {
            query = query.Where(r => r.EmployeeId == request.EmployeeId.Value);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(r => r.PeriodStart >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(r => r.PeriodEnd <= request.ToDate.Value);
        }

        query = query.OrderByDescending(r => r.GeneratedAt);

        var payrollQuery = query
            .Select(r => new PayrollRunDto
            {
                Id = r.Id,
                EmployeeId = r.EmployeeId,
                EmployeeName = r.Employee.Name,
                PeriodStart = r.PeriodStart,
                PeriodEnd = r.PeriodEnd,
                Gross = r.Gross,
                Deductions = r.Deductions,
                Net = r.Net,
                GeneratedAt = r.GeneratedAt,
                Notes = r.Notes
            });

        return await PaginatedList<PayrollRunDto>.CreateAsync(payrollQuery, request.PageNumber, request.PageSize, cancellationToken);
    }
}

public record PayrollRunDto
{
    public int Id { get; init; }
    public int EmployeeId { get; init; }
    public string EmployeeName { get; init; } = string.Empty;
    public DateTimeOffset PeriodStart { get; init; }
    public DateTimeOffset PeriodEnd { get; init; }
    public decimal Gross { get; init; }
    public decimal Deductions { get; init; }
    public decimal Net { get; init; }
    public DateTimeOffset GeneratedAt { get; init; }
    public string? Notes { get; init; }
}

