using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehouseById;

public record GetWarehouseByIdQuery(Guid Id) : IRequest<WarehouseDto>; 
