namespace eShop.WebhookClient.Services;

public class WebhookSubscriptionRequest
{
    public required string Url { get; init; }
    public string? Token { get; init; }
    public required string Event { get; init; }
    public required string GrantUrl { get; init; }
}
