using Toss.Application.Common.Interfaces;
using Toss.Domain.Enums;

namespace Toss.Application.Dashboard.Queries.GetCashFlowSummary;

public record CashFlowSummaryDto
{
    public decimal TotalCashIn { get; init; }
    public decimal TotalCashOut { get; init; }
    public decimal NetCashFlow { get; init; }
    public decimal SalesRevenue { get; init; }
    public decimal PurchaseExpenses { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}

public record GetCashFlowSummaryQuery : IRequest<CashFlowSummaryDto>
{
    public int ShopId { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}

public class GetCashFlowSummaryQueryHandler : IRequestHandler<GetCashFlowSummaryQuery, CashFlowSummaryDto>
{
    private readonly IApplicationDbContext _context;

    public GetCashFlowSummaryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CashFlowSummaryDto> Handle(GetCashFlowSummaryQuery request, CancellationToken cancellationToken)
    {
        // Cash In: Sales
        var salesRevenue = await _context.Sales
            .Where(s => s.ShopId == request.ShopId &&
                       s.SaleDate >= request.StartDate &&
                       s.SaleDate <= request.EndDate &&
                       s.Status != SaleStatus.Voided)
            .SumAsync(s => s.TotalAmount, cancellationToken);

        // Cash Out: Purchase Orders
        var purchaseExpenses = await _context.PurchaseOrders
            .Where(po => po.ShopId == request.ShopId &&
                        po.OrderDate >= request.StartDate &&
                        po.OrderDate <= request.EndDate &&
                        po.Status != PurchaseOrderStatus.Cancelled)
            .SumAsync(po => po.TotalAmount, cancellationToken);

        return new CashFlowSummaryDto
        {
            TotalCashIn = salesRevenue,
            TotalCashOut = purchaseExpenses,
            NetCashFlow = salesRevenue - purchaseExpenses,
            SalesRevenue = salesRevenue,
            PurchaseExpenses = purchaseExpenses,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };
    }
}

