using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehousesByItem;

public record GetWarehousesByItemQuery(Guid ItemId) : IRequest<List<WarehouseDto>>; 
