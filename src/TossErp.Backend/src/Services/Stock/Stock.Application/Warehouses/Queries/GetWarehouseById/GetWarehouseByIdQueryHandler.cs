using MediatR;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Entities;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehouseById;

public class GetWarehouseByIdQueryHandler : IRequestHandler<GetWarehouseByIdQuery, WarehouseDto?>
{
    private readonly IRepository<WarehouseAggregate> _repository;

    public GetWarehouseByIdQueryHandler(IRepository<WarehouseAggregate> repository)
    {
        _repository = repository;
    }

    public async Task<WarehouseDto?> Handle(GetWarehouseByIdQuery request, CancellationToken cancellationToken)
    {
        var warehouse = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (warehouse == null)
        {
            return null;
        }

        return new WarehouseDto
        {
            Id = warehouse.Id,
            Code = warehouse.Code.Value,
            Name = warehouse.Name,
            Description = warehouse.Description,
            IsGroup = warehouse.IsGroup,
            Company = warehouse.Company,
            Account = warehouse.Account,
            IsRejectedWarehouse = warehouse.IsRejectedWarehouse,
            WarehouseType = warehouse.WarehouseType,
            DefaultInTransitWarehouse = warehouse.DefaultInTransitWarehouse,
            IsDisabled = warehouse.IsDisabled,
            EmailId = warehouse.EmailId,
            PhoneNo = warehouse.PhoneNo,
            MobileNo = warehouse.MobileNo,
            AddressLine1 = warehouse.AddressLine1,
            AddressLine2 = warehouse.AddressLine2,
            City = warehouse.City,
            State = warehouse.State,
            Pin = warehouse.Pin,
            Country = warehouse.Country,
            Lft = warehouse.Lft,
            Rgt = warehouse.Rgt
        };
    }
} 
