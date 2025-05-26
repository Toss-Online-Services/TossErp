namespace Ordering.API.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly IHttpContextAccessor _context;

    public IdentityService(IHttpContextAccessor context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public string GetUserIdentity() => _context.HttpContext?.User?.FindFirst("sub")?.Value;

    public string GetUserName()
        => _context.HttpContext?.User.Identity?.Name;
}
