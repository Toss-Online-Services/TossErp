namespace eShop.WebhookClient.Services;

public class WebHookReceived
{
    public required DateTime When { get; init; }

    public string? Data { get; init; }

    public string? Token { get; init; }
}
