using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehouseByName;

public record GetWarehouseByNameQuery(string Name) : IRequest<WarehouseDto?>; 