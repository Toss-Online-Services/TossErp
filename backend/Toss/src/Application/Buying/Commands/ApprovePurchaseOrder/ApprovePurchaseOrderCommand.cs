using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Buying;
using Toss.Domain.Enums;

namespace Toss.Application.Buying.Commands.ApprovePurchaseOrder;

public record ApprovePurchaseOrderCommand : IRequest<bool>
{
    public int PurchaseOrderId { get; init; }
    public string ApprovedBy { get; init; } = string.Empty;
}

public class ApprovePurchaseOrderCommandHandler : IRequestHandler<ApprovePurchaseOrderCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public ApprovePurchaseOrderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ApprovePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var po = await _context.PurchaseOrders.FindAsync(new object[] { request.PurchaseOrderId }, cancellationToken);

        if (po == null)
            throw new NotFoundException(nameof(PurchaseOrder), request.PurchaseOrderId.ToString());

        if (po.Status != PurchaseOrderStatus.Pending)
            return false; // Can only approve pending POs

        po.Status = PurchaseOrderStatus.Approved;
        po.ApprovedDate = DateTime.UtcNow;
        po.ApprovedBy = request.ApprovedBy;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

