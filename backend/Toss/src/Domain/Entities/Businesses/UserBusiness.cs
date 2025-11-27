using Toss.Domain.Common;
using Toss.Infrastructure.Identity;

namespace Toss.Domain.Entities.Businesses;

public class UserBusiness : BaseAuditableEntity
{
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser User { get; set; } = null!;

    public int BusinessId { get; set; }

    public Business Business { get; set; } = null!;

    public string Role { get; set; } = string.Empty;

    public bool IsDefault { get; set; }
}

