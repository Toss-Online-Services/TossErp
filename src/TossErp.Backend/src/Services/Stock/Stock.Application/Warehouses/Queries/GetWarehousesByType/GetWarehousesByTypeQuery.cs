using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehousesByType;

public record GetWarehousesByTypeQuery(string Type) : IRequest<List<WarehouseDto>>; 
