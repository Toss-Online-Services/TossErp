using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Exceptions;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Toss.Application.Sales.Commands.CreateSalesDocument;

namespace Toss.Application.Sales.Commands.CreateInvoice;

public record CreateInvoiceCommand : IRequest<int>
{
    public int SaleId { get; init; }
    public string? InvoiceNumber { get; init; }
    public DateTimeOffset? DueDate { get; init; }
    public string? Notes { get; init; }
}

public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ISender _sender;

    public CreateInvoiceCommandHandler(IApplicationDbContext context, ISender sender)
    {
        _context = context;
        _sender = sender;
    }

    public async Task<int> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var sale = await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == request.SaleId, cancellationToken);

        if (sale == null)
            throw new Common.Exceptions.NotFoundException(nameof(Sale), request.SaleId.ToString());

        var invoiceNumber = request.InvoiceNumber ?? await GenerateInvoiceNumber(sale.ShopId, cancellationToken);

        // Create SalesDocument as Invoice - unified approach
        var documentId = await _sender.Send(new CreateSalesDocumentCommand
        {
            SaleId = sale.Id,
            DocumentType = SalesDocumentType.Invoice,
            DocumentNumber = invoiceNumber,
            DueDate = request.DueDate,
            Notes = request.Notes
        }, cancellationToken);

        return documentId;
    }

    private async Task<string> GenerateInvoiceNumber(int shopId, CancellationToken cancellationToken)
    {
        var year = DateTimeOffset.UtcNow.Year;
        var count = await _context.SalesDocuments
            .Where(i => i.DocumentType == SalesDocumentType.Invoice)
            .Where(i => i.Sale.ShopId == shopId && i.DocumentDate.Year == year)
            .CountAsync(cancellationToken);

        return $"INV-{year}-{(count + 1):D3}";
    }
}

