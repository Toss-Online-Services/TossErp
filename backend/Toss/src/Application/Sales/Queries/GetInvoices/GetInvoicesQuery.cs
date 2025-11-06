using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Mappings;
using Toss.Application.Common.Models;
using Toss.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Sales.Queries.GetInvoices;

public record InvoiceDto
{
    public int Id { get; init; }
    public string InvoiceNumber { get; init; } = string.Empty;
    public int SaleId { get; init; }
    public string? SaleNumber { get; init; }
    public int CustomerId { get; init; }
    public string Customer { get; init; } = string.Empty;
    public DateTimeOffset InvoiceDate { get; init; }
    public DateTimeOffset? DueDate { get; init; }
    public decimal Subtotal { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal Total { get; init; }
    public string Status { get; init; } = string.Empty;
    public bool IsPaid { get; init; }
    public DateTimeOffset? PaidDate { get; init; }
    public string? Notes { get; init; }
    public string? OrderNumber { get; init; }
    public List<InvoiceItemDto> InvoiceItems { get; init; } = new();
}

public record InvoiceItemDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string SKU { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal Price { get; init; }
    public decimal Total { get; init; }
    public int? Stock { get; init; }
}

public record GetInvoicesQuery : IRequest<PaginatedList<InvoiceDto>>
{
    public int ShopId { get; init; }
    public string? Status { get; init; }
    public int? CustomerId { get; init; }
    public DateTimeOffset? FromDate { get; init; }
    public DateTimeOffset? ToDate { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}

public class GetInvoicesQueryHandler : IRequestHandler<GetInvoicesQuery, PaginatedList<InvoiceDto>>
{
    private readonly IApplicationDbContext _context;

    public GetInvoicesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<InvoiceDto>> Handle(GetInvoicesQuery request, CancellationToken cancellationToken)
    {
        // Base query without collection includes to avoid duplicate rows in count
        // Only filter by SalesDocument.ShopId, not by Sale.ShopId, to avoid cross-shop document leakage
        var baseQuery = _context.SalesDocuments
            .Where(i => i.DocumentType == Domain.Enums.SalesDocumentType.Invoice)
            .Where(i => i.ShopId == request.ShopId);

        if (request.Status != null)
        {
            baseQuery = request.Status.ToLower() switch
            {
                "draft" => baseQuery.Where(i => !i.IsPaid && i.Sale.Status == Domain.Enums.SaleStatus.Pending),
                "sent" => baseQuery.Where(i => !i.IsPaid && i.Sale.Status == Domain.Enums.SaleStatus.Completed),
                "paid" => baseQuery.Where(i => i.IsPaid),
                "overdue" => baseQuery.Where(i => !i.IsPaid && i.DueDate.HasValue && i.DueDate < DateTimeOffset.UtcNow),
                "cancelled" => baseQuery.Where(i => i.Sale.Status == Domain.Enums.SaleStatus.Voided),
                _ => baseQuery
            };
        }

        if (request.CustomerId.HasValue)
        {
            baseQuery = baseQuery.Where(i => i.CustomerId == request.CustomerId.Value); 
        }

        if (request.FromDate.HasValue)
        {
            // Normalize to UTC for consistent querying
            var fromDate = request.FromDate.Value.Offset != TimeSpan.Zero       
                ? request.FromDate.Value.ToUniversalTime()
                : request.FromDate.Value;
            baseQuery = baseQuery.Where(i => i.DocumentDate >= fromDate);
        }

        if (request.ToDate.HasValue)
        {
            // Normalize to UTC for consistent querying
            var toDate = request.ToDate.Value.Offset != TimeSpan.Zero
                ? request.ToDate.Value.ToUniversalTime()
                : request.ToDate.Value;
            baseQuery = baseQuery.Where(i => i.DocumentDate <= toDate);
        }

        // Count before including collections to avoid duplicate rows
        var totalCount = await baseQuery.CountAsync(cancellationToken);

        // Query with includes for data retrieval
        var query = baseQuery
            .Include(i => i.Sale)
                .ThenInclude(s => s!.Items)
            .Include(i => i.Customer);

        var invoices = await query
            .OrderByDescending(i => i.DocumentDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(i => new InvoiceDto
            {
                Id = i.Id,
                InvoiceNumber = i.DocumentNumber,
                SaleId = i.SaleId,
                SaleNumber = i.Sale.SaleNumber,
                CustomerId = i.CustomerId!.Value,
                Customer = i.Customer!.FullName ?? i.Customer.Email ?? "Unknown",
                InvoiceDate = i.DocumentDate,
                DueDate = i.DueDate,
                Subtotal = i.Subtotal,
                TaxAmount = i.TaxAmount,
                Total = i.TotalAmount,
                Status = DetermineInvoiceStatus(i),
                IsPaid = i.IsPaid,
                PaidDate = i.PaidDate,
                Notes = i.Notes,
                OrderNumber = i.Sale.SaleNumber,
                InvoiceItems = i.Sale.Items.Select(item => new InvoiceItemDto
                {
                    Id = item.Id,
                    Name = item.ProductName,
                    SKU = item.ProductSKU ?? "",
                    Quantity = item.Quantity,
                    Price = item.UnitPrice,
                    Total = item.LineTotal,
                    Stock = null // Would need to lookup current stock
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        return new PaginatedList<InvoiceDto>(invoices, totalCount, request.PageNumber, request.PageSize);
    }

    private static string DetermineInvoiceStatus(SalesDocument invoice)
    {
        if (invoice.IsPaid)
            return "paid";
        
        if (invoice.Sale.Status == Domain.Enums.SaleStatus.Voided)
            return "cancelled";
        
        if (invoice.DueDate.HasValue && invoice.DueDate < DateTimeOffset.UtcNow && !invoice.IsPaid)
            return "overdue";
        
        if (invoice.Sale.Status == Domain.Enums.SaleStatus.Completed)
            return "sent";
        
        return "draft";
    }
}

