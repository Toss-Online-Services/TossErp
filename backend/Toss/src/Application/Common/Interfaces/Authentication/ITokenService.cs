using Toss.Application.Common.Models.Auth;

namespace Toss.Application.Common.Interfaces.Authentication;

public interface ITokenService
{
    Task<AuthTokenResult> CreateAsync(string userId, CancellationToken cancellationToken = default);

    Task<AuthTokenResult?> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);

    Task RevokeAsync(string refreshToken, CancellationToken cancellationToken = default);
}

