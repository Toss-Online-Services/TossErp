using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Warehouses.Commands.CreateWarehouse;

public record CreateWarehouseCommand : IRequest<WarehouseDto>
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Company { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsGroup { get; init; }
    public string? Account { get; init; }
    public bool IsRejectedWarehouse { get; init; }
    public string? WarehouseType { get; init; }
    public string? DefaultInTransitWarehouse { get; init; }
    public bool IsDisabled { get; init; }
    
    // Contact Information
    public string? EmailId { get; init; }
    public string? PhoneNo { get; init; }
    public string? MobileNo { get; init; }
    
    // Address Information
    public string? AddressLine1 { get; init; }
    public string? AddressLine2 { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? Pin { get; init; }
    public string? Country { get; init; }
} 
