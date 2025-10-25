using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.ArtificialIntelligence;

namespace Toss.Application.AI.Queries.GetAISettings;

public record AISettingsDto
{
    public int Id { get; init; }
    public bool Enabled { get; init; }
    public AIProviderType ProviderType { get; init; }
    public string? ApiKey { get; init; }
    public string? ApiEndpoint { get; init; }
    public int RequestTimeoutSeconds { get; init; }
    public List<string> SupportedLanguages { get; init; } = new();
    public bool AllowSalesForecasting { get; init; }
    public bool AllowInventoryPrediction { get; init; }
    public bool AllowBusinessInsights { get; init; }
    public bool AllowPriceSuggestions { get; init; }
    public bool AllowProductDescriptionGeneration { get; init; }
}

public record GetAISettingsQuery : IRequest<AISettingsDto?>
{
    public int ShopId { get; init; }
}

public class GetAISettingsQueryHandler : IRequestHandler<GetAISettingsQuery, AISettingsDto?>
{
    private readonly IApplicationDbContext _context;

    public GetAISettingsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AISettingsDto?> Handle(GetAISettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await _context.AISettings
            .Where(s => s.ShopId == request.ShopId)
            .Select(s => new AISettingsDto
            {
                Id = s.Id,
                Enabled = s.Enabled,
                ProviderType = s.ProviderType,
                ApiKey = s.ApiKey,
                ApiEndpoint = s.ApiEndpoint,
                RequestTimeoutSeconds = s.RequestTimeoutSeconds,
                SupportedLanguages = s.SupportedLanguages,
                AllowSalesForecasting = s.AllowSalesForecasting,
                AllowInventoryPrediction = s.AllowInventoryPrediction,
                AllowBusinessInsights = s.AllowBusinessInsights,
                AllowPriceSuggestions = s.AllowPriceSuggestions,
                AllowProductDescriptionGeneration = s.AllowProductDescriptionGeneration
            })
            .FirstOrDefaultAsync(cancellationToken);

        return settings;
    }
}

