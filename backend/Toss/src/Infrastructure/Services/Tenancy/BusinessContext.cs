using Toss.Application.Common.Interfaces.Tenancy;

namespace Toss.Infrastructure.Services.Tenancy;

public class BusinessContext : IBusinessContext
{
    public int? CurrentBusinessId { get; private set; }

    public string? CurrentBusinessCode { get; private set; }

    public string? CurrentBusinessName { get; private set; }

    public string? CurrentBusinessRole { get; private set; }

    public bool HasBusiness => CurrentBusinessId.HasValue;

    public void SetBusiness(int businessId, string code, string name, string? role = null)
    {
        CurrentBusinessId = businessId;
        CurrentBusinessCode = code;
        CurrentBusinessName = name;
        CurrentBusinessRole = role;
    }

    public void Clear()
    {
        CurrentBusinessId = null;
        CurrentBusinessCode = null;
        CurrentBusinessName = null;
        CurrentBusinessRole = null;
    }
}

