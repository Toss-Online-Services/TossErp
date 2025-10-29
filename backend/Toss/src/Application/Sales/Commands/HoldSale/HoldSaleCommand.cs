using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Enums;

namespace Toss.Application.Sales.Commands.HoldSale;

public record HoldSaleCommand : IRequest<int>
{
    public int ShopId { get; init; }
    public int? CustomerId { get; init; }
    public List<SaleItemDto> Items { get; init; } = new();
    public PaymentType PaymentMethod { get; init; }
    public decimal TotalAmount { get; init; }
    public string? Notes { get; init; }
}

public record SaleItemDto
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}

public class HoldSaleCommandHandler : IRequestHandler<HoldSaleCommand, int>
{
    private readonly IApplicationDbContext _context;

    public HoldSaleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(HoldSaleCommand request, CancellationToken cancellationToken)
    {
        // Generate sale number
        var lastSale = await _context.Sales
            .OrderByDescending(s => s.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var saleNumber = $"SALE-{(lastSale?.Id ?? 0) + 1:D6}";

        // Create held sale
        var sale = new Sale
        {
            SaleNumber = saleNumber,
            ShopId = request.ShopId,
            CustomerId = request.CustomerId,
            SaleDate = DateTimeOffset.UtcNow,
            Status = SaleStatus.OnHold,
            PaymentMethod = request.PaymentMethod,
            Subtotal = request.TotalAmount,
            TaxAmount = 0,
            DiscountAmount = 0,
            Total = request.TotalAmount,
            Notes = request.Notes
        };

        // Add items
        foreach (var item in request.Items)
        {
            var saleItem = new SaleItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = item.UnitPrice * item.Quantity,
                TaxAmount = 0,
                DiscountAmount = 0
            };

            sale.Items.Add(saleItem);
        }

        _context.Sales.Add(sale);
        await _context.SaveChangesAsync(cancellationToken);

        return sale.Id;
    }
}

