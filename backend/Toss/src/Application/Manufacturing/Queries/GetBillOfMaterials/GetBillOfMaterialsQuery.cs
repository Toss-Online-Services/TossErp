using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Common.Mappings;
using Toss.Application.Common.Models;
using Toss.Domain.Entities.Manufacturing;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Manufacturing.Queries.GetBillOfMaterials;

public record GetBillOfMaterialsQuery : IRequest<PaginatedList<BillOfMaterialsDto>>
{
    public int? ProductId { get; init; }
    public bool? IsActive { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public record BillOfMaterialsDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public string? Version { get; init; }
    public string? Notes { get; init; }
    public int ComponentCount { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}

public class GetBillOfMaterialsQueryHandler : IRequestHandler<GetBillOfMaterialsQuery, PaginatedList<BillOfMaterialsDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetBillOfMaterialsQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<PaginatedList<BillOfMaterialsDto>> Handle(GetBillOfMaterialsQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var query = _context.BillOfMaterials
            .Include(b => b.Product)
            .Include(b => b.Components)
            .Where(b => b.BusinessId == _businessContext.CurrentBusinessId!.Value)
            .AsQueryable();

        if (request.ProductId.HasValue)
        {
            query = query.Where(b => b.ProductId == request.ProductId.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(b => b.IsActive == request.IsActive.Value);
        }

        return await query
            .OrderByDescending(b => b.Created)
            .Select(b => new BillOfMaterialsDto
            {
                Id = b.Id,
                ProductId = b.ProductId,
                ProductName = b.Product.Name,
                IsActive = b.IsActive,
                Version = b.Version,
                Notes = b.Notes,
                ComponentCount = b.Components.Count,
                CreatedAt = b.Created
            })
            .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}

