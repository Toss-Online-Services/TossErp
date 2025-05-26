namespace eShop.WebhookClient.Services;

public class WebhookResponse
{
    public required DateTime Date { get; init; }
    public string? DestUrl { get; init; }
    public string? Token { get; init; }
}
