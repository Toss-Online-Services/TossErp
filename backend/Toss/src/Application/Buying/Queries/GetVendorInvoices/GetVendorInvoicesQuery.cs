using Microsoft.EntityFrameworkCore;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Models;
using Toss.Domain.Entities.Accounting;
using Toss.Domain.Entities.Orders;
using Toss.Domain.Enums;

namespace Toss.Application.Buying.Queries.GetVendorInvoices;

public record VendorInvoiceDto
{
    public int Id { get; init; }
    public string InvoiceNumber { get; init; } = string.Empty;
    public int PurchaseOrderId { get; init; }
    public int VendorId { get; init; }
    public string Vendor { get; init; } = string.Empty;
    public DateTimeOffset InvoiceDate { get; init; }
    public DateTimeOffset? DueDate { get; init; }
    public decimal Subtotal { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal Total { get; init; }
    public string Status { get; init; } = string.Empty;
    public bool IsPaid { get; init; }
    public DateTimeOffset? PaidDate { get; init; }
    public string? Notes { get; init; }
    public decimal Balance { get; init; }
    public string LedgerStatus { get; init; } = string.Empty;
}

public record GetVendorInvoicesQuery : IRequest<PaginatedList<VendorInvoiceDto>>
{
    public int? ShopId { get; init; }
    public string? Status { get; init; }
    public int? VendorId { get; init; }
    public DateTimeOffset? FromDate { get; init; }
    public DateTimeOffset? ToDate { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}

public class GetVendorInvoicesQueryHandler : IRequestHandler<GetVendorInvoicesQuery, PaginatedList<VendorInvoiceDto>>
{
    private readonly IApplicationDbContext _context;

    public GetVendorInvoicesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<VendorInvoiceDto>> Handle(GetVendorInvoicesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.PurchaseDocuments
            .Include(d => d.Vendor)
            .Include(d => d.PurchaseOrder)
            .Where(d => d.DocumentType == PurchaseDocumentType.VendorInvoice);

        if (request.ShopId.HasValue)
        {
            query = query.Where(d => d.ShopId == request.ShopId);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            var status = request.Status!.ToLowerInvariant();
            query = status switch
            {
                "paid" => query.Where(d => d.IsPaid),
                "overdue" => query.Where(d => !d.IsPaid && d.DueDate.HasValue && d.DueDate < DateTimeOffset.UtcNow),
                "approved" => query.Where(d => d.IsApproved && !d.IsPaid),
                "draft" => query.Where(d => !d.IsApproved && !d.IsPaid),
                _ => query
            };
        }

        if (request.VendorId.HasValue)
        {
            query = query.Where(d => d.VendorId == request.VendorId);
        }

        if (request.FromDate.HasValue)
        {
            var fromDate = request.FromDate.Value.Offset != TimeSpan.Zero
                ? request.FromDate.Value.ToUniversalTime()
                : request.FromDate.Value;
            query = query.Where(d => d.DocumentDate >= fromDate);
        }

        if (request.ToDate.HasValue)
        {
            var toDate = request.ToDate.Value.Offset != TimeSpan.Zero
                ? request.ToDate.Value.ToUniversalTime()
                : request.ToDate.Value;
            query = query.Where(d => d.DocumentDate <= toDate);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var pagedDocs = query
            .OrderByDescending(d => d.DocumentDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

        var items = await pagedDocs
            .GroupJoin(_context.VendorLedgerEntries,
                doc => doc.Id,
                ledger => ledger.PurchaseDocumentId,
                (doc, ledgerEntries) => new
                {
                    Document = doc,
                    Ledger = ledgerEntries.FirstOrDefault()
                })
            .Select(x => new VendorInvoiceDto
            {
                Id = x.Document.Id,
                InvoiceNumber = x.Document.DocumentNumber,
                PurchaseOrderId = x.Document.PurchaseOrderId,
                VendorId = x.Document.VendorId,
                Vendor = x.Document.Vendor.Name,
                InvoiceDate = x.Document.DocumentDate,
                DueDate = x.Document.DueDate,
                Subtotal = x.Document.Subtotal,
                TaxAmount = x.Document.TaxAmount,
                Total = x.Document.TotalAmount,
                Status = DetermineStatus(x.Document),
                IsPaid = x.Document.IsPaid,
                PaidDate = x.Document.PaidDate,
                Notes = x.Document.Notes,
                Balance = x.Ledger != null ? x.Ledger.Balance : (x.Document.IsPaid ? 0 : x.Document.TotalAmount),
                LedgerStatus = x.Ledger != null ? x.Ledger.Status.ToString().ToLowerInvariant() : (x.Document.IsPaid ? "settled" : "open")
            })
            .ToListAsync(cancellationToken);

        return new PaginatedList<VendorInvoiceDto>(items, totalCount, request.PageNumber, request.PageSize);
    }

    private static string DetermineStatus(PurchaseDocument d)
    {
        if (d.IsPaid) return "paid";
        if (d.DueDate.HasValue && d.DueDate < DateTimeOffset.UtcNow) return "overdue";
        if (d.IsApproved) return "sent"; // treated as ready/approved for payment
        return "draft";
    }
}
