using System.ComponentModel.DataAnnotations;

namespace TossErp.Configuration;

/// <summary>
/// Event Bus configuration options
/// </summary>
public class EventBusOptions
{
    public const string SectionName = "EventBus";

    /// <summary>
    /// RabbitMQ connection string
    /// </summary>
    [Required]
    public string ConnectionString { get; set; } = "amqp://guest:guest@localhost:5672";

    /// <summary>
    /// Username for RabbitMQ connection
    /// </summary>
    [Required]
    public string UserName { get; set; } = "guest";

    /// <summary>
    /// Password for RabbitMQ connection
    /// </summary>
    [Required]
    public string Password { get; set; } = "guest";

    /// <summary>
    /// Number of retry attempts for failed messages
    /// </summary>
    [Range(0, 10)]
    public int RetryCount { get; set; } = 5;

    /// <summary>
    /// Service name for subscription client
    /// </summary>
    [Required]
    public string SubscriptionClientName { get; set; } = "DefaultService";

    /// <summary>
    /// Exchange name prefix
    /// </summary>
    public string ExchangePrefix { get; set; } = "toss";

    /// <summary>
    /// Enable message persistence
    /// </summary>
    public bool EnablePersistence { get; set; } = true;

    /// <summary>
    /// Message time-to-live in minutes
    /// </summary>
    [Range(1, 1440)]
    public int MessageTtlMinutes { get; set; } = 60;
}
