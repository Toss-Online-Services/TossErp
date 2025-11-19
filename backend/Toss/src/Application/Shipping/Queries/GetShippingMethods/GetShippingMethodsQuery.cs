using Toss.Application.Common.Interfaces;

namespace Toss.Application.Shipping.Queries.GetShippingMethods;

public record ShippingMethodDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int DisplayOrder { get; init; }
}

public record GetShippingMethodsQuery : IRequest<List<ShippingMethodDto>>
{
}

public class GetShippingMethodsQueryHandler : IRequestHandler<GetShippingMethodsQuery, List<ShippingMethodDto>>
{
    private readonly IApplicationDbContext _context;

    public GetShippingMethodsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ShippingMethodDto>> Handle(GetShippingMethodsQuery request, CancellationToken cancellationToken)
    {
        var methods = await _context.ShippingMethods
            .OrderBy(m => m.DisplayOrder)
            .ThenBy(m => m.Name)
            .Select(m => new ShippingMethodDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                DisplayOrder = m.DisplayOrder
            })
            .ToListAsync(cancellationToken);

        return methods;
    }
}

