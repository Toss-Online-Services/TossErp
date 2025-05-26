namespace Webhooks.API.Model;

public class WebhookSubscription
{
    public int Id { get; set; }

    public required WebhookType Type { get; set; }
    public required DateTime Date { get; set; }
    public required string DestUrl { get; set; }
    public string? Token { get; set; }
    public required string UserId { get; set; }
}
