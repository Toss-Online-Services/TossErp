using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Vendors.Commands.CreateVendor;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace Toss.Application.ImportExport.Commands.ImportVendors;

public record ImportVendorsCommand : IRequest<ImportResult>
{
    public Stream CsvStream { get; init; } = null!;
    public bool SkipHeader { get; init; } = true;
}

public record ImportResult
{
    public int TotalRows { get; init; }
    public int SuccessCount { get; init; }
    public int ErrorCount { get; init; }
    public List<ImportError> Errors { get; init; } = new();
}

public record ImportError
{
    public int RowNumber { get; init; }
    public string? Field { get; init; }
    public string Message { get; init; } = string.Empty;
}

public class VendorCsvRecord
{
    public string? Name { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int? PaymentTermsDays { get; set; }
    public string? Description { get; set; }
}

public sealed class VendorCsvRecordMap : ClassMap<VendorCsvRecord>
{
    public VendorCsvRecordMap()
    {
        Map(m => m.Name).Name("Name", "Vendor Name", "Supplier Name");
        Map(m => m.ContactPerson).Name("Contact Person", "Contact Name", "Contact");
        Map(m => m.Phone).Name("Phone", "Phone Number", "Mobile", "Cell");
        Map(m => m.Email).Name("Email", "Email Address");
        Map(m => m.PaymentTermsDays).Name("Payment Terms Days", "Payment Terms", "Terms Days");
        Map(m => m.Description).Name("Description", "Notes", "Remarks");
    }
}

public class ImportVendorsCommandHandler : IRequestHandler<ImportVendorsCommand, ImportResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;
    private readonly ISender _sender;

    public ImportVendorsCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext,
        ISender sender)
    {
        _context = context;
        _businessContext = businessContext;
        _sender = sender;
    }

    public async Task<ImportResult> Handle(ImportVendorsCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var errors = new List<ImportError>();
        int successCount = 0;
        int rowNumber = request.SkipHeader ? 1 : 0;

        using var reader = new StreamReader(request.CsvStream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null,
            TrimOptions = TrimOptions.Trim
        });

        csv.Context.RegisterClassMap<VendorCsvRecordMap>();

        await foreach (var record in csv.GetRecordsAsync<VendorCsvRecord>(cancellationToken))
        {
            rowNumber++;

            try
            {
                if (string.IsNullOrWhiteSpace(record.Name))
                {
                    errors.Add(new ImportError
                    {
                        RowNumber = rowNumber,
                        Field = "Name",
                        Message = "Vendor name is required"
                    });
                    continue;
                }

                if (string.IsNullOrWhiteSpace(record.Email))
                {
                    errors.Add(new ImportError
                    {
                        RowNumber = rowNumber,
                        Field = "Email",
                        Message = "Email is required"
                    });
                    continue;
                }

                // Create vendor
                var vendorId = await _sender.Send(new CreateVendorCommand
                {
                    Name = record.Name,
                    Email = record.Email,
                    ContactPerson = record.ContactPerson,
                    PhoneNumber = record.Phone,
                    PaymentTermsDays = record.PaymentTermsDays ?? 30,
                    Description = record.Description
                }, cancellationToken);

                successCount++;
            }
            catch (Exception ex)
            {
                errors.Add(new ImportError
                {
                    RowNumber = rowNumber,
                    Message = ex.Message
                });
            }
        }

        return new ImportResult
        {
            TotalRows = rowNumber,
            SuccessCount = successCount,
            ErrorCount = errors.Count,
            Errors = errors
        };
    }
}
