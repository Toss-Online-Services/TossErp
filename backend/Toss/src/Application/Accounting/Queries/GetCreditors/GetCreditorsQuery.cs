using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Common.Models;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Accounting.Queries.GetCreditors;

public record GetCreditorsQuery : IRequest<PaginatedList<CreditorDto>>
{
    public bool? OverdueOnly { get; init; }
    public int? VendorId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}

public record CreditorDto
{
    public int VendorId { get; init; }
    public string VendorName { get; init; } = string.Empty;
    public string? VendorEmail { get; init; }
    public string? VendorPhone { get; init; }
    public decimal TotalOutstanding { get; init; }
    public int InvoiceCount { get; init; }
    public DateTimeOffset? OldestInvoiceDate { get; init; }
    public DateTimeOffset? LatestDueDate { get; init; }
    public List<OutstandingVendorInvoiceDto> OutstandingInvoices { get; init; } = new();
}

public record OutstandingVendorInvoiceDto
{
    public int LedgerEntryId { get; init; }
    public string InvoiceNumber { get; init; } = string.Empty;
    public DateTimeOffset InvoiceDate { get; init; }
    public DateTimeOffset? DueDate { get; init; }
    public decimal Amount { get; init; }
    public decimal PaidAmount { get; init; }
    public decimal Balance { get; init; }
    public int DaysOverdue { get; init; }
    public bool IsOverdue { get; init; }
}

public class GetCreditorsQueryHandler : IRequestHandler<GetCreditorsQuery, PaginatedList<CreditorDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetCreditorsQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<PaginatedList<CreditorDto>> Handle(GetCreditorsQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var businessId = _businessContext.CurrentBusinessId!.Value;
        var now = DateTimeOffset.UtcNow;

        // Get all open vendor ledger entries (unpaid invoices)
        var ledgerQuery = _context.VendorLedgerEntries
            .Include(e => e.Vendor)
            .Include(e => e.PurchaseDocument)
            .Where(e => e.BusinessId == businessId
                && e.Status == VendorLedgerStatus.Open
                && e.Balance > 0);

        if (request.VendorId.HasValue)
        {
            ledgerQuery = ledgerQuery.Where(e => e.VendorId == request.VendorId.Value);
        }

        if (request.OverdueOnly == true)
        {
            ledgerQuery = ledgerQuery.Where(e => e.DueDate.HasValue && e.DueDate < now);
        }

        // Group by vendor
        var vendorGroups = await ledgerQuery
            .GroupBy(e => new
            {
                VendorId = e.VendorId,
                VendorName = e.Vendor.Name,
                VendorEmail = e.Vendor.Email,
                VendorPhone = e.Vendor.PhoneNumber
            })
            .Select(g => new CreditorDto
            {
                VendorId = g.Key.VendorId,
                VendorName = g.Key.VendorName,
                VendorEmail = g.Key.VendorEmail,
                VendorPhone = g.Key.VendorPhone,
                TotalOutstanding = g.Sum(e => e.Balance),
                InvoiceCount = g.Count(),
                OldestInvoiceDate = g.Min(e => e.DocumentDate),
                LatestDueDate = g.Max(e => e.DueDate),
                OutstandingInvoices = g.Select(e => new OutstandingVendorInvoiceDto
                {
                    LedgerEntryId = e.Id,
                    InvoiceNumber = e.ReferenceNumber,
                    InvoiceDate = e.DocumentDate,
                    DueDate = e.DueDate,
                    Amount = e.Amount,
                    PaidAmount = e.PaidAmount,
                    Balance = e.Balance,
                    DaysOverdue = e.DueDate.HasValue && e.DueDate < now
                        ? (int)(now - e.DueDate.Value).TotalDays
                        : 0,
                    IsOverdue = e.DueDate.HasValue && e.DueDate < now
                }).OrderByDescending(inv => inv.DueDate ?? inv.InvoiceDate).ToList()
            })
            .OrderByDescending(c => c.TotalOutstanding)
            .ToListAsync(cancellationToken);

        // Apply pagination
        var totalCount = vendorGroups.Count;
        var pagedCreditors = vendorGroups
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new PaginatedList<CreditorDto>(pagedCreditors, totalCount, request.PageNumber, request.PageSize);
    }
}
