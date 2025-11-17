using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Exceptions;
using Toss.Domain.Entities.Orders;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.CustomerOrders.Commands.UpdateCustomerOrderStatus;

public record UpdateCustomerOrderStatusCommand : IRequest<bool>
{
    public int OrderId { get; init; }
    public OrderStatus NewStatus { get; init; }
    public string? Notes { get; init; }
}

public class UpdateCustomerOrderStatusCommandHandler : IRequestHandler<UpdateCustomerOrderStatusCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateCustomerOrderStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateCustomerOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && !o.Deleted, cancellationToken);

        if (order == null)
            throw new Common.Exceptions.NotFoundException(nameof(Order), request.OrderId);

        order.OrderStatus = request.NewStatus;

        // Add note if provided
        if (!string.IsNullOrWhiteSpace(request.Notes))
        {
            order.OrderNotes.Add(new OrderNote
            {
                Note = request.Notes,
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
        }

        // Update payment status based on order status
        if (request.NewStatus == OrderStatus.Complete)
        {
            order.PaymentStatus = Domain.Enums.PaymentStatus.Completed;
            order.PaidDateUtc = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

