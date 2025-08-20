using Microsoft.Extensions.Logging;
using TossErp.Accounting.Application.Common.DTOs;
using TossErp.Accounting.Application.Common.Interfaces;
using TossErp.Accounting.Application.Common.Helpers;
// Use explicit aliases to avoid any assembly ambiguity warnings for enums
// Removed EntryType alias to avoid duplicate type conflict
using EntryCategory = TossErp.Accounting.Domain.Enums.EntryCategory;
using TossErp.Accounting.Domain.Entities;

namespace TossErp.Accounting.Application.Services;

/// <summary>
/// Service for generating accounting reports
/// </summary>
public class ReportingService : IReportingService
{
    private readonly ICashbookEntryRepository _cashbookEntryRepository;
    private readonly IStockValuationService _stockValuationService;
    private readonly ILogger<ReportingService> _logger;

    public ReportingService(
        ICashbookEntryRepository cashbookEntryRepository,
        IStockValuationService stockValuationService,
        ILogger<ReportingService> logger)
    {
        _cashbookEntryRepository = cashbookEntryRepository;
        _stockValuationService = stockValuationService;
        _logger = logger;
    }

    public async Task<ProfitLossReportDto> GenerateProfitLossReportAsync(
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating P&L report from {FromDate} to {ToDate}", fromDate, toDate);

        if (toDate < fromDate)
            throw new ArgumentException("toDate must be on or after fromDate");

        var tenantId = "tenant-001"; // TODO: inject from context
        var entries = await _cashbookEntryRepository.GetByDateRangeAsync(fromDate, toDate, tenantId, cancellationToken);
        var entryList = entries.ToList();

        // Categorize revenue (credits under Sale, SalesTax excluded from revenue total but listed separately if present)
        bool IsRevenue(EntryCategory c) => c is EntryCategory.Sale or EntryCategory.CashReceipt or EntryCategory.InterestIncome or EntryCategory.Commission or EntryCategory.RentalIncome or EntryCategory.Dividend;
        bool IsCOGS(EntryCategory c) => c is EntryCategory.Purchase or EntryCategory.Adjustment or EntryCategory.PurchaseReturn;
        bool IsOperatingExpense(EntryCategory c) => c >= EntryCategory.Rent && c <= EntryCategory.Meals || c == EntryCategory.BankCharges || c == EntryCategory.Miscellaneous || c == EntryCategory.Other || c == EntryCategory.CashPayment;

        // Select only one side of double-entry postings to avoid netting to zero
    var revenueEntries = entryList.Where(e => IsRevenue(e.Category) && e.Type == TossErp.Accounting.Domain.Enums.EntryType.Credit).ToList();
    var cogsEntries = entryList.Where(e => IsCOGS(e.Category) && e.Type == TossErp.Accounting.Domain.Enums.EntryType.Debit).ToList();
    var expenseEntries = entryList.Where(e => IsOperatingExpense(e.Category) && e.Type == TossErp.Accounting.Domain.Enums.EntryType.Debit).ToList();

        var revenueByCategory = revenueEntries
            .GroupBy(e => e.Category.ToString())
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount.Amount));
        var totalRevenue = revenueByCategory.Values.Sum();

        var cogsByCategory = cogsEntries
            .GroupBy(e => e.Category.ToString())
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount.Amount));
        var totalCogs = cogsByCategory.Values.Sum();

        var expensesByCategory = expenseEntries
            .GroupBy(e => e.Category.ToString())
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount.Amount));
        var totalExpenses = expensesByCategory.Values.Sum();

        // Stock valuation (opening = day before fromDate, closing = end of toDate)
        var openingStock = await _stockValuationService.GetTotalStockValueForPLAsync(fromDate.AddDays(-1), tenantId, cancellationToken);
        var closingStock = await _stockValuationService.GetTotalStockValueForPLAsync(toDate, tenantId, cancellationToken);
        var stockChange = closingStock.Amount - openingStock.Amount;

        // Gross profit = Revenue - COGS (adjust with stockChange if using periodic system)
        var grossProfit = totalRevenue - totalCogs;
        var grossMargin = totalRevenue != 0 ? (grossProfit / totalRevenue) * 100m : 0;

        // Net profit = Gross profit - Operating expenses
        var netProfit = grossProfit - totalExpenses;
        var netMargin = totalRevenue != 0 ? (netProfit / totalRevenue) * 100m : 0;

        var report = new ProfitLossReportDto
        {
            FromDate = fromDate,
            ToDate = toDate,
            Currency = "ZAR",
            TotalRevenue = totalRevenue.RoundMoney(),
            RevenueByCategory = revenueByCategory.ToDictionary(k => k.Key, v => v.Value.RoundMoney()),
            TotalCostOfGoodsSold = totalCogs.RoundMoney(),
            CostOfGoodsSoldByCategory = cogsByCategory.ToDictionary(k => k.Key, v => v.Value.RoundMoney()),
            GrossProfit = grossProfit.RoundMoney(),
            GrossProfitMargin = grossMargin.RoundMoney(),
            TotalOperatingExpenses = totalExpenses.RoundMoney(),
            OperatingExpensesByCategory = expensesByCategory.ToDictionary(k => k.Key, v => v.Value.RoundMoney()),
            NetProfit = netProfit.RoundMoney(),
            NetProfitMargin = netMargin.RoundMoney(),
            OpeningStockValue = openingStock.Amount.RoundMoney(),
            ClosingStockValue = closingStock.Amount.RoundMoney(),
            StockValuationChange = stockChange.RoundMoney(),
            TotalTransactions = entryList.Count
        };

        _logger.LogInformation("Generated P&L report: NetProfit={NetProfit}, TotalRevenue={TotalRevenue}", report.NetProfit, report.TotalRevenue);
        return report;
    }

    public async Task<CashPositionReportDto> GenerateCashPositionReportAsync(
        DateTime asOfDate,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating cash position report as of {AsOfDate}", asOfDate);
        var tenantId = "tenant-001";
        var entries = await _cashbookEntryRepository.GetByTenantAsync(tenantId, cancellationToken);
        var list = entries.Where(e => e.TransactionDate.Date <= asOfDate.Date).OrderByDescending(e => e.TransactionDate).ToList();

        // Current cash balance = sum of effective amounts for cash related categories (simplified: all debits - credits overall)
    decimal currentBalance = list.Sum(e => e.Type == TossErp.Accounting.Domain.Enums.EntryType.Debit ? e.Amount.Amount : -e.Amount.Amount);

        // Define inflow/outflow categories
        bool IsInflow(EntryCategory c) => c is EntryCategory.Sale or EntryCategory.CashReceipt or EntryCategory.InterestIncome or EntryCategory.RentalIncome or EntryCategory.Commission or EntryCategory.Dividend;
        bool IsOutflow(EntryCategory c) => !IsInflow(c) && c != EntryCategory.OpeningBalance && c != EntryCategory.ClosingBalance; // treat everything else as outflow for MVP except balancing entries

    var inflowByCat = list.Where(e => IsInflow(e.Category) && e.Type == TossErp.Accounting.Domain.Enums.EntryType.Debit)
            .GroupBy(e => e.Category.ToString())
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount.Amount));
    var outflowByCat = list.Where(e => IsOutflow(e.Category) && e.Type == TossErp.Accounting.Domain.Enums.EntryType.Credit)
            .GroupBy(e => e.Category.ToString())
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount.Amount));

        decimal totalInflow = inflowByCat.Values.Sum();
        decimal totalOutflow = outflowByCat.Values.Sum();
        decimal netCashFlow = totalInflow - totalOutflow;

        // Liquidity metrics (simple heuristics): daily avg outflow over last 30 days
        var last30 = list.Where(e => e.TransactionDate >= asOfDate.AddDays(-30)).ToList();
        var dailyOutflow = last30.Where(e => IsOutflow(e.Category))
            .Sum(e => e.Type == TossErp.Accounting.Domain.Enums.EntryType.Debit ? e.Amount.Amount : -e.Amount.Amount) / 30m;
        decimal daysOfCash = dailyOutflow > 0 ? currentBalance / dailyOutflow : 0;
        decimal cashFlowRatio = totalOutflow != 0 ? totalInflow / totalOutflow : 0;

        var recentTransactions = list.Take(10).Select(e => new CashTransactionDto
        {
            Id = e.Id,
            TransactionDate = e.TransactionDate,
            Amount = e.Amount.Amount,
            Type = e.Type.ToString(),
            Category = e.Category.ToString(),
            Description = e.Description,
            Reference = e.Reference
        }).ToList();

        var report = new CashPositionReportDto
        {
            AsOfDate = asOfDate,
            Currency = "ZAR",
            CurrentCashBalance = currentBalance.RoundMoney(),
            AvailableCash = currentBalance.RoundMoney(), // no committed tracking yet
            CommittedCash = 0,
            CashInflow = totalInflow.RoundMoney(),
            CashOutflow = totalOutflow.RoundMoney(),
            NetCashFlow = netCashFlow.RoundMoney(),
            CashInflowByCategory = inflowByCat.ToDictionary(k => k.Key, v => v.Value.RoundMoney()),
            CashOutflowByCategory = outflowByCat.ToDictionary(k => k.Key, v => Math.Abs(v.Value).RoundMoney()),
            DaysOfCashOnHand = daysOfCash.RoundMoney(),
            CashFlowRatio = cashFlowRatio.RoundMoney(),
            RecentTransactions = recentTransactions,
            TotalTransactions = list.Count
        };

        _logger.LogInformation("Generated cash position report: CurrentBalance={CurrentBalance}, NetCashFlow={NetCashFlow}", report.CurrentCashBalance, report.NetCashFlow);
        return report;
    }

    public async Task<MonthOverMonthReportDto> GenerateMonthOverMonthReportAsync(
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating month-over-month report from {FromDate} to {ToDate}", fromDate, toDate);
        if (toDate < fromDate) throw new ArgumentException("toDate must be >= fromDate");

        var tenantId = "tenant-001";
        var entries = await _cashbookEntryRepository.GetByDateRangeAsync(fromDate, toDate, tenantId, cancellationToken);
        var list = entries.ToList();

        bool IsRevenue(EntryCategory c) => c is EntryCategory.Sale or EntryCategory.CashReceipt or EntryCategory.InterestIncome or EntryCategory.Commission or EntryCategory.RentalIncome or EntryCategory.Dividend;
        bool IsExpense(EntryCategory c) => !(IsRevenue(c) || c == EntryCategory.OpeningBalance || c == EntryCategory.ClosingBalance);

        var monthlyData = new List<MonthlyDataDto>();
        var cursor = new DateTime(fromDate.Year, fromDate.Month, 1);
        var endInclusive = new DateTime(toDate.Year, toDate.Month, 1);
        endInclusive = endInclusive.AddMonths(1); // exclusive

        while (cursor < endInclusive)
        {
            var monthStart = cursor;
            var monthEnd = cursor.AddMonths(1).AddDays(-1);
            var monthEntries = list.Where(e => e.TransactionDate.Date >= monthStart.Date && e.TransactionDate.Date <= monthEnd.Date).ToList();

            // Use single-side selection consistent with P&L: revenue from credit entries only, expenses from debit entries only
            decimal revenue = monthEntries.Where(e => IsRevenue(e.Category) && e.Type == TossErp.Accounting.Domain.Enums.EntryType.Credit)
                .Sum(e => e.Amount.Amount);
            decimal expenses = monthEntries.Where(e => IsExpense(e.Category) && e.Type == TossErp.Accounting.Domain.Enums.EntryType.Debit)
                .Sum(e => e.Amount.Amount);
            decimal profit = revenue - expenses;
            decimal cashFlow = profit; // simplified

            var categoryBreakdown = monthEntries
                .GroupBy(e => e.Category.ToString())
                .ToDictionary(g => g.Key, g => g.Sum(e => e.Type == TossErp.Accounting.Domain.Enums.EntryType.Debit ? e.Amount.Amount : -e.Amount.Amount));

            monthlyData.Add(new MonthlyDataDto
            {
                Month = monthStart,
                Revenue = revenue.RoundMoney(),
                Expenses = expenses.RoundMoney(),
                Profit = profit.RoundMoney(),
                CashFlow = cashFlow.RoundMoney(),
                TransactionCount = monthEntries.Count,
                CategoryBreakdown = categoryBreakdown.ToDictionary(k => k.Key, v => v.Value.RoundMoney())
            });

            cursor = cursor.AddMonths(1);
        }

        if (!monthlyData.Any())
        {
            return new MonthOverMonthReportDto
            {
                FromDate = fromDate,
                ToDate = toDate,
                Currency = "ZAR",
                MonthlyData = new List<MonthlyDataDto>(),
                TotalMonths = 0
            };
        }

        decimal revenueGrowthRate = 0, expenseGrowthRate = 0, profitGrowthRate = 0;
        if (monthlyData.Count > 1)
        {
            var first = monthlyData.First();
            var last = monthlyData.Last();
            revenueGrowthRate = first.Revenue != 0 ? ((last.Revenue - first.Revenue) / first.Revenue) * 100m : 0;
            expenseGrowthRate = first.Expenses != 0 ? ((last.Expenses - first.Expenses) / first.Expenses) * 100m : 0;
            profitGrowthRate = first.Profit != 0 ? ((last.Profit - first.Profit) / first.Profit) * 100m : 0;
        }

        // Trends by category
        var categoryTrends = new Dictionary<string, List<MonthlyTrendDto>>();
        var allCategories = monthlyData.SelectMany(m => m.CategoryBreakdown.Keys).Distinct();
        foreach (var cat in allCategories)
        {
            decimal prev = 0;
            var trends = new List<MonthlyTrendDto>();
            foreach (var month in monthlyData)
            {
                var val = month.CategoryBreakdown.GetValueOrDefault(cat, 0);
                var change = prev != 0 ? val - prev : 0;
                var pct = prev != 0 ? (change / prev) * 100m : 0;
                trends.Add(new MonthlyTrendDto
                {
                    Month = month.Month,
                    Value = Round2(val),
                    ChangeFromPrevious = Round2(change),
                    PercentageChange = Round2(pct)
                });
                prev = val;
            }
            categoryTrends[cat] = trends;
        }

        var report = new MonthOverMonthReportDto
        {
            FromDate = fromDate,
            ToDate = toDate,
            Currency = "ZAR",
            MonthlyData = monthlyData,
            RevenueGrowthRate = revenueGrowthRate.RoundMoney(),
            ExpenseGrowthRate = expenseGrowthRate.RoundMoney(),
            ProfitGrowthRate = profitGrowthRate.RoundMoney(),
            CategoryTrends = categoryTrends,
            TotalMonths = monthlyData.Count
        };

        _logger.LogInformation("Generated month-over-month report: TotalMonths={TotalMonths}", report.TotalMonths);
        return report;
    }

    private static decimal Round2(decimal value) => value.RoundMoney();
}
