using MediatR;

namespace TossErp.Stock.Application.Bins.Commands.UpdateBin;

public record UpdateBinCommand : IRequest<Guid>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Guid WarehouseId { get; init; }
    public bool IsActive { get; init; } = true;
} 
