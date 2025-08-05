using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehousesByValue;

public record GetWarehousesByValueQuery(decimal? MinValue, decimal? MaxValue) : IRequest<List<WarehouseDto>>; 
