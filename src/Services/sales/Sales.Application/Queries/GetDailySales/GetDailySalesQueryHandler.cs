using MediatR;
using TossErp.Sales.Application.Common.DTOs;
using TossErp.Sales.Application.Common.Interfaces;
using TossErp.Sales.Domain.Common;
using TossErp.Sales.Domain.Enums;

namespace TossErp.Sales.Application.Queries.GetDailySales;

/// <summary>
/// Handler for getting daily sales summary
/// </summary>
public class GetDailySalesQueryHandler : IRequestHandler<GetDailySalesQuery, DailySalesSummaryDto>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ITillRepository _tillRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetDailySalesQueryHandler(
        ISaleRepository saleRepository,
        ITillRepository tillRepository,
        ICurrentUserService currentUserService)
    {
        _saleRepository = saleRepository;
        _tillRepository = tillRepository;
        _currentUserService = currentUserService;
    }

    public async Task<DailySalesSummaryDto> Handle(GetDailySalesQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUserService.TenantId ?? "default-tenant";
        var startDate = request.Date.Date;
        var endDate = startDate.AddDays(1).AddTicks(-1);

        // Get sales for the date range
        var sales = await _saleRepository.GetByDateRangeAsync(startDate, endDate, request.TillId, cancellationToken);
        var salesList = sales.ToList();

        // Get tills for reference
        var tills = await _tillRepository.GetAllAsync(cancellationToken);
        var tillsList = tills.ToList();

        // Calculate summary
        var completedSales = salesList.Where(s => s.Status == SaleStatus.Completed).ToList();
        var cancelledSales = salesList.Where(s => s.Status == SaleStatus.Cancelled).ToList();

        var summary = new DailySalesSummaryDto
        {
            Date = request.Date,
            TillId = request.TillId,
            TillName = request.TillId.HasValue 
                ? tillsList.FirstOrDefault(t => t.Id == request.TillId)?.Name ?? "Unknown"
                : "All Tills",
            TotalSales = completedSales.Count,
            TotalCancelled = cancelledSales.Count,
            TotalTransactions = salesList.Count,
            TotalRevenue = completedSales.Sum(s => s.Total.Amount),
            TotalTax = completedSales.Sum(s => s.TaxAmount.Amount),
            TotalDiscount = completedSales.Sum(s => s.DiscountAmount.Amount),
            AverageTransactionValue = completedSales.Any() 
                ? completedSales.Average(s => s.Total.Amount) 
                : 0,
            TopSellingItems = GetTopSellingItems(completedSales, 10),
            SalesByHour = GetSalesByHour(completedSales),
            PaymentMethodBreakdown = GetPaymentMethodBreakdown(completedSales)
        };

        return summary;
    }

    private static List<TopSellingItemDto> GetTopSellingItems(IEnumerable<Sale> sales, int count)
    {
        return sales
            .SelectMany(s => s.Items)
            .GroupBy(item => new { item.ItemId, item.ItemName, item.ItemSku })
            .Select(g => new TopSellingItemDto
            {
                ItemId = g.Key.ItemId,
                ItemName = g.Key.ItemName,
                ItemSku = g.Key.ItemSku,
                TotalQuantity = g.Sum(item => item.Quantity),
                TotalRevenue = g.Sum(item => item.LineTotalIncludingTax.Amount),
                AveragePrice = g.Average(item => item.UnitPrice.Amount)
            })
            .OrderByDescending(x => x.TotalQuantity)
            .Take(count)
            .ToList();
    }

    private static List<SalesByHourDto> GetSalesByHour(IEnumerable<Sale> sales)
    {
        var salesByHour = new List<SalesByHourDto>();
        
        for (int hour = 0; hour < 24; hour++)
        {
            var hourSales = sales.Where(s => s.CompletedAt?.Hour == hour).ToList();
            salesByHour.Add(new SalesByHourDto
            {
                Hour = hour,
                SalesCount = hourSales.Count,
                TotalRevenue = hourSales.Sum(s => s.Total.Amount)
            });
        }

        return salesByHour;
    }

    private static List<PaymentMethodBreakdownDto> GetPaymentMethodBreakdown(IEnumerable<Sale> sales)
    {
        return sales
            .SelectMany(s => s.Payments)
            .GroupBy(p => p.Method)
            .Select(g => new PaymentMethodBreakdownDto
            {
                Method = g.Key,
                Count = g.Count(),
                TotalAmount = g.Sum(p => p.Amount.Amount)
            })
            .OrderByDescending(x => x.TotalAmount)
            .ToList();
    }
}
