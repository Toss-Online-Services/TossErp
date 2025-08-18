using System.ComponentModel.DataAnnotations;

namespace TossErp.Configuration;

/// <summary>
/// Database configuration options
/// </summary>
public class DatabaseOptions
{
    public const string SectionName = "Database";

    /// <summary>
    /// Primary database connection string
    /// </summary>
    [Required]
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Read-only replica connection string (optional)
    /// </summary>
    public string? ReadOnlyConnectionString { get; set; }

    /// <summary>
    /// Command timeout in seconds
    /// </summary>
    [Range(1, 300)]
    public int CommandTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Maximum retry attempts for transient failures
    /// </summary>
    [Range(0, 10)]
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Enable sensitive data logging (only for development)
    /// </summary>
    public bool EnableSensitiveDataLogging { get; set; } = false;

    /// <summary>
    /// Enable detailed errors (only for development)
    /// </summary>
    public bool EnableDetailedErrors { get; set; } = false;
}
