using TossErp.Sales.Application.Common.Interfaces;

namespace TossErp.Sales.API.Services;

/// <summary>
/// Implementation of ICurrentUserService for API
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value ?? "system";

    public string? TenantId => _httpContextAccessor.HttpContext?.User?.FindFirst("tenant_id")?.Value ?? "default-tenant";

    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirst("email")?.Value;

    public string? Name => _httpContextAccessor.HttpContext?.User?.FindFirst("name")?.Value;

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
