using Toss.Domain.Enums;

namespace Toss.Application.Common.Interfaces.Collaborations;

/// <summary>
/// Service for validating collaboration links (tokens)
/// </summary>
public interface ICollabLinkService
{
    /// <summary>
    /// Validates a collaboration link token
    /// </summary>
    /// <param name="linkCode">The link code/token to validate</param>
    /// <param name="purpose">The expected purpose (Feedback or Offer)</param>
    /// <param name="linkedType">The expected linked entity type</param>
    /// <param name="linkedId">The expected linked entity ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the link is valid, active, and matches the criteria; false otherwise</returns>
    Task<bool> ValidateLinkAsync(
        string linkCode,
        CollabLinkPurpose purpose,
        string linkedType,
        int linkedId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets collaboration link details by code
    /// </summary>
    Task<CollabLinkInfo?> GetLinkInfoAsync(string linkCode, CancellationToken cancellationToken = default);
}

/// <summary>
/// Information about a collaboration link
/// </summary>
public record CollabLinkInfo
{
    public int BusinessId { get; init; }
    public string LinkedType { get; init; } = string.Empty;
    public int LinkedId { get; init; }
    public CollabLinkPurpose Purpose { get; init; }
    public bool IsActive { get; init; }
    public DateTimeOffset? ExpiresAt { get; init; }
}

