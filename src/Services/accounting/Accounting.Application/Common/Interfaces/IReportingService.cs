using TossErp.Accounting.Application.Common.DTOs;

namespace TossErp.Accounting.Application.Common.Interfaces;

/// <summary>
/// Service for generating accounting reports
/// </summary>
public interface IReportingService
{
    /// <summary>
    /// Generate Profit & Loss summary for a given period
    /// </summary>
    Task<ProfitLossReportDto> GenerateProfitLossReportAsync(
        DateTime fromDate, 
        DateTime toDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate cash position report as of a specific date
    /// </summary>
    Task<CashPositionReportDto> GenerateCashPositionReportAsync(
        DateTime asOfDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate month-over-month comparison report
    /// </summary>
    Task<MonthOverMonthReportDto> GenerateMonthOverMonthReportAsync(
        DateTime fromDate, 
        DateTime toDate, 
        CancellationToken cancellationToken = default);
}

