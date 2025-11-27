namespace Toss.Application.Common.Interfaces.Authentication;

public interface ITwoFactorSessionStore
{
    string CreateSession(string userId, string provider, TimeSpan? lifetime = null);

    bool TryGetSession(string sessionId, out TwoFactorSession session);

    void RemoveSession(string sessionId);
}

public record TwoFactorSession(string UserId, string Provider, DateTimeOffset ExpiresAt);

