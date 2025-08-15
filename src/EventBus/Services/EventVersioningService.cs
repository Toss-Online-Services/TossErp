using System.Text.Json;
using eShop.EventBus.Events;
using Microsoft.Extensions.Logging;

namespace eShop.EventBus.Services;

/// <summary>
/// Service for handling event versioning and backward compatibility
/// </summary>
public class EventVersioningService : IEventVersioningService
{
    private readonly ILogger<EventVersioningService> _logger;
    private readonly Dictionary<string, IEventVersionConverter> _converters;

    public EventVersioningService(ILogger<EventVersioningService> logger)
    {
        _logger = logger;
        _converters = new Dictionary<string, IEventVersionConverter>();
        RegisterConverters();
    }

    public IntegrationEvent? ConvertToLatestVersion(string eventType, string eventData, int sourceVersion)
    {
        try
        {
            if (!_converters.TryGetValue(eventType, out var converter))
            {
                _logger.LogWarning("No converter found for event type {EventType}", eventType);
                return null;
            }

            var convertedEvent = converter.ConvertToLatestVersion(eventData, sourceVersion);
            _logger.LogInformation("Successfully converted event {EventType} from version {SourceVersion} to latest", 
                eventType, sourceVersion);

            return convertedEvent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting event {EventType} from version {SourceVersion}", eventType, sourceVersion);
            return null;
        }
    }

    public int GetLatestVersion(string eventType)
    {
        if (_converters.TryGetValue(eventType, out var converter))
        {
            return converter.LatestVersion;
        }

        return 1; // Default to version 1 if no converter found
    }

    private void RegisterConverters()
    {
        // Register converters for different event types
        // Example: _converters["ItemCreatedIntegrationEvent"] = new ItemCreatedEventConverter();
        
        _logger.LogInformation("Registered {Count} event version converters", _converters.Count);
    }
}

/// <summary>
/// Interface for event versioning service
/// </summary>
public interface IEventVersioningService
{
    IntegrationEvent? ConvertToLatestVersion(string eventType, string eventData, int sourceVersion);
    int GetLatestVersion(string eventType);
}

/// <summary>
/// Interface for event version converters
/// </summary>
public interface IEventVersionConverter
{
    int LatestVersion { get; }
    IntegrationEvent ConvertToLatestVersion(string eventData, int sourceVersion);
}

/// <summary>
/// Example converter for ItemCreatedIntegrationEvent
/// </summary>
public class ItemCreatedEventConverter : IEventVersionConverter
{
    public int LatestVersion => 2;

    public IntegrationEvent ConvertToLatestVersion(string eventData, int sourceVersion)
    {
        return sourceVersion switch
        {
            1 => ConvertFromV1ToV2(eventData),
            2 => JsonSerializer.Deserialize<eShop.EventBus.Events.Stock.ItemCreatedIntegrationEvent>(eventData)!,
            _ => throw new NotSupportedException($"Version {sourceVersion} is not supported")
        };
    }

    private IntegrationEvent ConvertFromV1ToV2(string eventData)
    {
        // Example conversion logic
        // In a real implementation, you would deserialize the old version and create a new version
        // with any new required fields set to default values
        
        var v1Event = JsonSerializer.Deserialize<ItemCreatedV1>(eventData);
        if (v1Event == null)
            throw new InvalidOperationException("Failed to deserialize V1 event");

        // Convert to V2 with new fields
        return new eShop.EventBus.Events.Stock.ItemCreatedIntegrationEvent(
            v1Event.ItemId,
            v1Event.ItemCode,
            v1Event.ItemName,
            v1Event.Description,
            v1Event.Category,
            v1Event.Unit,
            v1Event.StandardRate,
            v1Event.MinimumPrice,
            v1Event.WeightPerUnit,
            v1Event.Length,
            v1Event.Width,
            v1Event.Height,
            v1Event.IsActive,
            v1Event.CreatedAt,
            v1Event.CreatedBy);
    }
}

/// <summary>
/// Example V1 version of ItemCreatedIntegrationEvent
/// </summary>
public class ItemCreatedV1
{
    public Guid ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal StandardRate { get; set; }
    public decimal MinimumPrice { get; set; }
    public decimal? WeightPerUnit { get; set; }
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}
