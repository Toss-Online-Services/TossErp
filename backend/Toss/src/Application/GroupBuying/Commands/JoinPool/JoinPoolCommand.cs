using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities;
using Toss.Domain.Entities.GroupBuying;
using Toss.Domain.Enums;

namespace Toss.Application.GroupBuying.Commands.JoinPool;

public record JoinPoolCommand : IRequest<int>
{
    public int GroupBuyPoolId { get; init; }
    public int ShopId { get; init; }
    public int QuantityCommitted { get; init; }
    public string? Notes { get; init; }
}

public class JoinPoolCommandHandler : IRequestHandler<JoinPoolCommand, int>
{
    private readonly IApplicationDbContext _context;

    public JoinPoolCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(JoinPoolCommand request, CancellationToken cancellationToken)
    {
        var pool = await _context.GroupBuyPools
            .Include(p => p.Participations)
            .FirstOrDefaultAsync(p => p.Id == request.GroupBuyPoolId, cancellationToken);

        if (pool == null)
            throw new NotFoundException(nameof(GroupBuyPool), request.GroupBuyPoolId.ToString());

        if (pool.Status != PoolStatus.Open)
            throw new InvalidOperationException($"Cannot join pool with status: {pool.Status}");

        if (pool.CloseDate < DateTimeOffset.UtcNow)
            throw new InvalidOperationException("Pool has closed");

        // Check if shop already participating
        var existingParticipation = pool.Participations
            .FirstOrDefault(p => p.ShopId == request.ShopId);

        if (existingParticipation != null)
            throw new InvalidOperationException("Shop is already participating in this pool");

        // Check if adding quantity would exceed maximum
        var newTotal = pool.CurrentQuantity + request.QuantityCommitted;
        if (pool.MaximumQuantity.HasValue && newTotal > pool.MaximumQuantity.Value)
            throw new InvalidOperationException($"Adding {request.QuantityCommitted} would exceed pool maximum of {pool.MaximumQuantity.Value}");

        var subtotal = pool.FinalUnitPrice * request.QuantityCommitted;
        var shippingShare = pool.EstimatedShippingCost / (pool.Participations.Count + 1); // Recalculated when pool closes

        var participation = new PoolParticipation
        {
            GroupBuyPoolId = request.GroupBuyPoolId,
            ShopId = request.ShopId,
            QuantityCommitted = request.QuantityCommitted,
            UnitPrice = pool.FinalUnitPrice,
            Subtotal = subtotal,
            ShippingShare = shippingShare,
            Total = subtotal + shippingShare,
            JoinedDate = DateTimeOffset.UtcNow,
            IsConfirmed = false,
            Notes = request.Notes
        };

        // Update pool quantity
        pool.CurrentQuantity += request.QuantityCommitted;

        _context.PoolParticipations.Add(participation);
        await _context.SaveChangesAsync(cancellationToken);

        return participation.Id;
    }
}

