using Toss.Application.Common.Interfaces;

namespace Toss.Application.Directory.Queries.GetCurrencies;

public record CurrencyDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string CurrencyCode { get; init; } = string.Empty;
    public decimal Rate { get; init; }
    public string DisplayLocale { get; init; } = string.Empty;
    public string? CustomFormatting { get; init; }
    public bool Published { get; init; }
    public int DisplayOrder { get; init; }
}

public record GetCurrenciesQuery : IRequest<List<CurrencyDto>>
{
    public bool? PublishedOnly { get; init; }
}

public class GetCurrenciesQueryHandler : IRequestHandler<GetCurrenciesQuery, List<CurrencyDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCurrenciesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CurrencyDto>> Handle(GetCurrenciesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Currencies.AsQueryable();

        if (request.PublishedOnly == true)
        {
            query = query.Where(c => c.Published);
        }

        var currencies = await query
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .Select(c => new CurrencyDto
            {
                Id = c.Id,
                Name = c.Name,
                CurrencyCode = c.CurrencyCode,
                Rate = c.Rate,
                DisplayLocale = c.DisplayLocale,
                CustomFormatting = c.CustomFormatting,
                Published = c.Published,
                DisplayOrder = c.DisplayOrder
            })
            .ToListAsync(cancellationToken);

        return currencies;
    }
}

