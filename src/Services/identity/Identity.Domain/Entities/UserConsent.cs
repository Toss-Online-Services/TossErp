namespace Identity.Domain.Entities;

public class UserConsent : Entity
{
    public Guid UserId { get; private set; }
    public string ConsentType { get; private set; }
    public bool IsGranted { get; private set; }
    public DateTime GrantedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? RevokedBy { get; private set; }
    public string? RevocationReason { get; private set; }
    public string IpAddress { get; private set; }
    public string UserAgent { get; private set; }
    public string TenantId { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsActive => IsGranted && (ExpiresAt == DateTime.MinValue || ExpiresAt > DateTime.UtcNow);

    public UserConsent(
        Guid userId,
        string consentType,
        string ipAddress,
        string userAgent,
        string tenantId,
        DateTime? expiresAt = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        ConsentType = consentType ?? throw new ArgumentNullException(nameof(consentType));
        IsGranted = true;
        GrantedAt = DateTime.UtcNow;
        IpAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
        UserAgent = userAgent ?? throw new ArgumentNullException(nameof(userAgent));
        TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        ExpiresAt = expiresAt ?? DateTime.MinValue; // No expiration by default
    }

    private UserConsent() { }

    public void Revoke(string revokedBy, string? reason = null)
    {
        IsGranted = false;
        RevokedAt = DateTime.UtcNow;
        RevokedBy = revokedBy ?? throw new ArgumentNullException(nameof(revokedBy));
        RevocationReason = reason;
    }

    public void Renew()
    {
        if (!IsGranted)
        {
            IsGranted = true;
            GrantedAt = DateTime.UtcNow;
            RevokedAt = null;
            RevokedBy = null;
            RevocationReason = null;
        }
    }
}
