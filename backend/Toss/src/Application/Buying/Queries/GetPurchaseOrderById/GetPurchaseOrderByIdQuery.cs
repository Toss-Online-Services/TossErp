using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Orders;

namespace Toss.Application.Buying.Queries.GetPurchaseOrderById;

public record PurchaseOrderDetailDto
{
    public int Id { get; init; }
    public string PONumber { get; init; } = string.Empty;
    public int ShopId { get; init; }
    public string ShopName { get; init; } = string.Empty;
    public int VendorId { get; init; }
    public string VendorName { get; init; } = string.Empty;
    public DateTimeOffset OrderDate { get; init; }
    public DateTimeOffset? RequiredDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public decimal SubTotal { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public List<POItemDto> Items { get; init; } = new();
}

public record POItemDto
{
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
}

public record GetPurchaseOrderByIdQuery : IRequest<PurchaseOrderDetailDto>
{
    public int Id { get; init; }
}

public class GetPurchaseOrderByIdQueryHandler : IRequestHandler<GetPurchaseOrderByIdQuery, PurchaseOrderDetailDto>
{
    private readonly IApplicationDbContext _context;

    public GetPurchaseOrderByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PurchaseOrderDetailDto> Handle(GetPurchaseOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var po = await _context.PurchaseOrders
            .Include(p => p.Shop)
            .Include(p => p.Vendor)
            .Include(p => p.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (po == null)
            throw new NotFoundException(nameof(PurchaseOrder), request.Id.ToString());

        return new PurchaseOrderDetailDto
        {
            Id = po.Id,
            PONumber = po.PONumber,
            ShopId = po.ShopId,
            ShopName = po.Shop.Name,
            VendorId = po.VendorId,
            VendorName = po.Vendor.Name,
            OrderDate = po.OrderDate,
            RequiredDate = po.RequiredDate,
            Status = po.Status.ToString(),
            SubTotal = po.SubTotal,
            TaxAmount = po.TaxAmount,
            TotalAmount = po.TotalAmount,
            Items = po.Items.Select(i => new POItemDto
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

