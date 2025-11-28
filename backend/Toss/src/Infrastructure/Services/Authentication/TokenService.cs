using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Authentication;
using Toss.Application.Common.Models.Auth;
using Toss.Infrastructure.Data;
using Toss.Infrastructure.Identity;

namespace Toss.Infrastructure.Services.Authentication;

public class TokenService : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IIdentityService _identityService;
    private readonly ApplicationDbContext _context;
    private readonly TimeSpan _accessTokenLifetime;
    private readonly TimeSpan _refreshTokenLifetime = TimeSpan.FromDays(14);

    public TokenService(
        UserManager<ApplicationUser> userManager,
        IIdentityService identityService,
        ApplicationDbContext context,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _identityService = identityService;
        _context = context;

        var jwtSettings = configuration.GetSection("JwtSettings");
        var accessTokenDays = jwtSettings.GetValue("ExpirationInDays", 7);
        _accessTokenLifetime = TimeSpan.FromDays(accessTokenDays);
    }

    public async Task<AuthTokenResult> CreateAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
                   ?? throw new InvalidOperationException("User not found.");

        var jwt = await _identityService.GenerateTokenAsync(user.Id);
        var refreshToken = await CreateRefreshTokenAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var profile = await BuildUserProfileAsync(user, cancellationToken);
        return new AuthTokenResult(jwt, refreshToken.Token, (long)_accessTokenLifetime.TotalSeconds, profile);
    }

    public async Task<AuthTokenResult?> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var existing = await _context.RefreshTokens
            .Include(x => x.User)
            .SingleOrDefaultAsync(x => x.Token == refreshToken, cancellationToken);

        if (existing is null || !existing.IsActive)
        {
            return null;
        }

        existing.RevokedAt = DateTimeOffset.UtcNow;

        var replacement = await CreateRefreshTokenAsync(existing.User, cancellationToken);
        existing.ReplacedByToken = replacement.Token;

        var jwt = await _identityService.GenerateTokenAsync(existing.UserId);
        var profile = await BuildUserProfileAsync(existing.User, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return new AuthTokenResult(jwt, replacement.Token, (long)_accessTokenLifetime.TotalSeconds, profile);
    }

    public async Task RevokeAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var existing = await _context.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken, cancellationToken);
        if (existing is null || existing.RevokedAt is not null)
        {
            return;
        }

        existing.RevokedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }

    private Task<RefreshToken> CreateRefreshTokenAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = token,
            UserId = user.Id,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = DateTimeOffset.UtcNow.Add(_refreshTokenLifetime)
        };

        _context.RefreshTokens.Add(refreshToken);
        return Task.FromResult(refreshToken);
    }

    private async Task<IReadOnlyDictionary<string, object?>> BuildUserProfileAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var fullName = $"{user.FirstName} {user.LastName}".Trim();

        var profile = new Dictionary<string, object?>
        {
            ["id"] = user.Id,
            ["name"] = string.IsNullOrWhiteSpace(fullName) ? user.UserName : fullName,
            ["email"] = user.Email,
            ["phone"] = user.PhoneNumber,
            ["firstName"] = user.FirstName,
            ["lastName"] = user.LastName,
            ["roles"] = roles
        };

        return profile;
    }
}

