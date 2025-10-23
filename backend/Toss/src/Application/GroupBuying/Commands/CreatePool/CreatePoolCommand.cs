using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.GroupBuying;
using Toss.Domain.Enums;

namespace Toss.Application.GroupBuying.Commands.CreatePool;

public record CreatePoolCommand : IRequest<int>
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int ProductId { get; init; }
    public int InitiatorShopId { get; init; }
    public int SupplierId { get; init; }
    public int MinimumQuantity { get; init; }
    public int? MaximumQuantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal BulkDiscountPercentage { get; init; }
    public DateTimeOffset CloseDate { get; init; }
    public decimal EstimatedShippingCost { get; init; }
    public double? MaxDistanceKm { get; init; }
}

public class CreatePoolCommandHandler : IRequestHandler<CreatePoolCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreatePoolCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreatePoolCommand request, CancellationToken cancellationToken)
    {
        // Validate product exists
        var product = await _context.Products.FindAsync(new object[] { request.ProductId }, cancellationToken);
        if (product == null)
            throw new NotFoundException(nameof(Product), request.ProductId);

        // Validate shop exists
        var shop = await _context.Shops.FindAsync(new object[] { request.InitiatorShopId }, cancellationToken);
        if (shop == null)
            throw new NotFoundException(nameof(Shop), request.InitiatorShopId);

        // Validate supplier exists
        var supplier = await _context.Suppliers.FindAsync(new object[] { request.SupplierId }, cancellationToken);
        if (supplier == null)
            throw new NotFoundException(nameof(Supplier), request.SupplierId);

        var pool = new GroupBuyPool
        {
            PoolNumber = await GeneratePoolNumber(cancellationToken),
            Title = request.Title,
            Description = request.Description,
            ProductId = request.ProductId,
            InitiatorShopId = request.InitiatorShopId,
            SupplierId = request.SupplierId,
            MinimumQuantity = request.MinimumQuantity,
            MaximumQuantity = request.MaximumQuantity,
            CurrentQuantity = 0,
            UnitPrice = request.UnitPrice,
            BulkDiscountPercentage = request.BulkDiscountPercentage,
            FinalUnitPrice = request.UnitPrice * (1 - request.BulkDiscountPercentage / 100),
            OpenDate = DateTimeOffset.UtcNow,
            CloseDate = request.CloseDate,
            Status = PoolStatus.Open,
            AreaGroup = shop.AreaGroup,
            MaxDistanceKm = request.MaxDistanceKm,
            EstimatedShippingCost = request.EstimatedShippingCost
        };

        _context.GroupBuyPools.Add(pool);
        await _context.SaveChangesAsync(cancellationToken);

        return pool.Id;
    }

    private async Task<string> GeneratePoolNumber(CancellationToken cancellationToken)
    {
        var date = DateTimeOffset.UtcNow;
        var count = await _context.GroupBuyPools
            .Where(p => p.OpenDate.Date == date.Date)
            .CountAsync(cancellationToken);

        return $"POOL-{date:yyyyMMdd}-{count + 1:D4}";
    }
}

