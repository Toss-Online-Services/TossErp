using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Localization;

namespace Toss.Application.Localization.Commands.CreateLanguage;

public record CreateLanguageCommand : IRequest<int>
{
    public string Name { get; init; } = string.Empty;
    public string LanguageCulture { get; init; } = string.Empty;
    public string UniqueSeoCode { get; init; } = string.Empty;
    public string? FlagImageFileName { get; init; }
    public bool Rtl { get; init; }
    public bool Published { get; init; } = true;
    public int DisplayOrder { get; init; }
}

public class CreateLanguageCommandHandler : IRequestHandler<CreateLanguageCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateLanguageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateLanguageCommand request, CancellationToken cancellationToken)
    {
        var language = new Language
        {
            Name = request.Name,
            LanguageCulture = request.LanguageCulture,
            UniqueSeoCode = request.UniqueSeoCode,
            FlagImageFileName = request.FlagImageFileName,
            Rtl = request.Rtl,
            Published = request.Published,
            DisplayOrder = request.DisplayOrder
        };

        _context.Languages.Add(language);
        await _context.SaveChangesAsync(cancellationToken);

        return language.Id;
    }
}

