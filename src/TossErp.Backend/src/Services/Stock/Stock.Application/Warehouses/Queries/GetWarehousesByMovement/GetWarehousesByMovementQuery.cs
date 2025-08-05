using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehousesByMovement;

public record GetWarehousesByMovementQuery(Guid MovementId) : IRequest<List<WarehouseDto>>; 
