using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using Toss.Application.Common.Interfaces.Authentication;

namespace Toss.Infrastructure.Services.Authentication;

public class TwoFactorSessionStore : ITwoFactorSessionStore
{
    private readonly IMemoryCache _cache;

    public TwoFactorSessionStore(IMemoryCache cache)
    {
        _cache = cache;
    }

    public string CreateSession(string userId, string provider, TimeSpan? lifetime = null)
    {
        var sessionId = Guid.NewGuid().ToString("N");
        var expiresAt = DateTimeOffset.UtcNow.Add(lifetime ?? TimeSpan.FromMinutes(5));
        var session = new TwoFactorSession(userId, provider, expiresAt);
        _cache.Set(sessionId, session, expiresAt);
        return sessionId;
    }

    public bool TryGetSession(string sessionId, out TwoFactorSession session)
    {
        return _cache.TryGetValue(sessionId, out session!);
    }

    public void RemoveSession(string sessionId)
    {
        _cache.Remove(sessionId);
    }
}

