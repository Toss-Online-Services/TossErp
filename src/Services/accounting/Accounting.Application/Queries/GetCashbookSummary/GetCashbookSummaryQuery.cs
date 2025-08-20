using MediatR;
using TossErp.Accounting.Application.Common.DTOs;

namespace TossErp.Accounting.Application.Queries.GetCashbookSummary;

/// <summary>
/// Query to get cashbook summary
/// </summary>
public class GetCashbookSummaryQuery : IRequest<CashbookSummaryResponse>
{
    public DateTime AsOfDate { get; init; } = DateTime.Today;
}


