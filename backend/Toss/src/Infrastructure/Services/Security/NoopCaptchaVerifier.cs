using Toss.Application.Common.Interfaces.Security;

namespace Toss.Infrastructure.Services.Security;

/// <summary>
/// No-op captcha verifier for development/testing
/// Always returns true
/// </summary>
public class NoopCaptchaVerifier : ICaptchaVerifier
{
    public Task<bool> VerifyAsync(string token, string? remoteIp = null, CancellationToken cancellationToken = default)
    {
        // In development, always pass captcha verification
        return Task.FromResult(true);
    }
}

