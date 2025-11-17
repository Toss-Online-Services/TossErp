using Toss.Application.Common.Interfaces;

namespace Toss.Application.Buying.Commands.UpdateVendorInvoiceStatus;

public record UpdateVendorInvoiceStatusCommand : IRequest<bool>
{
    public int Id { get; init; }
    public string Status { get; init; } = string.Empty;
}

public class UpdateVendorInvoiceStatusCommandHandler : IRequestHandler<UpdateVendorInvoiceStatusCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateVendorInvoiceStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateVendorInvoiceStatusCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.PurchaseDocuments.FindAsync(new object?[] { request.Id }, cancellationToken);
        if (entity is null)
            return false;

        switch (request.Status.ToLowerInvariant())
        {
            case "paid":
                entity.IsPaid = true;
                entity.PaidDate = DateTimeOffset.UtcNow;
                break;
            case "approved":
                entity.IsApproved = true;
                break;
            case "sent":
                // no-op for now
                break;
            case "cancelled":
                // no explicit flag; use notes to annotate
                entity.Notes = (entity.Notes ?? string.Empty) + "\n[CANCELLED] " + DateTimeOffset.UtcNow.ToString("u");
                break;
            default:
                return false;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
