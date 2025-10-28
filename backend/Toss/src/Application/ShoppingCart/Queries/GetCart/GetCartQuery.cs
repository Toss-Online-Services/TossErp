using Toss.Application.Common.Interfaces;

namespace Toss.Application.ShoppingCart.Queries.GetCart;

public record GetCartQuery : IRequest<GetCartResult>
{
    public string SessionId { get; init; } = string.Empty;
    public int ShopId { get; init; }
}

public record CartItemDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string ProductSku { get; init; } = string.Empty;
    public string? ProductImage { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TaxRate { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal Subtotal { get; init; }
    public decimal Tax { get; init; }
    public decimal Total { get; init; }
    public string? Attributes { get; init; }
}

public record GetCartResult
{
    public List<CartItemDto> Items { get; init; } = new();
    public int TotalItems { get; init; }
    public decimal Subtotal { get; init; }
    public decimal TotalTax { get; init; }
    public decimal TotalDiscount { get; init; }
    public decimal GrandTotal { get; init; }
}

public class GetCartQueryHandler : IRequestHandler<GetCartQuery, GetCartResult>
{
    private readonly IApplicationDbContext _context;

    public GetCartQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetCartResult> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var cartItems = await _context.ShoppingCartItems
            .Include(c => c.Product)
            .Where(c => c.SessionId == request.SessionId && c.ShopId == request.ShopId && c.IsActive)
            .ToListAsync(cancellationToken);

        var items = cartItems.Select(c =>
        {
            var subtotal = c.UnitPrice * c.Quantity - c.DiscountAmount;
            var tax = subtotal * c.TaxRate / 100;
            var total = subtotal + tax;

            return new CartItemDto
            {
                Id = c.Id,
                ProductId = c.ProductId,
                ProductName = c.Product?.Name ?? string.Empty,
                ProductSku = c.Product?.SKU ?? string.Empty,
                ProductImage = null, // TODO: Add image support to Product entity
                Quantity = c.Quantity,
                UnitPrice = c.UnitPrice,
                TaxRate = c.TaxRate,
                DiscountAmount = c.DiscountAmount,
                Subtotal = subtotal,
                Tax = tax,
                Total = total,
                Attributes = c.Attributes
            };
        }).ToList();

        var subtotal = items.Sum(i => i.Subtotal);
        var totalTax = items.Sum(i => i.Tax);
        var totalDiscount = items.Sum(i => i.DiscountAmount);
        var grandTotal = items.Sum(i => i.Total);
        var totalItems = items.Sum(i => i.Quantity);

        return new GetCartResult
        {
            Items = items,
            TotalItems = totalItems,
            Subtotal = subtotal,
            TotalTax = totalTax,
            TotalDiscount = totalDiscount,
            GrandTotal = grandTotal
        };
    }
}

