using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Sales;

namespace Toss.Application.Sales.Commands.GenerateReceipt;

public record ReceiptDto
{
    public int Id { get; init; }
    public string ReceiptNumber { get; init; } = string.Empty;
    public int SaleId { get; init; }
    public DateTimeOffset IssuedDate { get; init; }
    public decimal TotalAmount { get; init; }
    public List<ReceiptItemDto> Items { get; init; } = new();
}

public record ReceiptItemDto
{
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
}

public record GenerateReceiptCommand : IRequest<ReceiptDto>
{
    public int SaleId { get; init; }
}

public class GenerateReceiptCommandHandler : IRequestHandler<GenerateReceiptCommand, ReceiptDto>
{
    private readonly IApplicationDbContext _context;

    public GenerateReceiptCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReceiptDto> Handle(GenerateReceiptCommand request, CancellationToken cancellationToken)
    {
        var sale = await _context.Sales
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(s => s.Id == request.SaleId, cancellationToken);

        if (sale == null)
            throw new NotFoundException(nameof(Sale), request.SaleId.ToString());

        // Check if receipt already exists
        var existingReceipt = await _context.Receipts
            .FirstOrDefaultAsync(r => r.SaleId == request.SaleId, cancellationToken);

        if (existingReceipt != null)
        {
            // Return existing receipt
            return MapToDto(existingReceipt, sale);
        }

        // Generate new receipt
        var receipt = new Receipt
        {
            SaleId = sale.Id,
            ReceiptNumber = $"RCP-{DateTime.UtcNow:yyyyMMdd}-{sale.Id:D6}",
            IssuedDate = DateTime.UtcNow,
            TotalAmount = sale.TotalAmount,
            ShopId = sale.ShopId
        };

        _context.Receipts.Add(receipt);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(receipt, sale);
    }

    private ReceiptDto MapToDto(Receipt receipt, Sale sale)
    {
        return new ReceiptDto
        {
            Id = receipt.Id,
            ReceiptNumber = receipt.ReceiptNumber,
            SaleId = receipt.SaleId,
            IssuedDate = receipt.IssuedDate,
            TotalAmount = receipt.TotalAmount,
            Items = sale.Items.Select(i => new ReceiptItemDto
            {
                ProductName = i.Product.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                LineTotal = i.LineTotal
            }).ToList()
        };
    }
}

