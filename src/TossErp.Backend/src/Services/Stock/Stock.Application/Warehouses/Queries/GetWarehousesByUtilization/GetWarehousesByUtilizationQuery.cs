using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehousesByUtilization;

public record GetWarehousesByUtilizationQuery(decimal? MinUtilization, decimal? MaxUtilization) : IRequest<List<WarehouseDto>>; 
