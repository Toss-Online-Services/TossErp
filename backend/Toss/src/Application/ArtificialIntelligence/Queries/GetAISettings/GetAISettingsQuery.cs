using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.ArtificialIntelligence;

namespace Toss.Application.ArtificialIntelligence.Queries.GetAISettings;

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
        var entity = await _context.AISettings
            .Where(s => s.ShopId == request.ShopId)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity == null)
            return null;

        // Get the appropriate API key based on provider type
        var apiKey = entity.ProviderType switch
        {
            AIProviderType.Gemini => entity.GeminiApiKey,
            AIProviderType.ChatGpt => entity.ChatGptApiKey,
            AIProviderType.DeepSeek => entity.DeepSeekApiKey,
            _ => null
        };

        return new AISettingsDto
        {
            Id = entity.Id,
            Enabled = entity.Enabled,
            ProviderType = entity.ProviderType,
            ApiKey = apiKey,
            ApiEndpoint = entity.ApiEndpoint,
            RequestTimeoutSeconds = entity.RequestTimeout ?? 30,
            SupportedLanguages = entity.SupportedLanguages,
            AllowSalesForecasting = entity.AllowSalesForecasting,
            AllowInventoryPrediction = entity.AllowInventoryPrediction,
            AllowBusinessInsights = entity.AllowBusinessInsights,
            AllowPriceSuggestions = entity.AllowPriceSuggestions,
            AllowProductDescriptionGeneration = entity.AllowProductDescriptionGeneration
        };
    }
}

