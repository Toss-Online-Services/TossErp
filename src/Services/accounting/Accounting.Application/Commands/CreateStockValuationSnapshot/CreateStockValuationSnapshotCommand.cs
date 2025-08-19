using MediatR;
using TossErp.Accounting.Domain.Enums;

namespace TossErp.Accounting.Application.Commands.CreateStockValuationSnapshot;

/// <summary>
/// Command to create a stock valuation snapshot for P&L reporting
/// </summary>
public class CreateStockValuationSnapshotCommand : IRequest<Guid>
{
    public DateTime SnapshotDate { get; init; }
    public ValuationMethod Method { get; init; }
    public string? Notes { get; init; }
}

