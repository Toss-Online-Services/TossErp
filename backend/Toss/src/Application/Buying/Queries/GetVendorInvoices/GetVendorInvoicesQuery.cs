using Microsoft.EntityFrameworkCore;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Models;
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

        var items = await query
            .OrderByDescending(d => d.DocumentDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(d => new VendorInvoiceDto
            {
                Id = d.Id,
                InvoiceNumber = d.DocumentNumber,
                PurchaseOrderId = d.PurchaseOrderId,
                VendorId = d.VendorId,
                Vendor = d.Vendor.Name,
                InvoiceDate = d.DocumentDate,
                DueDate = d.DueDate,
                Subtotal = d.Subtotal,
                TaxAmount = d.TaxAmount,
                Total = d.TotalAmount,
                Status = DetermineStatus(d),
                IsPaid = d.IsPaid,
                PaidDate = d.PaidDate,
                Notes = d.Notes
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
