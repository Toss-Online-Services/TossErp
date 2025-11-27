namespace Toss.Domain.Entities.Businesses;

public class UserBusiness : BaseAuditableEntity
{
    public string UserId { get; set; } = string.Empty;

    public int BusinessId { get; set; }

    public Business Business { get; set; } = null!;

    public string Role { get; set; } = string.Empty;

    public bool IsDefault { get; set; }
}

