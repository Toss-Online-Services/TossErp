using Microsoft.EntityFrameworkCore;
using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Enums;

namespace Toss.Application.Sales.Commands.CreateSalesDocument;

public record CreateSalesDocumentCommand : IRequest<int>
{
    public int SaleId { get; init; }
    public SalesDocumentType DocumentType { get; init; }
    public string? DocumentNumber { get; init; }
    public DateTimeOffset? DueDate { get; init; }
    public string? Notes { get; init; }
}

public class CreateSalesDocumentCommandHandler : IRequestHandler<CreateSalesDocumentCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateSalesDocumentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateSalesDocumentCommand request, CancellationToken cancellationToken)
    {
        // Check for existing document FIRST (idempotency)
        var existing = await _context.SalesDocuments
            .FirstOrDefaultAsync(d => d.SaleId == request.SaleId && d.DocumentType == request.DocumentType, cancellationToken);
        if (existing != null)
        {
            return existing.Id;
        }

        var sale = await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == request.SaleId, cancellationToken);

        if (sale == null)
            throw new Toss.Application.Common.Exceptions.NotFoundException(nameof(Sale), request.SaleId.ToString());

        // Validation
        if (request.DocumentType == SalesDocumentType.Invoice && !sale.CustomerId.HasValue)
            throw new InvalidOperationException("Sale must have a customer to create invoice");

        var number = request.DocumentNumber ?? await GenerateDocumentNumber(request.DocumentType, sale.ShopId, cancellationToken);

        var isReceipt = request.DocumentType == SalesDocumentType.Receipt;
        var isInvoice = request.DocumentType == SalesDocumentType.Invoice;

        var document = new SalesDocument
        {
            DocumentType = request.DocumentType,
            DocumentNumber = number,
            SaleId = sale.Id,
            CustomerId = sale.CustomerId, // nullable OK for Receipt
            ShopId = sale.ShopId,
            DocumentDate = DateTimeOffset.UtcNow,
            DueDate = isInvoice
                ? (request.DueDate.HasValue ? request.DueDate.Value.ToUniversalTime() : DateTimeOffset.UtcNow.AddDays(30))
                : null,
            Subtotal = sale.Subtotal,
            TaxAmount = sale.TaxAmount,
            TotalAmount = sale.Total,
            IsPaid = isReceipt,
            PaidDate = isReceipt ? DateTimeOffset.UtcNow : null,
            Notes = request.Notes
        };

        _context.SalesDocuments.Add(document);
        await _context.SaveChangesAsync(cancellationToken);

        return document.Id;
    }

    private async Task<string> GenerateDocumentNumber(SalesDocumentType type, int shopId, CancellationToken cancellationToken)
    {
        var year = DateTimeOffset.UtcNow.Year;
        var prefix = type switch
        {
            SalesDocumentType.Invoice => "INV",
            SalesDocumentType.Receipt => "RCT",
            SalesDocumentType.CreditNote => "CRN",
            _ => "DOC"
        };

        var count = await _context.SalesDocuments
            .Where(d => d.ShopId == shopId && d.DocumentType == type && d.DocumentDate.Year == year)
            .CountAsync(cancellationToken);

        return $"{prefix}-{year}-{(count + 1):D4}";
    }
}
