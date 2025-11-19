using Toss.Application.Common.Interfaces;

namespace Toss.Application.Localization.Queries.GetLocalizedStrings;

public record LocalizedStringDto
{
    public int Id { get; init; }
    public string ResourceName { get; init; } = string.Empty;
    public string ResourceValue { get; init; } = string.Empty;
}

public record GetLocalizedStringsQuery : IRequest<List<LocalizedStringDto>>
{
    public int LanguageId { get; init; }
    public string? ResourceNameFilter { get; init; }
}

public class GetLocalizedStringsQueryHandler : IRequestHandler<GetLocalizedStringsQuery, List<LocalizedStringDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLocalizedStringsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<LocalizedStringDto>> Handle(GetLocalizedStringsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.LocaleStringResources
            .Where(r => r.LanguageId == request.LanguageId);

        if (!string.IsNullOrWhiteSpace(request.ResourceNameFilter))
        {
            query = query.Where(r => r.ResourceName.Contains(request.ResourceNameFilter));
        }

        var resources = await query
            .OrderBy(r => r.ResourceName)
            .Select(r => new LocalizedStringDto
            {
                Id = r.Id,
                ResourceName = r.ResourceName,
                ResourceValue = r.ResourceValue
            })
            .ToListAsync(cancellationToken);

        return resources;
    }
}

