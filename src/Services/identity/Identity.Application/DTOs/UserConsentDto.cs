namespace Identity.Application.DTOs;

public record UserConsentDto(
    Guid Id,
    Guid UserId,
    string ConsentType,
    bool IsGranted,
    DateTime GrantedAt,
    DateTime? RevokedAt,
    string? RevokedBy,
    string? RevocationReason,
    string IpAddress,
    string UserAgent,
    string TenantId,
    DateTime ExpiresAt,
    bool IsActive);

public record CreateUserConsentDto(
    Guid UserId,
    string ConsentType,
    string IpAddress,
    string UserAgent,
    string TenantId,
    DateTime? ExpiresAt = null);

public record RevokeUserConsentDto(
    Guid ConsentId,
    string RevokedBy,
    string? Reason = null);

public record ConsentStatusDto(
    string ConsentType,
    bool IsGranted,
    DateTime? GrantedAt,
    DateTime? ExpiresAt,
    bool IsActive);
