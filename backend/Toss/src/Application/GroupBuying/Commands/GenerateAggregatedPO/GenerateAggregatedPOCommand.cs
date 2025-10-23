using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.GroupBuying;
using Toss.Domain.Enums;

namespace Toss.Application.GroupBuying.Commands.GenerateAggregatedPO;

public record GenerateAggregatedPOCommand : IRequest<int>
{
    public int PoolId { get; init; }
}

public class GenerateAggregatedPOCommandHandler : IRequestHandler<GenerateAggregatedPOCommand, int>
{
    private readonly IApplicationDbContext _context;

    public GenerateAggregatedPOCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(GenerateAggregatedPOCommand request, CancellationToken cancellationToken)
    {
        var pool = await _context.GroupBuyPools
            .Include(p => p.Participations)
            .FirstOrDefaultAsync(p => p.Id == request.PoolId, cancellationToken);

        if (pool == null)
            throw new NotFoundException(nameof(GroupBuyPool), request.PoolId.ToString());

        if (pool.Status != PoolStatus.Confirmed)
            throw new InvalidOperationException("Pool must be confirmed before generating aggregated PO");

        // Check if PO already exists
        var existing = await _context.AggregatedPurchaseOrders
            .FirstOrDefaultAsync(apo => apo.PoolId == request.PoolId, cancellationToken);

        if (existing != null)
            return existing.Id;

        // Calculate totals
        var totalQuantity = pool.Participations.Sum(p => p.Quantity);
        var totalAmount = totalQuantity * pool.ProductPrice;

        var aggregatedPO = new AggregatedPurchaseOrder
        {
            PoolId = pool.Id,
            SupplierId = pool.SupplierId,
            OrderDate = DateTime.UtcNow,
            RequiredDate = pool.TargetDate ?? DateTime.UtcNow.AddDays(7),
            TotalQuantity = totalQuantity,
            TotalAmount = totalAmount,
            Status = PurchaseOrderStatus.Pending,
            PONumber = $"AGPO-{DateTime.UtcNow:yyyyMMdd}-{pool.Id:D6}"
        };

        _context.AggregatedPurchaseOrders.Add(aggregatedPO);
        await _context.SaveChangesAsync(cancellationToken);

        return aggregatedPO.Id;
    }
}

