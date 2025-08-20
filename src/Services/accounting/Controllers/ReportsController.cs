using Microsoft.AspNetCore.Mvc;
using MediatR;
using TossErp.Accounting.Application.Common.DTOs;
using TossErp.Accounting.Application.Common.Interfaces;

namespace TossErp.Accounting.Controllers;

/// <summary>
/// API controller for accounting reports
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportingService _reportingService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(IReportingService reportingService, ILogger<ReportsController> logger)
    {
        _reportingService = reportingService;
        _logger = logger;
    }

    /// <summary>
    /// Generate Profit & Loss report for a given period
    /// </summary>
    [HttpGet("profit-loss")]
    public async Task<ActionResult<ProfitLossReportDto>> GetProfitLossReport(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        try
        {
            _logger.LogInformation("Generating P&L report from {FromDate} to {ToDate}", fromDate, toDate);

            var report = await _reportingService.GenerateProfitLossReportAsync(fromDate, toDate);
            return Ok(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating P&L report");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Generate cash position report as of a specific date
    /// </summary>
    [HttpGet("cash-position")]
    public async Task<ActionResult<CashPositionReportDto>> GetCashPositionReport(
        [FromQuery] DateTime? asOfDate = null)
    {
        try
        {
            var reportDate = asOfDate ?? DateTime.Today;
            _logger.LogInformation("Generating cash position report as of {AsOfDate}", reportDate);

            var report = await _reportingService.GenerateCashPositionReportAsync(reportDate);
            return Ok(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating cash position report");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Generate month-over-month comparison report
    /// </summary>
    [HttpGet("month-over-month")]
    public async Task<ActionResult<MonthOverMonthReportDto>> GetMonthOverMonthReport(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        try
        {
            _logger.LogInformation("Generating month-over-month report from {FromDate} to {ToDate}", fromDate, toDate);

            var report = await _reportingService.GenerateMonthOverMonthReportAsync(fromDate, toDate);
            return Ok(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating month-over-month report");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Export P&L report to CSV
    /// </summary>
    [HttpGet("profit-loss/export/csv")]
    public async Task<IActionResult> ExportProfitLossReportToCsv(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        try
        {
            _logger.LogInformation("Exporting P&L report to CSV from {FromDate} to {ToDate}", fromDate, toDate);

            var report = await _reportingService.GenerateProfitLossReportAsync(fromDate, toDate);
            var csvContent = GenerateProfitLossCsvContent(report);
            var fileName = $"profit_loss_report_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.csv";
            
            return File(System.Text.Encoding.UTF8.GetBytes(csvContent), "text/csv", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting P&L report to CSV");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Export cash position report to CSV
    /// </summary>
    [HttpGet("cash-position/export/csv")]
    public async Task<IActionResult> ExportCashPositionReportToCsv(
        [FromQuery] DateTime? asOfDate = null)
    {
        try
        {
            var reportDate = asOfDate ?? DateTime.Today;
            _logger.LogInformation("Exporting cash position report to CSV as of {AsOfDate}", reportDate);

            var report = await _reportingService.GenerateCashPositionReportAsync(reportDate);
            var csvContent = GenerateCashPositionCsvContent(report);
            var fileName = $"cash_position_report_{reportDate:yyyyMMdd}.csv";
            
            return File(System.Text.Encoding.UTF8.GetBytes(csvContent), "text/csv", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting cash position report to CSV");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    private string GenerateProfitLossCsvContent(ProfitLossReportDto report)
    {
        var csv = new System.Text.StringBuilder();
        
        // Header
        csv.AppendLine("Profit & Loss Report");
        csv.AppendLine($"Period: {report.FromDate:yyyy-MM-dd} to {report.ToDate:yyyy-MM-dd}");
        csv.AppendLine($"Currency: {report.Currency}");
        csv.AppendLine($"Generated: {report.GeneratedAt:yyyy-MM-dd HH:mm:ss}");
        csv.AppendLine();
        
        // Revenue Section
        csv.AppendLine("REVENUE");
        csv.AppendLine("Category,Amount");
        foreach (var category in report.RevenueByCategory)
        {
            csv.AppendLine($"{category.Key},{category.Value:F2}");
        }
        csv.AppendLine($"Total Revenue,{report.TotalRevenue:F2}");
        csv.AppendLine();
        
        // Cost of Goods Sold Section
        csv.AppendLine("COST OF GOODS SOLD");
        csv.AppendLine("Category,Amount");
        foreach (var category in report.CostOfGoodsSoldByCategory)
        {
            csv.AppendLine($"{category.Key},{category.Value:F2}");
        }
        csv.AppendLine($"Total Cost of Goods Sold,{report.TotalCostOfGoodsSold:F2}");
        csv.AppendLine();
        
        // Gross Profit
        csv.AppendLine($"Gross Profit,{report.GrossProfit:F2}");
        csv.AppendLine($"Gross Profit Margin,{report.GrossProfitMargin:F2}%");
        csv.AppendLine();
        
        // Operating Expenses Section
        csv.AppendLine("OPERATING EXPENSES");
        csv.AppendLine("Category,Amount");
        foreach (var category in report.OperatingExpensesByCategory)
        {
            csv.AppendLine($"{category.Key},{category.Value:F2}");
        }
        csv.AppendLine($"Total Operating Expenses,{report.TotalOperatingExpenses:F2}");
        csv.AppendLine();
        
        // Net Profit
        csv.AppendLine($"Net Profit,{report.NetProfit:F2}");
        csv.AppendLine($"Net Profit Margin,{report.NetProfitMargin:F2}%");
        csv.AppendLine();
        
        // Stock Valuation
        csv.AppendLine("STOCK VALUATION");
        csv.AppendLine($"Opening Stock Value,{report.OpeningStockValue:F2}");
        csv.AppendLine($"Closing Stock Value,{report.ClosingStockValue:F2}");
        csv.AppendLine($"Stock Valuation Change,{report.StockValuationChange:F2}");
        
        return csv.ToString();
    }

    private string GenerateCashPositionCsvContent(CashPositionReportDto report)
    {
        var csv = new System.Text.StringBuilder();
        
        // Header
        csv.AppendLine("Cash Position Report");
        csv.AppendLine($"As of: {report.AsOfDate:yyyy-MM-dd}");
        csv.AppendLine($"Currency: {report.Currency}");
        csv.AppendLine($"Generated: {report.GeneratedAt:yyyy-MM-dd HH:mm:ss}");
        csv.AppendLine();
        
        // Current Position
        csv.AppendLine("CURRENT POSITION");
        csv.AppendLine($"Current Cash Balance,{report.CurrentCashBalance:F2}");
        csv.AppendLine($"Available Cash,{report.AvailableCash:F2}");
        csv.AppendLine($"Committed Cash,{report.CommittedCash:F2}");
        csv.AppendLine();
        
        // Cash Flow Summary
        csv.AppendLine("CASH FLOW SUMMARY");
        csv.AppendLine($"Cash Inflow,{report.CashInflow:F2}");
        csv.AppendLine($"Cash Outflow,{report.CashOutflow:F2}");
        csv.AppendLine($"Net Cash Flow,{report.NetCashFlow:F2}");
        csv.AppendLine();
        
        // Cash Inflow Breakdown
        csv.AppendLine("CASH INFLOW BY CATEGORY");
        csv.AppendLine("Category,Amount");
        foreach (var category in report.CashInflowByCategory)
        {
            csv.AppendLine($"{category.Key},{category.Value:F2}");
        }
        csv.AppendLine();
        
        // Cash Outflow Breakdown
        csv.AppendLine("CASH OUTFLOW BY CATEGORY");
        csv.AppendLine("Category,Amount");
        foreach (var category in report.CashOutflowByCategory)
        {
            csv.AppendLine($"{category.Key},{category.Value:F2}");
        }
        csv.AppendLine();
        
        // Liquidity Metrics
        csv.AppendLine("LIQUIDITY METRICS");
        csv.AppendLine($"Days of Cash on Hand,{report.DaysOfCashOnHand:F1}");
        csv.AppendLine($"Cash Flow Ratio,{report.CashFlowRatio:F2}");
        csv.AppendLine();
        
        // Recent Transactions
        csv.AppendLine("RECENT TRANSACTIONS");
        csv.AppendLine("Date,Type,Category,Amount,Description");
        foreach (var transaction in report.RecentTransactions.Take(10))
        {
            csv.AppendLine($"{transaction.TransactionDate:yyyy-MM-dd}," +
                          $"{transaction.Type}," +
                          $"{transaction.Category}," +
                          $"{transaction.Amount:F2}," +
                          $"\"{transaction.Description?.Replace("\"", "\"\"")}\"");
        }
        
        return csv.ToString();
    }
}


