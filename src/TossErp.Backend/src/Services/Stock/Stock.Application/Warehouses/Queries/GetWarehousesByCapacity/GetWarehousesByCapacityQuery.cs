using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehousesByCapacity;

public record GetWarehousesByCapacityQuery(decimal? MinCapacity, decimal? MaxCapacity) : IRequest<List<WarehouseDto>>; 
