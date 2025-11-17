using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Logistics;
using Toss.Domain.Enums;

namespace Toss.Application.Logistics.Commands.CaptureProofOfDelivery;

public record CaptureProofOfDeliveryCommand : IRequest<int>
{
    public int DeliveryStopId { get; init; }
    public ProofOfDeliveryType ProofType { get; init; }
    public string? ProofData { get; init; } // PIN code, photo URL, or signature data
    public string? RecipientName { get; init; }
    public string? Notes { get; init; }
}

public class CaptureProofOfDeliveryCommandHandler : IRequestHandler<CaptureProofOfDeliveryCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CaptureProofOfDeliveryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CaptureProofOfDeliveryCommand request, CancellationToken cancellationToken)
    {
        var deliveryStop = await _context.DeliveryStops
            .FindAsync(new object[] { request.DeliveryStopId }, cancellationToken);

        if (deliveryStop == null)
            throw new NotFoundException(nameof(DeliveryStop), request.DeliveryStopId.ToString());

        var proofOfDelivery = new ProofOfDelivery
        {
            DeliveryStopId = request.DeliveryStopId,
            ProofType = request.ProofType,
            ProofData = request.ProofData,
            RecipientName = request.RecipientName,
            Notes = request.Notes,
            CapturedAt = DateTime.UtcNow
        };

        _context.ProofOfDeliveries.Add(proofOfDelivery);

        // Update delivery stop status
        deliveryStop.Status = DeliveryStatus.Completed;
        deliveryStop.ActualDeliveryTime = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return proofOfDelivery.Id;
    }
}

