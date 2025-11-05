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

        // Unified path (dual-write): ensure SalesDocument is created with the same number
        await _sender.Send(new CreateSalesDocumentCommand
        {
            SaleId = sale.Id,
            DocumentType = SalesDocumentType.Invoice,
            DocumentNumber = invoiceNumber,
            DueDate = request.DueDate,
            Notes = request.Notes
        }, cancellationToken);

        var invoice = new Invoice
        {
            InvoiceNumber = invoiceNumber,
            SaleId = request.SaleId,
            CustomerId = sale.CustomerId ?? throw new InvalidOperationException("Sale must have a customer to create invoice"),
            InvoiceDate = DateTimeOffset.UtcNow,
            DueDate = request.DueDate.HasValue ? request.DueDate.Value.ToUniversalTime() : DateTimeOffset.UtcNow.AddDays(30),
            Subtotal = sale.Subtotal,
            TaxAmount = sale.TaxAmount,
            Total = sale.Total,
            IsPaid = false,
            Notes = request.Notes
        };

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync(cancellationToken);

        return invoice.Id;
    }

    private async Task<string> GenerateInvoiceNumber(int shopId, CancellationToken cancellationToken)
    {
        var year = DateTimeOffset.UtcNow.Year;
        var count = await _context.Invoices
            .Where(i => i.Sale.ShopId == shopId && i.InvoiceDate.Year == year)
            .CountAsync(cancellationToken);

        return $"INV-{year}-{(count + 1):D3}";
    }
}

