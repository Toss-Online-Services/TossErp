using Microsoft.AspNetCore.Http;
using TossErp.Procurement.Application.Common.Interfaces;

namespace TossErp.Procurement.API.Services;

/// <summary>
/// Implementation of ICurrentUserService for the API
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

    public string? UserName => _httpContextAccessor.HttpContext?.User?.FindFirst("name")?.Value;

    public string? UserEmail => _httpContextAccessor.HttpContext?.User?.FindFirst("email")?.Value;

    public string? TenantId => _httpContextAccessor.HttpContext?.User?.FindFirst("tenant_id")?.Value ?? "default-tenant";

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
