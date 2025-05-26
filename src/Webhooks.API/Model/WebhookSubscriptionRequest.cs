namespace Webhooks.API.Model;

public class WebhookSubscriptionRequest : IValidatableObject
{
    public required string Url { get; init; }
    public string? Token { get; init; }
    public required string Event { get; init; }
    public required string GrantUrl { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!Uri.IsWellFormedUriString(GrantUrl, UriKind.Absolute))
        {
            yield return new ValidationResult("GrantUrl is not valid", new[] { nameof(GrantUrl) });
        }

        if (!Uri.IsWellFormedUriString(Url, UriKind.Absolute))
        {
            yield return new ValidationResult("Url is not valid", new[] { nameof(Url) });
        }

        var isOk = Enum.TryParse(Event, ignoreCase: true, result: out WebhookType whtype);
        if (!isOk)
        {
            yield return new ValidationResult($"{Event} is invalid event name", new[] { nameof(Event) });
        }
    }
}
