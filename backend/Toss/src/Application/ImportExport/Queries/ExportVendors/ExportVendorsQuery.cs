using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Vendors;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.ImportExport.Queries.ExportVendors;

public record ExportVendorsQuery : IRequest<byte[]>
{
}

public record VendorExportDto
{
    public string Name { get; init; } = string.Empty;
    public string? ContactName { get; init; }
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string? Area { get; init; }
    public string? PaymentTerms { get; init; }
    public string? Notes { get; init; }
}

public class ExportVendorsQueryHandler : IRequestHandler<ExportVendorsQuery, byte[]>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public ExportVendorsQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<byte[]> Handle(ExportVendorsQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var businessId = _businessContext.CurrentBusinessId!.Value;

        var vendors = await _context.Vendors
            .Where(v => v.BusinessId == businessId)
            .Select(v => new VendorExportDto
            {
                Name = v.Name,
                ContactName = v.ContactPerson,
                Phone = v.ContactPhone != null ? v.ContactPhone.ToString() : null,
                Email = v.Email,
                Area = null, // Vendor doesn't have Area property
                PaymentTerms = v.PaymentTermsDays.ToString(),
                Notes = v.Description
            })
            .ToListAsync(cancellationToken);

        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Encoding = Encoding.UTF8
        });

        csv.WriteRecords(vendors);
        await writer.FlushAsync(cancellationToken);

        return memoryStream.ToArray();
    }
}
