using MediatR;
using TossErp.Sales.Application.Common.DTOs;

namespace TossErp.Sales.Application.Queries.GetDailySales;

/// <summary>
/// Query to get daily sales summary
/// </summary>
public class GetDailySalesQuery : IRequest<DailySalesSummaryDto>
{
    public DateTime Date { get; set; } = DateTime.Today;
    public Guid? TillId { get; set; }
}
