using MediatR;

namespace TossErp.Stock.Application.Warehouses.Commands.DeleteWarehouse;

public record DeleteWarehouseCommand(Guid Id) : IRequest; 
