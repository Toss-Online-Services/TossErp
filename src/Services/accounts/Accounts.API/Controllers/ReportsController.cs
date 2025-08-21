using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Accounts.Application.Queries.GetBalanceSheet;
using Accounts.Application.Queries.GetIncomeStatement;
using Accounts.Application.Queries.GetCashFlowStatement;
using Accounts.Application.Queries.GetTrialBalance;
using Accounts.Application.Queries.GetGeneralLedger;
using Accounts.Application.Queries.GetAccountAging;
using Accounts.Application.Queries.GetFinancialRatios;
using Accounts.Application.Queries.GetBudgetVariance;
using Accounts.Application.DTOs;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Accounts.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(IMediator mediator, ILogger<ReportsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Generate Balance Sheet report
    /// </summary>
    [HttpGet("balance-sheet")]
    [Authorize(Roles = "Admin,AccountManager,Accountant,Viewer")]
    public async Task<ActionResult<BalanceSheetDto>> GetBalanceSheet(
        [FromQuery] DateTime asOfDate,
        [FromQuery] bool includeZeroBalances = false,
        [FromQuery] bool consolidateSubAccounts = true)
    {
        try
        {
            var query = new GetBalanceSheetQuery
            {
                AsOfDate = asOfDate,
                IncludeZeroBalances = includeZeroBalances,
                ConsolidateSubAccounts = consolidateSubAccounts
            };

            var balanceSheet = await _mediator.Send(query);

            return Ok(balanceSheet);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating balance sheet");
            return StatusCode(500, "An error occurred while generating the balance sheet");
        }
    }

    /// <summary>
    /// Generate Income Statement (Profit & Loss) report
    /// </summary>
    [HttpGet("income-statement")]
    [Authorize(Roles = "Admin,AccountManager,Accountant,Viewer")]
    public async Task<ActionResult<IncomeStatementDto>> GetIncomeStatement(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        [FromQuery] bool includeZeroBalances = false,
        [FromQuery] bool consolidateSubAccounts = true)
    {
        try
        {
            var query = new GetIncomeStatementQuery
            {
                FromDate = fromDate,
                ToDate = toDate,
                IncludeZeroBalances = includeZeroBalances,
                ConsolidateSubAccounts = consolidateSubAccounts
            };

            var incomeStatement = await _mediator.Send(query);

            return Ok(incomeStatement);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating income statement");
            return StatusCode(500, "An error occurred while generating the income statement");
        }
    }

    /// <summary>
    /// Generate Cash Flow Statement report
    /// </summary>
    [HttpGet("cash-flow")]
    [Authorize(Roles = "Admin,AccountManager,Accountant,Viewer")]
    public async Task<ActionResult<CashFlowStatementDto>> GetCashFlowStatement(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        [FromQuery] string method = "indirect")
    {
        try
        {
            var query = new GetCashFlowStatementQuery
            {
                FromDate = fromDate,
                ToDate = toDate,
                Method = method
            };

            var cashFlowStatement = await _mediator.Send(query);

            return Ok(cashFlowStatement);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating cash flow statement");
            return StatusCode(500, "An error occurred while generating the cash flow statement");
        }
    }

    /// <summary>
    /// Generate Trial Balance report
    /// </summary>
    [HttpGet("trial-balance")]
    [Authorize(Roles = "Admin,AccountManager,Accountant,Viewer")]
    public async Task<ActionResult<TrialBalanceDto>> GetTrialBalance(
        [FromQuery] DateTime asOfDate,
        [FromQuery] bool includeZeroBalances = false,
        [FromQuery] bool adjustedTrialBalance = false)
    {
        try
        {
            var query = new GetTrialBalanceQuery
            {
                AsOfDate = asOfDate,
                IncludeZeroBalances = includeZeroBalances,
                AdjustedTrialBalance = adjustedTrialBalance
            };

            var trialBalance = await _mediator.Send(query);

            return Ok(trialBalance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating trial balance");
            return StatusCode(500, "An error occurred while generating the trial balance");
        }
    }

    /// <summary>
    /// Generate General Ledger report
    /// </summary>
    [HttpGet("general-ledger")]
    [Authorize(Roles = "Admin,AccountManager,Accountant,Viewer")]
    public async Task<ActionResult<GeneralLedgerDto>> GetGeneralLedger(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        [FromQuery] Guid? accountId = null,
        [FromQuery] bool includeZeroBalances = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100)
    {
        try
        {
            var query = new GetGeneralLedgerQuery
            {
                FromDate = fromDate,
                ToDate = toDate,
                AccountId = accountId,
                IncludeZeroBalances = includeZeroBalances,
                Page = page,
                PageSize = pageSize
            };

            var generalLedger = await _mediator.Send(query);

            // Add pagination headers
            Response.Headers.Add("X-Total-Count", generalLedger.TotalTransactions.ToString());
            Response.Headers.Add("X-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());
            Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)generalLedger.TotalTransactions / pageSize)).ToString());

            return Ok(generalLedger);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating general ledger");
            return StatusCode(500, "An error occurred while generating the general ledger");
        }
    }

    /// <summary>
    /// Generate Account Aging report
    /// </summary>
    [HttpGet("account-aging")]
    [Authorize(Roles = "Admin,AccountManager,Accountant,Viewer")]
    public async Task<ActionResult<AccountAgingDto>> GetAccountAging(
        [FromQuery] DateTime asOfDate,
        [FromQuery] string agingType = "receivables",
        [FromQuery] int[] agingPeriods = null)
    {
        try
        {
            var query = new GetAccountAgingQuery
            {
                AsOfDate = asOfDate,
                AgingType = agingType,
                AgingPeriods = agingPeriods ?? new[] { 30, 60, 90, 120 }
            };

            var accountAging = await _mediator.Send(query);

            return Ok(accountAging);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating account aging report");
            return StatusCode(500, "An error occurred while generating the account aging report");
        }
    }

    /// <summary>
    /// Generate Financial Ratios report
    /// </summary>
    [HttpGet("financial-ratios")]
    [Authorize(Roles = "Admin,AccountManager,Accountant,Viewer")]
    public async Task<ActionResult<FinancialRatiosDto>> GetFinancialRatios(
        [FromQuery] DateTime asOfDate,
        [FromQuery] DateTime? comparisonDate = null)
    {
        try
        {
            var query = new GetFinancialRatiosQuery
            {
                AsOfDate = asOfDate,
                ComparisonDate = comparisonDate
            };

            var financialRatios = await _mediator.Send(query);

            return Ok(financialRatios);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating financial ratios");
            return StatusCode(500, "An error occurred while generating the financial ratios");
        }
    }

    /// <summary>
    /// Generate Budget Variance report
    /// </summary>
    [HttpGet("budget-variance")]
    [Authorize(Roles = "Admin,AccountManager,Accountant,Viewer")]
    public async Task<ActionResult<BudgetVarianceDto>> GetBudgetVariance(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        [FromQuery] Guid? budgetId = null,
        [FromQuery] bool includeZeroVariances = false)
    {
        try
        {
            var query = new GetBudgetVarianceQuery
            {
                FromDate = fromDate,
                ToDate = toDate,
                BudgetId = budgetId,
                IncludeZeroVariances = includeZeroVariances
            };

            var budgetVariance = await _mediator.Send(query);

            return Ok(budgetVariance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating budget variance report");
            return StatusCode(500, "An error occurred while generating the budget variance report");
        }
    }

    /// <summary>
    /// Export Balance Sheet to Excel
    /// </summary>
    [HttpGet("balance-sheet/export/excel")]
    [Authorize(Roles = "Admin,AccountManager,Accountant,Viewer")]
    public async Task<IActionResult> ExportBalanceSheetToExcel(
        [FromQuery] DateTime asOfDate,
        [FromQuery] bool includeZeroBalances = false,
        [FromQuery] bool consolidateSubAccounts = true)
    {
        try
        {
            var query = new GetBalanceSheetQuery
            {
                AsOfDate = asOfDate,
                IncludeZeroBalances = includeZeroBalances,
                ConsolidateSubAccounts = consolidateSubAccounts
            };

            var balanceSheet = await _mediator.Send(query);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Balance Sheet");

            // Title
            worksheet.Cell(1, 1).Value = "Balance Sheet";
            worksheet.Cell(2, 1).Value = $"As of {asOfDate:yyyy-MM-dd}";
            worksheet.Range(1, 1, 1, 3).Merge();
            worksheet.Range(2, 1, 2, 3).Merge();

            var titleStyle = worksheet.Range(1, 1, 2, 3);
            titleStyle.Style.Font.Bold = true;
            titleStyle.Style.Font.FontSize = 14;
            titleStyle.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            int currentRow = 4;

            // Assets
            worksheet.Cell(currentRow, 1).Value = "ASSETS";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            currentRow += 2;

            foreach (var asset in balanceSheet.Assets)
            {
                worksheet.Cell(currentRow, 1).Value = asset.AccountName;
                worksheet.Cell(currentRow, 2).Value = asset.Balance;
                worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";
                currentRow++;
            }

            worksheet.Cell(currentRow, 1).Value = "Total Assets";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 2).Value = balanceSheet.TotalAssets;
            worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";
            currentRow += 2;

            // Liabilities
            worksheet.Cell(currentRow, 1).Value = "LIABILITIES";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            currentRow += 2;

            foreach (var liability in balanceSheet.Liabilities)
            {
                worksheet.Cell(currentRow, 1).Value = liability.AccountName;
                worksheet.Cell(currentRow, 2).Value = liability.Balance;
                worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";
                currentRow++;
            }

            worksheet.Cell(currentRow, 1).Value = "Total Liabilities";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 2).Value = balanceSheet.TotalLiabilities;
            worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";
            currentRow += 2;

            // Equity
            worksheet.Cell(currentRow, 1).Value = "EQUITY";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            currentRow += 2;

            foreach (var equity in balanceSheet.Equity)
            {
                worksheet.Cell(currentRow, 1).Value = equity.AccountName;
                worksheet.Cell(currentRow, 2).Value = equity.Balance;
                worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";
                currentRow++;
            }

            worksheet.Cell(currentRow, 1).Value = "Total Equity";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 2).Value = balanceSheet.TotalEquity;
            worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";
            currentRow += 2;

            worksheet.Cell(currentRow, 1).Value = "Total Liabilities & Equity";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 2).Value = balanceSheet.TotalLiabilitiesAndEquity;
            worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"Balance_Sheet_{asOfDate:yyyyMMdd}.xlsx";
            
            _logger.LogInformation("Exported balance sheet to Excel");

            return File(stream.ToArray(), 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting balance sheet to Excel");
            return StatusCode(500, "An error occurred while exporting the balance sheet");
        }
    }

    /// <summary>
    /// Export Income Statement to Excel
    /// </summary>
    [HttpGet("income-statement/export/excel")]
    [Authorize(Roles = "Admin,AccountManager,Accountant,Viewer")]
    public async Task<IActionResult> ExportIncomeStatementToExcel(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        [FromQuery] bool includeZeroBalances = false,
        [FromQuery] bool consolidateSubAccounts = true)
    {
        try
        {
            var query = new GetIncomeStatementQuery
            {
                FromDate = fromDate,
                ToDate = toDate,
                IncludeZeroBalances = includeZeroBalances,
                ConsolidateSubAccounts = consolidateSubAccounts
            };

            var incomeStatement = await _mediator.Send(query);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Income Statement");

            // Title
            worksheet.Cell(1, 1).Value = "Income Statement";
            worksheet.Cell(2, 1).Value = $"For the period {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}";
            worksheet.Range(1, 1, 1, 3).Merge();
            worksheet.Range(2, 1, 2, 3).Merge();

            var titleStyle = worksheet.Range(1, 1, 2, 3);
            titleStyle.Style.Font.Bold = true;
            titleStyle.Style.Font.FontSize = 14;
            titleStyle.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            int currentRow = 4;

            // Revenue
            worksheet.Cell(currentRow, 1).Value = "REVENUE";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            currentRow += 2;

            foreach (var revenue in incomeStatement.Revenue)
            {
                worksheet.Cell(currentRow, 1).Value = revenue.AccountName;
                worksheet.Cell(currentRow, 2).Value = revenue.Amount;
                worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";
                currentRow++;
            }

            worksheet.Cell(currentRow, 1).Value = "Total Revenue";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 2).Value = incomeStatement.TotalRevenue;
            worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";
            currentRow += 2;

            // Expenses
            worksheet.Cell(currentRow, 1).Value = "EXPENSES";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            currentRow += 2;

            foreach (var expense in incomeStatement.Expenses)
            {
                worksheet.Cell(currentRow, 1).Value = expense.AccountName;
                worksheet.Cell(currentRow, 2).Value = expense.Amount;
                worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";
                currentRow++;
            }

            worksheet.Cell(currentRow, 1).Value = "Total Expenses";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 2).Value = incomeStatement.TotalExpenses;
            worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";
            currentRow += 2;

            worksheet.Cell(currentRow, 1).Value = "Net Income";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 2).Value = incomeStatement.NetIncome;
            worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"Income_Statement_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.xlsx";
            
            _logger.LogInformation("Exported income statement to Excel");

            return File(stream.ToArray(), 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting income statement to Excel");
            return StatusCode(500, "An error occurred while exporting the income statement");
        }
    }

    /// <summary>
    /// Export Trial Balance to PDF
    /// </summary>
    [HttpGet("trial-balance/export/pdf")]
    [Authorize(Roles = "Admin,AccountManager,Accountant,Viewer")]
    public async Task<IActionResult> ExportTrialBalanceToPdf(
        [FromQuery] DateTime asOfDate,
        [FromQuery] bool includeZeroBalances = false,
        [FromQuery] bool adjustedTrialBalance = false)
    {
        try
        {
            var query = new GetTrialBalanceQuery
            {
                AsOfDate = asOfDate,
                IncludeZeroBalances = includeZeroBalances,
                AdjustedTrialBalance = adjustedTrialBalance
            };

            var trialBalance = await _mediator.Send(query);

            using var stream = new MemoryStream();
            var document = new Document(PageSize.A4, 50, 50, 25, 25);
            var writer = PdfWriter.GetInstance(document, stream);

            document.Open();

            // Title
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
            var title = new Paragraph($"{(adjustedTrialBalance ? "Adjusted " : "")}Trial Balance", titleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 10
            };
            document.Add(title);

            var subtitleFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
            var subtitle = new Paragraph($"As of {asOfDate:yyyy-MM-dd}", subtitleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20
            };
            document.Add(subtitle);

            // Table
            var table = new PdfPTable(3) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 50, 25, 25 });

            // Headers
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
            table.AddCell(new PdfPCell(new Phrase("Account", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
            table.AddCell(new PdfPCell(new Phrase("Debit", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = Element.ALIGN_RIGHT });
            table.AddCell(new PdfPCell(new Phrase("Credit", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = Element.ALIGN_RIGHT });

            // Data
            var cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
            foreach (var account in trialBalance.Accounts)
            {
                table.AddCell(new PdfPCell(new Phrase(account.AccountName, cellFont)));
                table.AddCell(new PdfPCell(new Phrase(account.DebitBalance > 0 ? account.DebitBalance.ToString("C") : "", cellFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                table.AddCell(new PdfPCell(new Phrase(account.CreditBalance > 0 ? account.CreditBalance.ToString("C") : "", cellFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
            }

            // Totals
            var totalFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
            table.AddCell(new PdfPCell(new Phrase("TOTAL", totalFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
            table.AddCell(new PdfPCell(new Phrase(trialBalance.TotalDebits.ToString("C"), totalFont)) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = Element.ALIGN_RIGHT });
            table.AddCell(new PdfPCell(new Phrase(trialBalance.TotalCredits.ToString("C"), totalFont)) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = Element.ALIGN_RIGHT });

            document.Add(table);

            // Balance check
            if (trialBalance.IsBalanced)
            {
                var balanceNote = new Paragraph("Trial balance is in balance.", FontFactory.GetFont(FontFactory.HELVETICA, 10))
                {
                    SpacingBefore = 10,
                    Alignment = Element.ALIGN_CENTER
                };
                document.Add(balanceNote);
            }
            else
            {
                var imbalanceNote = new Paragraph($"WARNING: Trial balance is out of balance by {Math.Abs(trialBalance.TotalDebits - trialBalance.TotalCredits):C}", 
                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.RED))
                {
                    SpacingBefore = 10,
                    Alignment = Element.ALIGN_CENTER
                };
                document.Add(imbalanceNote);
            }

            document.Close();

            var fileName = $"Trial_Balance_{asOfDate:yyyyMMdd}.pdf";
            
            _logger.LogInformation("Exported trial balance to PDF");

            return File(stream.ToArray(), "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting trial balance to PDF");
            return StatusCode(500, "An error occurred while exporting the trial balance");
        }
    }

    /// <summary>
    /// Get financial dashboard summary
    /// </summary>
    [HttpGet("dashboard")]
    [Authorize(Roles = "Admin,AccountManager,Accountant,Viewer")]
    public async Task<ActionResult<object>> GetFinancialDashboard([FromQuery] DateTime? asOfDate = null)
    {
        try
        {
            var reportDate = asOfDate ?? DateTime.UtcNow;
            var monthStart = new DateTime(reportDate.Year, reportDate.Month, 1);
            var yearStart = new DateTime(reportDate.Year, 1, 1);

            // Get basic financial statements
            var balanceSheetQuery = new GetBalanceSheetQuery { AsOfDate = reportDate };
            var incomeStatementQuery = new GetIncomeStatementQuery { FromDate = monthStart, ToDate = reportDate };
            var yearIncomeStatementQuery = new GetIncomeStatementQuery { FromDate = yearStart, ToDate = reportDate };
            var ratiosQuery = new GetFinancialRatiosQuery { AsOfDate = reportDate };

            var balanceSheet = await _mediator.Send(balanceSheetQuery);
            var monthlyIncome = await _mediator.Send(incomeStatementQuery);
            var yearlyIncome = await _mediator.Send(yearIncomeStatementQuery);
            var ratios = await _mediator.Send(ratiosQuery);

            var dashboard = new
            {
                AsOfDate = reportDate,
                BalanceSheet = new
                {
                    TotalAssets = balanceSheet.TotalAssets,
                    TotalLiabilities = balanceSheet.TotalLiabilities,
                    TotalEquity = balanceSheet.TotalEquity,
                    IsBalanced = balanceSheet.TotalAssets == balanceSheet.TotalLiabilitiesAndEquity
                },
                MonthlyPerformance = new
                {
                    Period = $"{monthStart:yyyy-MM-dd} to {reportDate:yyyy-MM-dd}",
                    TotalRevenue = monthlyIncome.TotalRevenue,
                    TotalExpenses = monthlyIncome.TotalExpenses,
                    NetIncome = monthlyIncome.NetIncome,
                    GrossMargin = monthlyIncome.TotalRevenue > 0 ? 
                        (monthlyIncome.TotalRevenue - monthlyIncome.TotalExpenses) / monthlyIncome.TotalRevenue * 100 : 0
                },
                YearlyPerformance = new
                {
                    Period = $"{yearStart:yyyy-MM-dd} to {reportDate:yyyy-MM-dd}",
                    TotalRevenue = yearlyIncome.TotalRevenue,
                    TotalExpenses = yearlyIncome.TotalExpenses,
                    NetIncome = yearlyIncome.NetIncome,
                    GrossMargin = yearlyIncome.TotalRevenue > 0 ? 
                        (yearlyIncome.TotalRevenue - yearlyIncome.TotalExpenses) / yearlyIncome.TotalRevenue * 100 : 0
                },
                KeyRatios = new
                {
                    CurrentRatio = ratios.LiquidityRatios?.CurrentRatio,
                    QuickRatio = ratios.LiquidityRatios?.QuickRatio,
                    DebtToEquityRatio = ratios.LeverageRatios?.DebtToEquityRatio,
                    ReturnOnAssets = ratios.ProfitabilityRatios?.ReturnOnAssets,
                    ReturnOnEquity = ratios.ProfitabilityRatios?.ReturnOnEquity
                }
            };

            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating financial dashboard");
            return StatusCode(500, "An error occurred while generating the financial dashboard");
        }
    }
}
