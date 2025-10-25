using Toss.Application.Common.Interfaces;

namespace Toss.Application.Directory.Queries.GetCountries;

public record CountryDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string TwoLetterIsoCode { get; init; } = string.Empty;
    public string ThreeLetterIsoCode { get; init; } = string.Empty;
    public int NumericIsoCode { get; init; }
    public bool Published { get; init; }
    public int DisplayOrder { get; init; }
}

public record GetCountriesQuery : IRequest<List<CountryDto>>
{
    public bool? PublishedOnly { get; init; }
}

public class GetCountriesQueryHandler : IRequestHandler<GetCountriesQuery, List<CountryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCountriesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CountryDto>> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Countries.AsQueryable();

        if (request.PublishedOnly == true)
        {
            query = query.Where(c => c.Published);
        }

        var countries = await query
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .Select(c => new CountryDto
            {
                Id = c.Id,
                Name = c.Name,
                TwoLetterIsoCode = c.TwoLetterIsoCode,
                ThreeLetterIsoCode = c.ThreeLetterIsoCode,
                NumericIsoCode = c.NumericIsoCode,
                Published = c.Published,
                DisplayOrder = c.DisplayOrder
            })
            .ToListAsync(cancellationToken);

        return countries;
    }
}

