using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Domain.Enums;
using Toss.Domain.Entities.Sales;

namespace Toss.Application.Sales.Commands.UpdateSaleStatus;

public record UpdateSaleStatusCommand : IRequest<bool>
{
    public int SaleId { get; init; }
    public SaleStatus NewStatus { get; init; }
    public string? Notes { get; init; }
}

public class UpdateSaleStatusCommandHandler : IRequestHandler<UpdateSaleStatusCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateSaleStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateSaleStatusCommand request, CancellationToken cancellationToken)
    {
        var sale = await _context.Sales
            .FirstOrDefaultAsync(s => s.Id == request.SaleId, cancellationToken);

        if (sale == null)
            throw new Common.Exceptions.NotFoundException(nameof(Sale), request.SaleId);

        // Validate status transition
        if (!IsValidStatusTransition(sale.Status, request.NewStatus))
        {
            throw new BadRequestException($"Cannot transition from {sale.Status} to {request.NewStatus}");
        }

        sale.Status = request.NewStatus;
        // LastModified is automatically handled by BaseAuditableEntity interceptor

        if (!string.IsNullOrEmpty(request.Notes))
        {
            // Store notes in the Notes field
            sale.Notes = string.IsNullOrEmpty(sale.Notes) 
                ? request.Notes 
                : $"{sale.Notes}\n[{DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm}] {request.Notes}";
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static bool IsValidStatusTransition(SaleStatus current, SaleStatus next)
    {
        // Define valid status transitions
        // Valid statuses: Pending, Completed, Voided, Refunded
        return (current, next) switch
        {
            (SaleStatus.Pending, SaleStatus.Completed) => true,
            (SaleStatus.Pending, SaleStatus.Voided) => true,
            (SaleStatus.Completed, SaleStatus.Refunded) => true,
            (SaleStatus.Completed, SaleStatus.Voided) => false,
            _ when current == next => true, // Allow same status (no-op)
            _ => false
        };
    }
}

