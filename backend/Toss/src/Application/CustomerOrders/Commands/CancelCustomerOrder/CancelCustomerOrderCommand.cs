using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Exceptions;
using Toss.Domain.Entities.Orders;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.CustomerOrders.Commands.CancelCustomerOrder;

public record CancelCustomerOrderCommand : IRequest<bool>
{
    public int OrderId { get; init; }
    public string? Reason { get; init; }
}

public class CancelCustomerOrderCommandHandler : IRequestHandler<CancelCustomerOrderCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public CancelCustomerOrderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(CancelCustomerOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && !o.Deleted, cancellationToken);

        if (order == null)
            throw new Common.Exceptions.NotFoundException(nameof(Order), request.OrderId);

        if (order.OrderStatus == OrderStatus.Complete)
            throw new BadRequestException("Cannot cancel a completed order");

        order.OrderStatus = OrderStatus.Cancelled;

        // Add cancellation note
        order.OrderNotes.Add(new OrderNote
        {
            Note = $"Order cancelled: {request.Reason ?? "No reason provided"}",
            DisplayToCustomer = true,
            CreatedOnUtc = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

