using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.CRM.Commands.CreateCustomer;
using Toss.Domain.ValueObjects;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.ImportExport.Commands.ImportCustomers;

public record ImportCustomersCommand : IRequest<ImportResult>
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

public class CustomerCsvRecord
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Notes { get; set; }
}

public sealed class CustomerCsvRecordMap : ClassMap<CustomerCsvRecord>
{
    public CustomerCsvRecordMap()
    {
        Map(m => m.FirstName).Name("First Name", "FirstName", "Name");
        Map(m => m.LastName).Name("Last Name", "LastName", "Surname");
        Map(m => m.Phone).Name("Phone", "Phone Number", "Mobile", "Cell");
        Map(m => m.Email).Name("Email", "Email Address");
        Map(m => m.Notes).Name("Notes", "Description", "Remarks");
    }
}

public class ImportCustomersCommandHandler : IRequestHandler<ImportCustomersCommand, ImportResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;
    private readonly ISender _sender;

    public ImportCustomersCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext,
        ISender sender)
    {
        _context = context;
        _businessContext = businessContext;
        _sender = sender;
    }

    public async Task<ImportResult> Handle(ImportCustomersCommand request, CancellationToken cancellationToken)
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

        csv.Context.RegisterClassMap<CustomerCsvRecordMap>();

        await foreach (var record in csv.GetRecordsAsync<CustomerCsvRecord>(cancellationToken))
        {
            rowNumber++;

            try
            {
                if (string.IsNullOrWhiteSpace(record.FirstName) && string.IsNullOrWhiteSpace(record.LastName))
                {
                    errors.Add(new ImportError
                    {
                        RowNumber = rowNumber,
                        Field = "Name",
                        Message = "First name or last name is required"
                    });
                    continue;
                }

                if (string.IsNullOrWhiteSpace(record.Phone))
                {
                    errors.Add(new ImportError
                    {
                        RowNumber = rowNumber,
                        Field = "Phone",
                        Message = "Phone number is required"
                    });
                    continue;
                }

                // Create customer
                var customerId = await _sender.Send(new CreateCustomerCommand
                {
                    FirstName = record.FirstName ?? string.Empty,
                    LastName = record.LastName ?? string.Empty,
                    PhoneNumber = record.Phone,
                    Email = record.Email,
                    Notes = record.Notes,
                    AllowsMarketing = false
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
