using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Common.Models;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Assets.Queries.GetAssets;

public record GetAssetsQuery : IRequest<PaginatedList<AssetDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public AssetCondition? Condition { get; init; }
    public int? ShopId { get; init; }
    public string? Category { get; init; }
    public bool? IsActive { get; init; }
}

public class GetAssetsQueryHandler : IRequestHandler<GetAssetsQuery, PaginatedList<AssetDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetAssetsQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<PaginatedList<AssetDto>> Handle(GetAssetsQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var query = _context.Assets
            .Include(a => a.Shop)
            .Where(a => a.BusinessId == _businessContext.CurrentBusinessId!.Value)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(a =>
                a.Name.ToLower().Contains(searchTerm) ||
                (a.Code != null && a.Code.ToLower().Contains(searchTerm)) ||
                (a.Brand != null && a.Brand.ToLower().Contains(searchTerm)) ||
                (a.Model != null && a.Model.ToLower().Contains(searchTerm)) ||
                (a.SerialNumber != null && a.SerialNumber.ToLower().Contains(searchTerm)) ||
                a.Location.ToLower().Contains(searchTerm));
        }

        if (request.Condition.HasValue)
        {
            query = query.Where(a => a.Condition == request.Condition.Value);
        }

        if (request.ShopId.HasValue)
        {
            query = query.Where(a => a.ShopId == request.ShopId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            query = query.Where(a => a.Category == request.Category);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(a => a.IsActive == request.IsActive.Value);
        }

        query = query.OrderByDescending(a => a.Created);

        var assetQuery = query
            .Select(a => new AssetDto
            {
                Id = a.Id,
                Name = a.Name,
                Code = a.Code,
                Value = a.Value,
                PurchaseCost = a.PurchaseCost,
                PurchaseDate = a.PurchaseDate,
                Location = a.Location,
                ShopId = a.ShopId,
                ShopName = a.Shop != null ? a.Shop.Name : null,
                Condition = a.Condition,
                Category = a.Category,
                Brand = a.Brand,
                Model = a.Model,
                SerialNumber = a.SerialNumber,
                Notes = a.Notes,
                IsActive = a.IsActive,
                Created = a.Created
            });

        return await PaginatedList<AssetDto>.CreateAsync(assetQuery, request.PageNumber, request.PageSize, cancellationToken);
    }
}

public record AssetDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Code { get; init; }
    public decimal Value { get; init; }
    public decimal? PurchaseCost { get; init; }
    public DateTimeOffset? PurchaseDate { get; init; }
    public string Location { get; init; } = string.Empty;
    public int? ShopId { get; init; }
    public string? ShopName { get; init; }
    public AssetCondition Condition { get; init; }
    public string? Category { get; init; }
    public string? Brand { get; init; }
    public string? Model { get; init; }
    public string? SerialNumber { get; init; }
    public string? Notes { get; init; }
    public bool IsActive { get; init; }
    public DateTimeOffset Created { get; init; }
}

