using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Analytics;
using Toss.Application.Common.Interfaces.Authentication;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Inventory.Commands.AcknowledgeStockAlert;

public record AcknowledgeStockAlertCommand : IRequest<bool>
{
    public int AlertId { get; init; }
}

public class AcknowledgeStockAlertCommandHandler : IRequestHandler<AcknowledgeStockAlertCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;
    private readonly IUser _user;
    private readonly IBusinessEventService _eventService;

    public AcknowledgeStockAlertCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext,
        IUser user,
        IBusinessEventService eventService)
    {
        _context = context;
        _businessContext = businessContext;
        _user = user;
        _eventService = eventService;
    }

    public async Task<bool> Handle(AcknowledgeStockAlertCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var alert = await _context.StockAlerts
            .Include(a => a.Product)
            .FirstOrDefaultAsync(a => a.Id == request.AlertId
                && a.Shop.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (alert == null)
        {
            return false;
        }

        if (alert.IsAcknowledged)
        {
            return true; // Already acknowledged
        }

        alert.IsAcknowledged = true;
        alert.AcknowledgedAt = DateTimeOffset.UtcNow;
        alert.AcknowledgedBy = _user.Id;

        await _context.SaveChangesAsync(cancellationToken);

        // Track analytics event
        var eventData = System.Text.Json.JsonSerializer.Serialize(new
        {
            AlertId = alert.Id,
            ProductId = alert.ProductId,
            ProductName = alert.Product.Name,
            ShopId = alert.ShopId,
            CurrentStock = alert.CurrentStock,
            MinimumStock = alert.MinimumStock
        });

        await _eventService.EmitEventAsync(
            BusinessEventType.StockAlertResolved,
            module: "Stock",
            eventData: eventData,
            userId: _user.Id,
            cancellationToken: cancellationToken);

        return true;
    }
}

