using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehouseByCode;

public record GetWarehouseByCodeQuery(string Code) : IRequest<WarehouseDto?>; 
