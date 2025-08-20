using Moq;
using Microsoft.Extensions.Logging;
using TossErp.Accounting.Application.Common.Interfaces;
using TossErp.Accounting.Application.Services;
using TossErp.Accounting.Domain.Entities;
using TossErp.Accounting.Domain.Enums;
using TossErp.Accounting.Domain.Common;
using Xunit;

namespace TossErp.Accounting.Application.Tests;

public class ReportingServiceTests
{
    private readonly Mock<ICashbookEntryRepository> _entryRepoMock = new();
    private readonly Mock<IStockValuationService> _stockValuationServiceMock = new();
    private readonly Mock<ILogger<ReportingService>> _loggerMock = new();
    private readonly ReportingService _service;

    private readonly DateTime _from = new DateTime(2025, 1, 1);
    private readonly DateTime _to = new DateTime(2025, 1, 31);

    public ReportingServiceTests()
    {
        _service = new ReportingService(_entryRepoMock.Object, _stockValuationServiceMock.Object, _loggerMock.Object);
    }

    private static CashbookEntry Entry(DateTime date, decimal amount, EntryType type, EntryCategory category, string tenant = "tenant-001")
    {
        return CashbookEntry.Create(date, $"REF-{Guid.NewGuid():N}".Substring(0,10), category.ToString(), new Money(amount, "ZAR"), type, category, Guid.NewGuid(), tenant);
    }

    private void SetupEntries(IEnumerable<CashbookEntry> entries)
    {
        var list = entries.ToList();
        _entryRepoMock.Setup(r => r.GetByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((DateTime f, DateTime t, string tenant, CancellationToken ct) => list.Where(e => e.TransactionDate.Date >= f.Date && e.TransactionDate.Date <= t.Date));
        _entryRepoMock.Setup(r => r.GetByTenantAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);
    }

    [Fact]
    public async Task GenerateProfitLossReportAsync_ShouldAggregateSingleSideEntries()
    {
        var entries = new List<CashbookEntry>
        {
            // Sale double-entry (debit cash inflow + credit revenue) – only credit counted for revenue
            Entry(_from.AddDays(1), 1000m, EntryType.Debit, EntryCategory.Sale),
            Entry(_from.AddDays(1), 1000m, EntryType.Credit, EntryCategory.Sale),
            // Second sale
            Entry(_from.AddDays(2), 500m, EntryType.Debit, EntryCategory.Sale),
            Entry(_from.AddDays(2), 500m, EntryType.Credit, EntryCategory.Sale),
            // Purchase (inventory) – debit counted as COGS (simplified for MVP)
            Entry(_from.AddDays(3), 300m, EntryType.Debit, EntryCategory.Purchase),
            Entry(_from.AddDays(3), 300m, EntryType.Credit, EntryCategory.Purchase),
            // Expense (rent) – debit counted as operating expense
            Entry(_from.AddDays(4), 200m, EntryType.Debit, EntryCategory.Rent),
            Entry(_from.AddDays(4), 200m, EntryType.Credit, EntryCategory.Rent)
        };
        SetupEntries(entries);

        _stockValuationServiceMock.Setup(s => s.GetTotalStockValueForPLAsync(_from.AddDays(-1), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Money(5000m, "ZAR"));
        _stockValuationServiceMock.Setup(s => s.GetTotalStockValueForPLAsync(_to, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Money(5500m, "ZAR"));

        var report = await _service.GenerateProfitLossReportAsync(_from, _to);

        Assert.Equal(1500m, report.TotalRevenue); // 1000 + 500 credit sale entries only
        Assert.Equal(300m, report.TotalCostOfGoodsSold); // purchase debit only
        Assert.Equal(200m, report.TotalOperatingExpenses); // rent debit only
        Assert.Equal(1200m, report.GrossProfit); // 1500 - 300
        Assert.Equal(1000m, report.NetProfit); // 1200 - 200
        Assert.Equal(500m, report.StockValuationChange); // 5500 - 5000
    }

    [Fact]
    public async Task GenerateCashPositionReportAsync_ShouldComputeInflowsOutflowsSingleSided()
    {
        var asOf = _from.AddDays(10);
        var entries = new List<CashbookEntry>
        {
            // Sale double-entry (Debit cash inflow, Credit revenue) – inflow counts debit only
            Entry(_from.AddDays(1), 800m, EntryType.Debit, EntryCategory.Sale),
            Entry(_from.AddDays(1), 800m, EntryType.Credit, EntryCategory.Sale),
            // Expense payment (rent) – outflow uses credit of outflow category (Rent treated as expense -> IsOutflow)
            Entry(_from.AddDays(2), 300m, EntryType.Debit, EntryCategory.Rent),
            Entry(_from.AddDays(2), 300m, EntryType.Credit, EntryCategory.Rent)
        };        
        SetupEntries(entries);

        var report = await _service.GenerateCashPositionReportAsync(asOf);

        Assert.Equal(800m, report.CashInflow); // debit sale only
        Assert.Equal(300m, report.CashOutflow); // credit rent only
        Assert.Equal(500m, report.NetCashFlow);
        Assert.True(report.CurrentCashBalance != 0); // Balance reflects all debits - credits net
    }

    [Fact]
    public async Task GenerateMonthOverMonthReportAsync_ShouldSplitMonthsCorrectly()
    {
        var febTo = new DateTime(2025, 2, 28);
        var entries = new List<CashbookEntry>
        {
            // January sale
            Entry(new DateTime(2025,1,10), 1000m, EntryType.Credit, EntryCategory.Sale),
            Entry(new DateTime(2025,1,10), 1000m, EntryType.Debit, EntryCategory.Sale),
            // February sale
            Entry(new DateTime(2025,2,5), 1500m, EntryType.Credit, EntryCategory.Sale),
            Entry(new DateTime(2025,2,5), 1500m, EntryType.Debit, EntryCategory.Sale),
            // February expense
            Entry(new DateTime(2025,2,15), 400m, EntryType.Debit, EntryCategory.Rent),
            Entry(new DateTime(2025,2,15), 400m, EntryType.Credit, EntryCategory.Rent)
        };
        SetupEntries(entries);

        var report = await _service.GenerateMonthOverMonthReportAsync(_from, febTo);

        Assert.Equal(2, report.TotalMonths);
        var jan = report.MonthlyData.First(m => m.Month.Month == 1);
        var feb = report.MonthlyData.First(m => m.Month.Month == 2);
        Assert.Equal(1000m, jan.Revenue);
        Assert.Equal(0m, jan.Expenses);
        Assert.Equal(1500m, feb.Revenue);
        Assert.Equal(400m, feb.Expenses);
        Assert.Equal(1100m, feb.Profit); // 1500 - 400
        Assert.True(report.RevenueGrowthRate > 0);
    }
}
