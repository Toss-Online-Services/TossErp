using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehousesByBin;

public record GetWarehousesByBinQuery(Guid BinId) : IRequest<List<WarehouseDto>>; 
