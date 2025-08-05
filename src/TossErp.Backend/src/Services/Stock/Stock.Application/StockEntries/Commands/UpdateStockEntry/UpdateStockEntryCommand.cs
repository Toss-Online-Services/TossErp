using MediatR;

namespace TossErp.Stock.Application.StockEntries.Commands.UpdateStockEntry;

public record UpdateStockEntryCommand : IRequest<Guid>
{
    public Guid Id { get; init; }
    public string StockEntryNo { get; init; } = string.Empty;
    public DateTime StockEntryDate { get; init; }
    public string ReferenceNo { get; init; } = string.Empty;
    public string ReferenceType { get; init; } = string.Empty;
    public Guid WarehouseId { get; init; }
    public string Notes { get; init; } = string.Empty;
    public bool IsActive { get; init; } = true;
} 
