using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Domain.Enums;
using Toss.Domain.Entities.Orders;

namespace Toss.Application.Buying.Commands.UpdatePurchaseOrderStatus;

public record UpdatePurchaseOrderStatusCommand : IRequest<bool>
{
    public int PurchaseOrderId { get; init; }
    public PurchaseOrderStatus NewStatus { get; init; }
    public string? Notes { get; init; }
}

public class UpdatePurchaseOrderStatusCommandHandler : IRequestHandler<UpdatePurchaseOrderStatusCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdatePurchaseOrderStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdatePurchaseOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var po = await _context.PurchaseOrders
            .FirstOrDefaultAsync(p => p.Id == request.PurchaseOrderId, cancellationToken);

        if (po == null)
            throw new Common.Exceptions.NotFoundException(nameof(PurchaseOrder), request.PurchaseOrderId);

        // Validate status transition
        if (!IsValidStatusTransition(po.Status, request.NewStatus))
        {
            throw new BadRequestException($"Cannot transition from {po.Status} to {request.NewStatus}");
        }

        po.Status = request.NewStatus;
        // LastModified is automatically handled by BaseAuditableEntity interceptor

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static bool IsValidStatusTransition(PurchaseOrderStatus current, PurchaseOrderStatus next)
    {
        // Valid statuses: Draft, Pending, Approved, Confirmed, PartiallyReceived, Received, Cancelled
        return (current, next) switch
        {
            (PurchaseOrderStatus.Draft, PurchaseOrderStatus.Pending) => true,
            (PurchaseOrderStatus.Pending, PurchaseOrderStatus.Approved) => true,
            (PurchaseOrderStatus.Approved, PurchaseOrderStatus.Confirmed) => true,
            (PurchaseOrderStatus.Confirmed, PurchaseOrderStatus.Received) => true,
            (PurchaseOrderStatus.Confirmed, PurchaseOrderStatus.PartiallyReceived) => true,
            (PurchaseOrderStatus.PartiallyReceived, PurchaseOrderStatus.Received) => true,
            (PurchaseOrderStatus.Draft, PurchaseOrderStatus.Cancelled) => true,
            (PurchaseOrderStatus.Pending, PurchaseOrderStatus.Cancelled) => true,
            _ when current == next => true,
            _ => false
        };
    }
}

