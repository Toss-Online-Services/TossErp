using MediatR;

namespace TossErp.Stock.Application.Warehouses.Commands.UpdateWarehouse;

public record UpdateWarehouseCommand : IRequest<Guid>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? Address { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? Country { get; init; }
    public string? PostalCode { get; init; }
    public bool IsActive { get; init; }
} 
