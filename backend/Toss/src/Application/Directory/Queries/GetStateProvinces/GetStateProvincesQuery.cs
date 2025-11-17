using Toss.Application.Common.Interfaces;

namespace Toss.Application.Directory.Queries.GetStateProvinces;

public record StateProvinceDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Abbreviation { get; init; }
    public int CountryId { get; init; }
    public string CountryName { get; init; } = string.Empty;
    public bool Published { get; init; }
    public int DisplayOrder { get; init; }
}

public record GetStateProvincesQuery : IRequest<List<StateProvinceDto>>
{
    public int? CountryId { get; init; }
    public bool? PublishedOnly { get; init; }
}

public class GetStateProvincesQueryHandler : IRequestHandler<GetStateProvincesQuery, List<StateProvinceDto>>
{
    private readonly IApplicationDbContext _context;

    public GetStateProvincesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<StateProvinceDto>> Handle(GetStateProvincesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.StateProvinces.AsQueryable();

        if (request.CountryId.HasValue)
        {
            query = query.Where(s => s.CountryId == request.CountryId.Value);
        }

        if (request.PublishedOnly == true)
        {
            query = query.Where(s => s.Published);
        }

        var stateProvinces = await query
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Name)
            .Select(s => new StateProvinceDto
            {
                Id = s.Id,
                Name = s.Name,
                Abbreviation = s.Abbreviation,
                CountryId = s.CountryId,
                CountryName = s.Country != null ? s.Country.Name : string.Empty,
                Published = s.Published,
                DisplayOrder = s.DisplayOrder
            })
            .ToListAsync(cancellationToken);

        return stateProvinces;
    }
}

