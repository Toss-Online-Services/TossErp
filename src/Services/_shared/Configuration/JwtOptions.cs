using System.ComponentModel.DataAnnotations;

namespace TossErp.Configuration;

/// <summary>
/// JWT authentication configuration options
/// </summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    /// <summary>
    /// JWT signing key (base64 encoded)
    /// </summary>
    [Required]
    public string SigningKey { get; set; } = string.Empty;

    /// <summary>
    /// JWT issuer
    /// </summary>
    [Required]
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// JWT audience
    /// </summary>
    [Required]
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Access token expiration in minutes
    /// </summary>
    [Range(1, 1440)]
    public int AccessTokenExpirationMinutes { get; set; } = 60;

    /// <summary>
    /// Refresh token expiration in days
    /// </summary>
    [Range(1, 365)]
    public int RefreshTokenExpirationDays { get; set; } = 30;

    /// <summary>
    /// Clock skew tolerance in minutes
    /// </summary>
    [Range(0, 60)]
    public int ClockSkewMinutes { get; set; } = 5;

    /// <summary>
    /// Require HTTPS for JWT validation
    /// </summary>
    public bool RequireHttps { get; set; } = true;

    /// <summary>
    /// Validate token lifetime
    /// </summary>
    public bool ValidateLifetime { get; set; } = true;

    /// <summary>
    /// Validate token issuer
    /// </summary>
    public bool ValidateIssuer { get; set; } = true;

    /// <summary>
    /// Validate token audience
    /// </summary>
    public bool ValidateAudience { get; set; } = true;
}
