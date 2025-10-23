using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Payments;
using Toss.Domain.Enums;
using Toss.Domain.Events;

namespace Toss.Application.Payments.Commands.RecordPayment;

public record RecordPaymentCommand : IRequest<int>
{
    public int ShopId { get; init; }
    public int? SaleId { get; init; }
    public int? PurchaseOrderId { get; init; }
    public decimal Amount { get; init; }
    public PaymentType PaymentType { get; init; }
    public string? TransactionRef { get; init; }
    public string? Notes { get; init; }
}

public class RecordPaymentCommandHandler : IRequestHandler<RecordPaymentCommand, int>
{
    private readonly IApplicationDbContext _context;

    public RecordPaymentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(RecordPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = new Payment
        {
            ShopId = request.ShopId,
            SaleId = request.SaleId,
            PurchaseOrderId = request.PurchaseOrderId,
            Amount = request.Amount,
            PaymentType = request.PaymentType,
            PaymentDate = DateTime.UtcNow,
            TransactionRef = request.TransactionRef,
            Status = PaymentStatus.Completed,
            Notes = request.Notes
        };

        _context.Payments.Add(payment);

        // Add domain event
        payment.AddDomainEvent(new PaymentReceivedEvent(payment));

        await _context.SaveChangesAsync(cancellationToken);

        return payment.Id;
    }
}

