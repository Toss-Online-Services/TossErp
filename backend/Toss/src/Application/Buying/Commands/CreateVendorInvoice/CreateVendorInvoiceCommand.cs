using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Orders;
using Toss.Domain.Enums;

namespace Toss.Application.Buying.Commands.CreateVendorInvoice;

public record CreateVendorInvoiceCommand : IRequest<int>
{
    public int PurchaseOrderId { get; init; }
    public int VendorId { get; init; }
    public int? ShopId { get; init; }
    public string InvoiceNumber { get; init; } = string.Empty;
    public DateTimeOffset InvoiceDate { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DueDate { get; init; }
    public decimal Subtotal { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public string? Notes { get; init; }
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
        var entity = new PurchaseDocument
        {
            DocumentNumber = request.InvoiceNumber,
            DocumentType = PurchaseDocumentType.VendorInvoice,
            PurchaseOrderId = request.PurchaseOrderId,
            VendorId = request.VendorId,
            ShopId = request.ShopId,
            DocumentDate = request.InvoiceDate,
            DueDate = request.DueDate,
            Subtotal = request.Subtotal,
            TaxAmount = request.TaxAmount,
            TotalAmount = request.TotalAmount,
            Notes = request.Notes,
            IsApproved = false,
            IsPaid = false
        };

        _context.PurchaseDocuments.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
