using Microsoft.EntityFrameworkCore;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Accounting;

namespace Toss.Application.Accounting.Queries.GetCreditors;

public record CreditorInvoiceDto
{
    public int InvoiceId { get; init; }
    public string InvoiceNumber { get; init; } = string.Empty;
    public DateTimeOffset DocumentDate { get; init; }
    public DateTimeOffset? DueDate { get; init; }
    public decimal Amount { get; init; }
    public decimal Balance { get; init; }
    public string Status { get; init; } = string.Empty;
}

public record CreditorVendorDto
{
    public int VendorId { get; init; }
    public string VendorName { get; init; } = string.Empty;
    public decimal Outstanding { get; init; }
    public decimal Overdue { get; init; }
    public List<CreditorInvoiceDto> Invoices { get; init; } = new();
}

public record GetCreditorsQuery : IRequest<List<CreditorVendorDto>>
{
    public int? VendorId { get; init; }
    public bool IncludeInvoices { get; init; } = true;
}

public class GetCreditorsQueryHandler : IRequestHandler<GetCreditorsQuery, List<CreditorVendorDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetCreditorsQueryHandler(IApplicationDbContext context, IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<List<CreditorVendorDto>> Handle(GetCreditorsQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            return new List<CreditorVendorDto>();
        }

        var now = DateTimeOffset.UtcNow;

        var ledgerQuery = _context.VendorLedgerEntries
            .Include(entry => entry.Vendor)
            .Include(entry => entry.PurchaseDocument)
            .Where(entry => entry.BusinessId == _businessContext.CurrentBusinessId);

        if (request.VendorId.HasValue)
        {
            ledgerQuery = ledgerQuery.Where(entry => entry.VendorId == request.VendorId.Value);
        }

        var grouped = await ledgerQuery
            .GroupBy(entry => new { entry.VendorId, entry.Vendor.Name })
            .Select(group => new CreditorVendorDto
            {
                VendorId = group.Key.VendorId,
                VendorName = group.Key.Name,
                Outstanding = group.Sum(e => e.Balance),
                Overdue = group.Where(e => e.DueDate.HasValue && e.DueDate < now).Sum(e => e.Balance),
                Invoices = request.IncludeInvoices
                    ? group
                        .OrderByDescending(e => e.DocumentDate)
                        .Select(e => new CreditorInvoiceDto
                        {
                            InvoiceId = e.PurchaseDocumentId,
                            InvoiceNumber = e.ReferenceNumber,
                            DocumentDate = e.DocumentDate,
                            DueDate = e.DueDate,
                            Amount = e.Amount,
                            Balance = e.Balance,
                            Status = e.Status.ToString().ToLowerInvariant()
                        })
                        .ToList()
                    : new List<CreditorInvoiceDto>()
            })
            .Where(dto => dto.Outstanding > 0)
            .OrderByDescending(dto => dto.Outstanding)
            .ToListAsync(cancellationToken);

        return grouped;
    }
}

