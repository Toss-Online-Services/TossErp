using Microsoft.EntityFrameworkCore;
using Toss.Application.Common.Models;
using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Enums;

namespace Toss.Application.Sales.Queries.GetSalesDocuments;

public record SalesDocumentDto
{
    public int Id { get; init; }
    public string DocumentNumber { get; init; } = string.Empty;
    public SalesDocumentType DocumentType { get; init; }
    public int SaleId { get; init; }
    public string? SaleNumber { get; init; }
    public int? CustomerId { get; init; }
    public string? Customer { get; init; }
    public int? ShopId { get; init; }
    public DateTimeOffset DocumentDate { get; init; }
    public DateTimeOffset? DueDate { get; init; }
    public bool IsPaid { get; init; }
    public DateTimeOffset? PaidDate { get; init; }
    public decimal Subtotal { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public string? Notes { get; init; }
}

public record GetSalesDocumentsQuery : IRequest<PaginatedList<SalesDocumentDto>>
{
    public int ShopId { get; init; }
    public SalesDocumentType? Type { get; init; }
    public int? CustomerId { get; init; }
    public DateTimeOffset? FromDate { get; init; }
    public DateTimeOffset? ToDate { get; init; }
    public bool? IsPaid { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}

public class GetSalesDocumentsQueryHandler : IRequestHandler<GetSalesDocumentsQuery, PaginatedList<SalesDocumentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSalesDocumentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<SalesDocumentDto>> Handle(GetSalesDocumentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.SalesDocuments
            .Include(d => d.Sale)
                .ThenInclude(s => s!.Items)
            .Include(d => d.Customer)
            .Where(d => d.ShopId == request.ShopId || d.Sale.ShopId == request.ShopId);

        if (request.Type.HasValue)
        {
            var type = request.Type.Value;
            query = query.Where(d => d.DocumentType == type);
        }

        if (request.CustomerId.HasValue)
        {
            query = query.Where(d => d.CustomerId == request.CustomerId.Value);
        }

        if (request.FromDate.HasValue)
        {
            var fromDate = request.FromDate.Value.Offset != TimeSpan.Zero ? request.FromDate.Value.ToUniversalTime() : request.FromDate.Value;
            query = query.Where(d => d.DocumentDate >= fromDate);
        }

        if (request.ToDate.HasValue)
        {
            var toDate = request.ToDate.Value.Offset != TimeSpan.Zero ? request.ToDate.Value.ToUniversalTime() : request.ToDate.Value;
            query = query.Where(d => d.DocumentDate <= toDate);
        }

        if (request.IsPaid.HasValue)
        {
            query = query.Where(d => d.IsPaid == request.IsPaid.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var docs = await query
            .OrderByDescending(d => d.DocumentDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(d => new SalesDocumentDto
            {
                Id = d.Id,
                DocumentNumber = d.DocumentNumber,
                DocumentType = d.DocumentType,
                SaleId = d.SaleId,
                SaleNumber = d.Sale.SaleNumber,
                CustomerId = d.CustomerId,
                Customer = d.Customer != null ? (d.Customer.FullName ?? d.Customer.Email ?? "Unknown") : null,
                ShopId = d.ShopId,
                DocumentDate = d.DocumentDate,
                DueDate = d.DueDate,
                IsPaid = d.IsPaid,
                PaidDate = d.PaidDate,
                Subtotal = d.Subtotal,
                TaxAmount = d.TaxAmount,
                TotalAmount = d.TotalAmount,
                Notes = d.Notes
            })
            .ToListAsync(cancellationToken);

        return new PaginatedList<SalesDocumentDto>(docs, totalCount, request.PageNumber, request.PageSize);
    }
}
