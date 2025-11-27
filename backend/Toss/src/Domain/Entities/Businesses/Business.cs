using Toss.Domain.Common;
using Toss.Domain.Entities.Stores;

namespace Toss.Domain.Entities.Businesses;

public class Business : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Currency { get; set; } = "ZAR";

    public string Timezone { get; set; } = "Africa/Johannesburg";

    public string? LogoUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<UserBusiness> Members { get; set; } = new List<UserBusiness>();

    public ICollection<Store> Stores { get; set; } = new List<Store>();
}

