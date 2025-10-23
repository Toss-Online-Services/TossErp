using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Enums;
using Toss.Domain.Events;

namespace Toss.Application.Sales.Commands.CreateSale;

public record CreateSaleCommand : IRequest<int>
{
    public int ShopId { get; init; }
    public int? CustomerId { get; init; }
    public PaymentType PaymentMethod { get; init; }
    public string? PaymentReference { get; init; }
    public string? Notes { get; init; }
    public List<SaleItemDto> Items { get; init; } = new();
}

public record SaleItemDto
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal DiscountAmount { get; init; }
}

public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateSaleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = new Sale
        {
            SaleNumber = await GenerateSaleNumber(request.ShopId, cancellationToken),
            ShopId = request.ShopId,
            CustomerId = request.CustomerId,
            SaleDate = DateTimeOffset.UtcNow,
            Status = SaleStatus.Completed,
            PaymentMethod = request.PaymentMethod,
            PaymentReference = request.PaymentReference,
            Notes = request.Notes
        };

        decimal subtotal = 0;
        decimal taxTotal = 0;

        foreach (var itemDto in request.Items)
        {
            var product = await _context.Products
                .FindAsync(new object[] { itemDto.ProductId }, cancellationToken);

            if (product == null)
                throw new NotFoundException(nameof(Product), itemDto.ProductId);

            var lineTotal = (itemDto.UnitPrice * itemDto.Quantity) - itemDto.DiscountAmount;
            var taxAmount = product.IsTaxable ? lineTotal * 0.15m : 0; // 15% VAT

            var item = new SaleItem
            {
                ProductId = itemDto.ProductId,
                ProductName = product.Name,
                ProductSKU = product.SKU,
                Quantity = itemDto.Quantity,
                UnitPrice = itemDto.UnitPrice,
                DiscountAmount = itemDto.DiscountAmount,
                TaxAmount = taxAmount,
                LineTotal = lineTotal + taxAmount
            };

            sale.Items.Add(item);
            subtotal += lineTotal;
            taxTotal += taxAmount;
        }

        sale.Subtotal = subtotal;
        sale.TaxAmount = taxTotal;
        sale.Total = subtotal + taxTotal;

        sale.AddDomainEvent(new SaleCompletedEvent(sale));

        _context.Sales.Add(sale);
        await _context.SaveChangesAsync(cancellationToken);

        return sale.Id;
    }

    private async Task<string> GenerateSaleNumber(int shopId, CancellationToken cancellationToken)
    {
        var date = DateTimeOffset.UtcNow;
        var count = await _context.Sales
            .Where(s => s.ShopId == shopId && s.SaleDate.Date == date.Date)
            .CountAsync(cancellationToken);

        return $"SAL-{shopId}-{date:yyyyMMdd}-{count + 1:D4}";
    }
}

