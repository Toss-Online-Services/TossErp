using TossErp.Stock.Application.Common.Models;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Entities;

namespace TossErp.Stock.Application.Common.Mappings;

public static class MappingExtensions
{
    public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(this IQueryable<TDestination> queryable, int pageNumber, int pageSize, CancellationToken cancellationToken = default) where TDestination : class
        => PaginatedList<TDestination>.CreateAsync(queryable, pageNumber, pageSize, cancellationToken);

    public static Task<List<TDestination>> ProjectToListAsync<TDestination>(this IQueryable queryable, IConfigurationProvider configuration, CancellationToken cancellationToken = default) where TDestination : class
        => Task.FromResult(queryable.ProjectTo<TDestination>(configuration).ToList());

    public static WarehouseDto ToDto(this WarehouseAggregate warehouse)
    {
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
