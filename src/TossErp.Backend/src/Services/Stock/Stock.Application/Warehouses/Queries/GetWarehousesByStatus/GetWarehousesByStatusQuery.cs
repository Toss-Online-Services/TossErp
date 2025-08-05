using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehousesByStatus;

public record GetWarehousesByStatusQuery(string Status) : IRequest<List<WarehouseDto>>; 
