using System.Text.Json;

namespace eShop.WebhookClient.Services;

public class WebhookData
{
    public required DateTime When { get; init; }

    public required string Payload { get; init; }

    public required string Type { get; init; }
}
