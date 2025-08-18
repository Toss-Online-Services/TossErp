using System.ComponentModel.DataAnnotations;

namespace TossErp.Configuration;

/// <summary>
/// Redis cache configuration options
/// </summary>
public class RedisOptions
{
    public const string SectionName = "Redis";

    /// <summary>
    /// Redis connection string
    /// </summary>
    [Required]
    public string ConnectionString { get; set; } = "localhost:6379";

    /// <summary>
    /// Database number to use
    /// </summary>
    [Range(0, 15)]
    public int Database { get; set; } = 0;

    /// <summary>
    /// Key prefix for this application
    /// </summary>
    public string KeyPrefix { get; set; } = "toss:";

    /// <summary>
    /// Default cache expiration in minutes
    /// </summary>
    [Range(1, 10080)]
    public int DefaultExpirationMinutes { get; set; } = 60;

    /// <summary>
    /// Enable compression for cached values
    /// </summary>
    public bool EnableCompression { get; set; } = true;

    /// <summary>
    /// Connection timeout in milliseconds
    /// </summary>
    [Range(1000, 30000)]
    public int ConnectTimeoutMs { get; set; } = 5000;

    /// <summary>
    /// Sync timeout in milliseconds
    /// </summary>
    [Range(1000, 30000)]
    public int SyncTimeoutMs { get; set; } = 5000;
}
