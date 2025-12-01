using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.CRM;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.ImportExport.Queries.ExportCustomers;

public record ExportCustomersQuery : IRequest<byte[]>
{
}

public record CustomerExportDto
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string? Notes { get; init; }
}

public class ExportCustomersQueryHandler : IRequestHandler<ExportCustomersQuery, byte[]>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public ExportCustomersQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<byte[]> Handle(ExportCustomersQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var businessId = _businessContext.CurrentBusinessId!.Value;

        var customers = await _context.Customers
            .Where(c => c.BusinessId == businessId)
            .Select(c => new CustomerExportDto
            {
                FirstName = c.FirstName,
                LastName = c.LastName,
                Phone = c.Phone != null ? c.Phone.ToString() : null,
                Email = c.Email,
                Notes = c.Notes
            })
            .ToListAsync(cancellationToken);

        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Encoding = Encoding.UTF8
        });

        csv.WriteRecords(customers);
        await writer.FlushAsync(cancellationToken);

        return memoryStream.ToArray();
    }
}
