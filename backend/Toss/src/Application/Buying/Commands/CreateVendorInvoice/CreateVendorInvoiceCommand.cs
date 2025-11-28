using Microsoft.EntityFrameworkCore;
using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Accounting;
using Toss.Domain.Entities.Orders;
using Toss.Domain.Enums;

namespace Toss.Application.Buying.Commands.CreateVendorInvoice;

public record CreateVendorInvoiceLineDto
{
    public int PurchaseOrderItemId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TaxRate { get; init; } = 0.15m; // 15% VAT default
    public string? Description { get; init; }
}

public record CreateVendorInvoiceCommand : IRequest<int>
{
    public int PurchaseOrderId { get; init; }
    public string InvoiceNumber { get; init; } = string.Empty;
    public DateTimeOffset InvoiceDate { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DueDate { get; init; }
    public string? Notes { get; init; }
    public List<CreateVendorInvoiceLineDto> Lines { get; init; } = new();
}

public class CreateVendorInvoiceCommandHandler : IRequestHandler<CreateVendorInvoiceCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateVendorInvoiceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateVendorInvoiceCommand request, CancellationToken cancellationToken)
    {
        if (request.Lines == null || request.Lines.Count == 0)
        {
            throw new ValidationException("At least one invoice line is required.");
        }

        var purchaseOrder = await _context.PurchaseOrders
            .Include(po => po.Shop)
            .Include(po => po.Vendor)
            .Include(po => po.Items)
            .FirstOrDefaultAsync(po => po.Id == request.PurchaseOrderId, cancellationToken);

        if (purchaseOrder == null)
        {
            throw new NotFoundException(nameof(PurchaseOrder), request.PurchaseOrderId);
        }

        // Validate duplicate invoice numbers per vendor
        var existingInvoice = await _context.PurchaseDocuments
            .AnyAsync(doc =>
                doc.DocumentType == PurchaseDocumentType.VendorInvoice &&
                doc.DocumentNumber == request.InvoiceNumber &&
                doc.VendorId == purchaseOrder.VendorId,
                cancellationToken);

        if (existingInvoice)
        {
            throw new ValidationException($"Invoice number '{request.InvoiceNumber}' already exists for this vendor.");
        }

        decimal subtotal = 0;
        decimal taxTotal = 0;

        var document = new PurchaseDocument
        {
            DocumentNumber = request.InvoiceNumber,
            DocumentType = PurchaseDocumentType.VendorInvoice,
            PurchaseOrderId = purchaseOrder.Id,
            VendorId = purchaseOrder.VendorId,
            ShopId = purchaseOrder.ShopId,
            DocumentDate = request.InvoiceDate,
            DueDate = request.DueDate,
            Notes = request.Notes,
            IsApproved = true,
            IsMatchedToPO = true,
            IsPaid = false
        };

        foreach (var lineDto in request.Lines)
        {
            var poItem = purchaseOrder.Items?.FirstOrDefault(i => i.Id == lineDto.PurchaseOrderItemId);
            if (poItem == null)
            {
                throw new ValidationException($"Purchase order item {lineDto.PurchaseOrderItemId} was not found on PO {purchaseOrder.PONumber}.");
            }

            if (lineDto.Quantity <= 0)
            {
                throw new ValidationException("Invoice line quantity must be greater than zero.");
            }

            if (lineDto.Quantity > poItem.QuantityReceived)
            {
                throw new ValidationException("Cannot invoice more than the received quantity.");
            }

            var alreadyInvoiced = poItem.QuantityInvoiced;
            var outstanding = poItem.QuantityReceived - alreadyInvoiced;
            if (lineDto.Quantity > outstanding)
            {
                throw new ValidationException("Cannot invoice more than the outstanding quantity.");
            }

            var lineAmount = lineDto.UnitPrice * lineDto.Quantity;
            var taxAmount = decimal.Round(lineAmount * lineDto.TaxRate, 2, MidpointRounding.AwayFromZero);
            var lineTotal = lineAmount + taxAmount;

            var documentLine = new PurchaseDocumentLine
            {
                PurchaseOrderItemId = poItem.Id,
                ProductId = poItem.ProductId,
                Description = lineDto.Description ?? poItem.ProductName,
                Quantity = lineDto.Quantity,
                UnitPrice = lineDto.UnitPrice,
                TaxRate = lineDto.TaxRate,
                TaxAmount = taxAmount,
                LineTotal = lineTotal
            };

            document.Lines.Add(documentLine);

            poItem.QuantityInvoiced += lineDto.Quantity;
            subtotal += lineAmount;
            taxTotal += taxAmount;
        }

        document.Subtotal = subtotal;
        document.TaxAmount = taxTotal;
        document.TotalAmount = subtotal + taxTotal;

        _context.PurchaseDocuments.Add(document);

        var ledgerEntry = new VendorLedgerEntry
        {
            BusinessId = purchaseOrder.Shop.BusinessId,
            VendorId = purchaseOrder.VendorId,
            PurchaseDocument = document,
            Amount = document.TotalAmount,
            PaidAmount = 0,
            Balance = document.TotalAmount,
            DocumentDate = document.DocumentDate,
            DueDate = document.DueDate,
            ReferenceNumber = document.DocumentNumber,
            Status = VendorLedgerStatus.Open
        };

        _context.VendorLedgerEntries.Add(ledgerEntry);

        await _context.SaveChangesAsync(cancellationToken);
        return document.Id;
    }
}
