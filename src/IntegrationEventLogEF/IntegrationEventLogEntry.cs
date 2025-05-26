using System;
using System.Text.Json;

namespace IntegrationEventLogEF;

public class IntegrationEventLogEntry
{
    private static readonly JsonSerializerOptions s_indentedOptions = new() { WriteIndented = true };
    private static readonly JsonSerializerOptions s_caseInsensitiveOptions = new() { PropertyNameCaseInsensitive = true };

    private IntegrationEventLogEntry() { }
    public IntegrationEventLogEntry(object integrationEvent, string? transactionId = null)
    {
        IntegrationEvent = integrationEvent;
        EventTypeName = integrationEvent.GetType().FullName ?? throw new ArgumentNullException(nameof(integrationEvent));
        Content = JsonSerializer.Serialize(integrationEvent, s_indentedOptions);
        State = EventStateEnum.NotPublished.ToString();
        TimesSent = 0;
        TransactionId = transactionId;
        CreationTime = DateTime.UtcNow;
    }
    public Guid EventId { get; private set; }
    public required string EventTypeName { get; set; }
    [NotMapped]
    public string EventTypeShortName => EventTypeName.Split('.')?.Last();
    public required object IntegrationEvent { get; set; }
    public string? State { get; set; }
    public int TimesSent { get; set; }
    public DateTime CreationTime { get; set; }
    public required string Content { get; set; }
    public string? TransactionId { get; set; }

    public IntegrationEventLogEntry DeserializeJsonContent(Type type)
    {
        IntegrationEvent = JsonSerializer.Deserialize(Content, type, s_caseInsensitiveOptions) as IntegrationEvent;
        return this;
    }
}
