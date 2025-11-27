using Toss.Application.Common.Interfaces.Tenancy;

namespace Toss.Infrastructure.Services.Tenancy;

public sealed class NullBusinessContext : IBusinessContext
{
    public int? CurrentBusinessId => null;

    public string? CurrentBusinessCode => null;

    public string? CurrentBusinessName => null;

    public string? CurrentBusinessRole => null;

    public bool HasBusiness => false;

    public void Clear()
    {
    }

    public void SetBusiness(int businessId, string code, string name, string? role = null)
    {
    }
}

