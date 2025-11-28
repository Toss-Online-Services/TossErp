using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Sales.Commands.CreateSale;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Sales.Commands.PosCheckout;

public record PosCheckoutCommand : IRequest<PosCheckoutResult>
{
    public int ShopId { get; init; }
    public int? CustomerId { get; init; }
    public PaymentType PaymentMethod { get; init; }
    public string? PaymentReference { get; init; }
    public string? Notes { get; init; }
    public List<CreateSale.SaleItemDto> Items { get; init; } = new();
    
    // Idempotency support
    public string? IdempotencyKey { get; init; }
}

public record PosCheckoutResult
{
    public int SaleId { get; init; }
    public string SaleNumber { get; init; } = string.Empty;
    public decimal Total { get; init; }
    public int? ReceiptId { get; init; }
    public string? ReceiptNumber { get; init; }
    public bool IsNewSale { get; init; }
}

public class PosCheckoutCommandHandler : IRequestHandler<PosCheckoutCommand, PosCheckoutResult>
{
    private readonly IApplicationDbContext _context;
    private readonly ISender _sender;

    public PosCheckoutCommandHandler(
        IApplicationDbContext context,
        ISender sender)
    {
        _context = context;
        _sender = sender;
    }

    public async Task<PosCheckoutResult> Handle(PosCheckoutCommand request, CancellationToken cancellationToken)
    {
        // Check idempotency if key provided
        if (!string.IsNullOrWhiteSpace(request.IdempotencyKey))
        {
            var existingSale = await _context.Sales
                .Where(s => s.ShopId == request.ShopId 
                    && s.SaleType == SaleType.POS
                    && s.Status == SaleStatus.Completed
                    && s.PaymentReference == request.IdempotencyKey)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingSale != null)
            {
                // Get receipt if exists
                var existingReceipt = await _context.SalesDocuments
                    .Where(d => d.SaleId == existingSale.Id && d.DocumentType == SalesDocumentType.Receipt)
                    .FirstOrDefaultAsync(cancellationToken);

                return new PosCheckoutResult
                {
                    SaleId = existingSale.Id,
                    SaleNumber = existingSale.SaleNumber,
                    Total = existingSale.Total,
                    ReceiptId = existingReceipt?.Id,
                    ReceiptNumber = existingReceipt?.DocumentNumber,
                    IsNewSale = false
                };
            }
        }

        // Create the sale
        var saleId = await _sender.Send(new CreateSaleCommand
        {
            ShopId = request.ShopId,
            CustomerId = request.CustomerId,
            PaymentMethod = request.PaymentMethod,
            PaymentReference = request.IdempotencyKey ?? request.PaymentReference,
            Notes = request.Notes,
            Items = request.Items,
            SaleType = SaleType.POS
        }, cancellationToken);

        // Get the created sale
        var sale = await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == saleId, cancellationToken);

        if (sale == null)
        {
            throw new NotFoundException(nameof(Sale), saleId.ToString());
        }

        // Get receipt if auto-generated
        var receipt = await _context.SalesDocuments
            .Where(d => d.SaleId == sale.Id && d.DocumentType == SalesDocumentType.Receipt)
            .FirstOrDefaultAsync(cancellationToken);

        return new PosCheckoutResult
        {
            SaleId = sale.Id,
            SaleNumber = sale.SaleNumber,
            Total = sale.Total,
            ReceiptId = receipt?.Id,
            ReceiptNumber = receipt?.DocumentNumber,
            IsNewSale = true
        };
    }
}

