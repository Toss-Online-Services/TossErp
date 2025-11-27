namespace Toss.Application.Common.Interfaces.Tenancy;

public interface IBusinessContext
{
    int? CurrentBusinessId { get; }

    string? CurrentBusinessCode { get; }

    string? CurrentBusinessName { get; }

    string? CurrentBusinessRole { get; }

    bool HasBusiness { get; }

    void SetBusiness(int businessId, string code, string name, string? role = null);

    void Clear();
}

