using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Models;
using Toss.Domain.Entities.GroupBuying;
using Toss.Domain.Enums;
using Toss.Domain.Events;

namespace Toss.Application.GroupBuying.Commands.ConfirmPool;

public record ConfirmPoolCommand : IRequest<Result>
{
    public int PoolId { get; init; }
}

public class ConfirmPoolCommandHandler : IRequestHandler<ConfirmPoolCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public ConfirmPoolCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(ConfirmPoolCommand request, CancellationToken cancellationToken)
    {
        var pool = await _context.GroupBuyPools
            .Include(p => p.Participations)
            .FirstOrDefaultAsync(p => p.Id == request.PoolId, cancellationToken);

        if (pool == null)
            return Result.Failure(new[] { "Pool not found" });

        if (pool.Status != PoolStatus.Open && pool.Status != PoolStatus.Pending)
            return Result.Failure(new[] { $"Cannot confirm pool with status: {pool.Status}" });

        if (pool.CurrentQuantity < pool.MinimumQuantity)
            return Result.Failure(new[] { $"Pool has not reached minimum quantity ({pool.CurrentQuantity}/{pool.MinimumQuantity})" });

        // Recalculate shipping share for all participants
        var participantCount = pool.Participations.Count;
        if (participantCount > 0)
        {
            var shippingPerParticipant = pool.EstimatedShippingCost / participantCount;
            foreach (var participation in pool.Participations)
            {
                participation.ShippingShare = shippingPerParticipant;
                participation.Total = participation.Subtotal + shippingPerParticipant;
                participation.IsConfirmed = true;
                participation.ConfirmedDate = DateTimeOffset.UtcNow;
            }
        }

        // Create aggregated purchase order
        var aggregatedPO = new AggregatedPurchaseOrder
        {
            APONumber = await GenerateAPONumber(cancellationToken),
            GroupBuyPoolId = pool.Id,
            VendorId = pool.VendorId,
            TotalQuantity = pool.CurrentQuantity,
            Subtotal = pool.Participations.Sum(p => p.Subtotal),
            TaxAmount = pool.Participations.Sum(p => p.Subtotal) * 0.15m, // 15% VAT
            ShippingCost = pool.EstimatedShippingCost,
            OrderDate = DateTimeOffset.UtcNow,
            ExpectedDeliveryDate = DateTimeOffset.UtcNow.AddDays(7), // Default 7 days
            Status = PurchaseOrderStatus.Pending
        };

        aggregatedPO.Total = aggregatedPO.Subtotal + aggregatedPO.TaxAmount + aggregatedPO.ShippingCost;

        _context.AggregatedPurchaseOrders.Add(aggregatedPO);

        // Update pool status
        pool.Status = PoolStatus.Confirmed;
        pool.ConfirmedDate = DateTimeOffset.UtcNow;
        pool.AggregatedPurchaseOrder = aggregatedPO;

        // Raise domain event
        pool.AddDomainEvent(new PoolConfirmedEvent(pool));

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<string> GenerateAPONumber(CancellationToken cancellationToken)
    {
        var date = DateTimeOffset.UtcNow;
        var count = await _context.AggregatedPurchaseOrders
            .Where(apo => apo.OrderDate.Date == date.Date)
            .CountAsync(cancellationToken);

        return $"APO-{date:yyyyMMdd}-{count + 1:D4}";
    }
}

