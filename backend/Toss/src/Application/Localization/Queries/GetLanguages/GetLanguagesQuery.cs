using Toss.Application.Common.Interfaces;

namespace Toss.Application.Localization.Queries.GetLanguages;

public record LanguageDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string LanguageCulture { get; init; } = string.Empty;
    public string? FlagImageFileName { get; init; }
    public bool Published { get; init; }
    public int DisplayOrder { get; init; }
}

public record GetLanguagesQuery : IRequest<List<LanguageDto>>
{
    public bool? PublishedOnly { get; init; }
}

public class GetLanguagesQueryHandler : IRequestHandler<GetLanguagesQuery, List<LanguageDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLanguagesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<LanguageDto>> Handle(GetLanguagesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Languages.AsQueryable();

        if (request.PublishedOnly == true)
        {
            query = query.Where(l => l.Published);
        }

        var languages = await query
            .OrderBy(l => l.DisplayOrder)
            .Select(l => new LanguageDto
            {
                Id = l.Id,
                Name = l.Name,
                LanguageCulture = l.LanguageCulture,
                FlagImageFileName = l.FlagImageFileName,
                Published = l.Published,
                DisplayOrder = l.DisplayOrder
            })
            .ToListAsync(cancellationToken);

        return languages;
    }
}

