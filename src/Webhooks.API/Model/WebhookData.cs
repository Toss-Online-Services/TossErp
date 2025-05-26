namespace Webhooks.API.Model;

public class WebhookData
{
    public required DateTime When { get; init; }

    public required string Payload { get; init; }

    public required string Type { get; init; }

    public WebhookData(WebhookType hookType, object data)
    {
        When = DateTime.UtcNow;
        Type = hookType.ToString();
        Payload = JsonSerializer.Serialize(data);
    }
}
