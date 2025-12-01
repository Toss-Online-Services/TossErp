using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.HR.Queries.ExportPayroll;

public record ExportPayrollQuery : IRequest<byte[]>
{
    public DateTimeOffset FromDate { get; init; }
    public DateTimeOffset ToDate { get; init; }
    public List<int>? EmployeeIds { get; init; }
}

public class ExportPayrollQueryHandler : IRequestHandler<ExportPayrollQuery, byte[]>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public ExportPayrollQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<byte[]> Handle(ExportPayrollQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        // Validate date range
        if (request.ToDate < request.FromDate)
        {
            throw new ValidationException("To date must be after from date.");
        }

        var query = _context.PayrollRuns
            .Include(r => r.Employee)
            .Where(r => r.BusinessId == _businessContext.CurrentBusinessId!.Value
                && r.PeriodStart >= request.FromDate.Date
                && r.PeriodEnd <= request.ToDate.Date)
            .AsQueryable();

        if (request.EmployeeIds != null && request.EmployeeIds.Any())
        {
            query = query.Where(r => request.EmployeeIds.Contains(r.EmployeeId));
        }

        var payrollRuns = await query
            .OrderBy(r => r.Employee.Name)
            .ThenBy(r => r.PeriodStart)
            .Select(r => new PayrollExportDto
            {
                EmployeeName = r.Employee.Name,
                EmployeeEmail = r.Employee.Email ?? string.Empty,
                EmployeePhone = r.Employee.Phone ?? string.Empty,
                PeriodStart = r.PeriodStart.ToString("yyyy-MM-dd"),
                PeriodEnd = r.PeriodEnd.ToString("yyyy-MM-dd"),
                Gross = r.Gross,
                Deductions = r.Deductions,
                Net = r.Net,
                GeneratedAt = r.GeneratedAt.ToString("yyyy-MM-dd HH:mm:ss")
            })
            .ToListAsync(cancellationToken);

        // Generate CSV
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true
        });

        csv.WriteRecords(payrollRuns);
        await writer.FlushAsync(cancellationToken);

        return memoryStream.ToArray();
    }
}

public record PayrollExportDto
{
    public string EmployeeName { get; init; } = string.Empty;
    public string EmployeeEmail { get; init; } = string.Empty;
    public string EmployeePhone { get; init; } = string.Empty;
    public string PeriodStart { get; init; } = string.Empty;
    public string PeriodEnd { get; init; } = string.Empty;
    public decimal Gross { get; init; }
    public decimal Deductions { get; init; }
    public decimal Net { get; init; }
    public string GeneratedAt { get; init; } = string.Empty;
}

