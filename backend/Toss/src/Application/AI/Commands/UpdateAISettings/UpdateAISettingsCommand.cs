using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.ArtificialIntelligence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.AI.Commands.UpdateAISettings;

public record UpdateAISettingsCommand : IRequest
{
    public int ShopId { get; init; }
    public bool Enabled { get; init; }
    public string ProviderType { get; init; } = "OpenAI";
    public string? ApiKey { get; init; }
    public string? ApiEndpoint { get; init; }
    public int RequestTimeoutSeconds { get; init; } = 30;
    public bool AllowSalesForecasting { get; init; }
    public bool AllowInventoryPrediction { get; init; }
    public bool AllowBusinessInsights { get; init; }
    public bool AllowPriceSuggestions { get; init; }
    public bool AllowProductDescriptionGeneration { get; init; }
    public List<string> SupportedLanguages { get; init; } = new();
}

public class UpdateAISettingsCommandHandler : IRequestHandler<UpdateAISettingsCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateAISettingsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateAISettingsCommand request, CancellationToken cancellationToken)
    {
        var settings = await _context.AISettings
            .FirstOrDefaultAsync(s => s.ShopId == request.ShopId, cancellationToken);

        if (settings == null)
        {
            settings = new AISettings
            {
                ShopId = request.ShopId
            };
            _context.AISettings.Add(settings);
        }

        settings.Enabled = request.Enabled;
        settings.ProviderType = Enum.Parse<AIProviderType>(request.ProviderType);
        settings.ApiKey = request.ApiKey;
        settings.ApiEndpoint = request.ApiEndpoint;
        settings.RequestTimeoutSeconds = request.RequestTimeoutSeconds;
        settings.AllowSalesForecasting = request.AllowSalesForecasting;
        settings.AllowInventoryPrediction = request.AllowInventoryPrediction;
        settings.AllowBusinessInsights = request.AllowBusinessInsights;
        settings.AllowPriceSuggestions = request.AllowPriceSuggestions;
        settings.AllowProductDescriptionGeneration = request.AllowProductDescriptionGeneration;
        settings.SupportedLanguages = request.SupportedLanguages;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

