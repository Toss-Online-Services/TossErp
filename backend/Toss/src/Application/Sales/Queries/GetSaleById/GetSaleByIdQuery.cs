using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Sales;

namespace Toss.Application.Sales.Queries.GetSaleById;

public record SaleDetailDto
{
    public int Id { get; init; }
    public string SaleNumber { get; init; } = string.Empty;
    public int ShopId { get; init; }
    public string ShopName { get; init; } = string.Empty;
    public DateTimeOffset SaleDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public decimal Subtotal { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public string PaymentMethod { get; init; } = string.Empty;
    public int? CustomerId { get; init; }
    public List<SaleItemDto> Items { get; init; } = new();
}

public record SaleItemDto
{
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
}

public record GetSaleByIdQuery : IRequest<SaleDetailDto>
{
    public int Id { get; init; }
}

public class GetSaleByIdQueryHandler : IRequestHandler<GetSaleByIdQuery, SaleDetailDto>
{
    private readonly IApplicationDbContext _context;

    public GetSaleByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SaleDetailDto> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
    {
        var sale = await _context.Sales
            .Include(s => s.Shop)
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (sale == null)
            throw new NotFoundException(nameof(Sale), request.Id.ToString());

        return new SaleDetailDto
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            ShopId = sale.ShopId,
            ShopName = sale.Shop.Name,
            SaleDate = sale.SaleDate,
            Status = sale.Status.ToString(),
            Subtotal = sale.Subtotal,
            TaxAmount = sale.TaxAmount,
            DiscountAmount = sale.DiscountAmount,
            TotalAmount = sale.TotalAmount,
            PaymentMethod = sale.PaymentMethod.ToString(),
            CustomerId = sale.CustomerId,
            Items = sale.Items.Select(i => new SaleItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                LineTotal = i.LineTotal
            }).ToList()
        };
    }
}

