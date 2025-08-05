using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehouses;

public record GetWarehousesQuery : IRequest<List<WarehouseDto>>; 
