using Toss.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Sales.Commands.UpdateInvoiceStatus;

public record UpdateInvoiceStatusCommand : IRequest<bool>
{
    public int InvoiceId { get; init; }
    public string Status { get; init; } = string.Empty;
}

public class UpdateInvoiceStatusCommandHandler : IRequestHandler<UpdateInvoiceStatusCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateInvoiceStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateInvoiceStatusCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Sale)
            .FirstOrDefaultAsync(i => i.Id == request.InvoiceId, cancellationToken);

        if (invoice == null)
            return false;

        switch (request.Status.ToLower())
        {
            case "paid":
                invoice.IsPaid = true;
                invoice.PaidDate = DateTimeOffset.UtcNow;
                break;
            case "sent":
                // Mark sale as completed if it's pending
                if (invoice.Sale.Status == Domain.Enums.SaleStatus.Pending)
                {
                    invoice.Sale.Status = Domain.Enums.SaleStatus.Completed;
                }
                break;
            case "cancelled":
                invoice.Sale.Status = Domain.Enums.SaleStatus.Voided;
                break;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

