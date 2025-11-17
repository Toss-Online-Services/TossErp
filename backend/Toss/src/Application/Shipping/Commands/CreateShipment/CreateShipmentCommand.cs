using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Shipping;

namespace Toss.Application.Shipping.Commands.CreateShipment;

public record ShipmentItemDto
{
    public int OrderItemId { get; init; }
    public int Quantity { get; init; }
}

public record CreateShipmentCommand : IRequest<int>
{
    public int OrderId { get; init; }
    public string? TrackingNumber { get; init; }
    public decimal? TotalWeight { get; init; }
    public List<ShipmentItemDto> Items { get; init; } = new();
}

public class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateShipmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
    {
        // Validate order exists
        var order = await _context.Orders.FindAsync(new object[] { request.OrderId }, cancellationToken);
        if (order == null)
            throw new NotFoundException(nameof(Domain.Entities.Orders.Order), request.OrderId.ToString());

        var shipment = new Shipment
        {
            OrderId = request.OrderId,
            TrackingNumber = request.TrackingNumber ?? string.Empty,
            TotalWeight = request.TotalWeight
        };

        _context.Shipments.Add(shipment);

        // Add shipment items
        foreach (var itemDto in request.Items)
        {
            var shipmentItem = new ShipmentItem
            {
                ShipmentId = shipment.Id,
                OrderItemId = itemDto.OrderItemId,
                Quantity = itemDto.Quantity
            };

            shipment.ShipmentItems.Add(shipmentItem);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return shipment.Id;
    }
}

