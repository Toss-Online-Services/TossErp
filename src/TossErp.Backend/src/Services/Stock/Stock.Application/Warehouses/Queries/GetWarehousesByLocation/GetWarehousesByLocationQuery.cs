using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehousesByLocation;

public record GetWarehousesByLocationQuery(string Location) : IRequest<List<WarehouseDto>>; 
