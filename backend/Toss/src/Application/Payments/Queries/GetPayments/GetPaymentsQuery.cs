using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Mappings;
using Toss.Application.Common.Models;
using Toss.Domain.Entities.Payments;

namespace Toss.Application.Payments.Queries.GetPayments;

public record PaymentDto
{
    public int Id { get; init; }
    public int ShopId { get; init; }
    public decimal Amount { get; init; }
    public string PaymentType { get; init; } = string.Empty;
    public DateTimeOffset PaymentDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? TransactionRef { get; init; }
    public int? SaleId { get; init; }
    public int? PurchaseOrderId { get; init; }
}

public record GetPaymentsQuery : IRequest<PaginatedList<PaymentDto>>
{
    public int ShopId { get; init; }
    public DateTimeOffset? StartDate { get; init; }
    public DateTimeOffset? EndDate { get; init; }
    public string? Status { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public class GetPaymentsQueryHandler : IRequestHandler<GetPaymentsQuery, PaginatedList<PaymentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPaymentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<PaymentDto>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Payments
            .Where(p => p.ShopId == request.ShopId)
            .AsQueryable();

        if (request.StartDate.HasValue)
            query = query.Where(p => p.PaymentDate >= request.StartDate.Value);

        if (request.EndDate.HasValue)
            query = query.Where(p => p.PaymentDate <= request.EndDate.Value);

        if (!string.IsNullOrWhiteSpace(request.Status))
            query = query.Where(p => p.Status.ToString() == request.Status);

        var payments = await query
            .OrderByDescending(p => p.PaymentDate)
            .Select(p => new PaymentDto
            {
                Id = p.Id,
                ShopId = p.ShopId,
                Amount = p.Amount,
                PaymentType = p.PaymentType.ToString(),
                PaymentDate = p.PaymentDate,
                Status = p.Status.ToString(),
                TransactionRef = p.TransactionRef,
                SaleId = p.SaleId,
                PurchaseOrderId = p.PurchaseOrderId
            })
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return payments;
    }
}

