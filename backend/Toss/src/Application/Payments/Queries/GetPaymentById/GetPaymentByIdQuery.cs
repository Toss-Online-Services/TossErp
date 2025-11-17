using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Payments;

namespace Toss.Application.Payments.Queries.GetPaymentById;

public record PaymentDetailDto
{
    public int Id { get; init; }
    public int ShopId { get; init; }
    public string ShopName { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string PaymentType { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string? TransactionRef { get; init; }
    public DateTimeOffset PaymentDate { get; init; }
}

public record GetPaymentByIdQuery : IRequest<PaymentDetailDto>
{
    public int Id { get; init; }
}

public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, PaymentDetailDto>
{
    private readonly IApplicationDbContext _context;

    public GetPaymentByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentDetailDto> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await _context.Payments
            .Include(p => p.Shop)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (payment == null)
            throw new NotFoundException(nameof(Payment), request.Id.ToString());

        return new PaymentDetailDto
        {
            Id = payment.Id,
            ShopId = payment.ShopId,
            ShopName = payment.Shop.Name,
            Amount = payment.Amount,
            PaymentType = payment.PaymentType.ToString(),
            Status = payment.Status.ToString(),
            TransactionRef = payment.TransactionRef,
            PaymentDate = payment.PaymentDate
        };
    }
}

