using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Bins.Commands.CreateBin;

public record CreateBinCommand : IRequest<BinDto>
{
    public string BinCode { get; init; } = string.Empty;
    public string BinName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Guid WarehouseId { get; init; }
    public string BinType { get; init; } = string.Empty;
    public string StorageLocation { get; init; } = string.Empty;
    public decimal Capacity { get; init; }
    public decimal UsedCapacity { get; init; }
    public decimal AvailableCapacity { get; init; }
    public bool IsDisabled { get; init; }
} 
