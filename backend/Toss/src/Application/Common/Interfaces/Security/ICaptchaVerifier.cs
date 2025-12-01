namespace Toss.Application.Common.Interfaces.Security;

/// <summary>
/// Service for verifying captcha tokens (hCaptcha, Turnstile, etc.)
/// </summary>
public interface ICaptchaVerifier
{
    /// <summary>
    /// Verifies a captcha token
    /// </summary>
    /// <param name="token">The captcha token to verify</param>
    /// <param name="remoteIp">The remote IP address of the client</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the captcha is valid, false otherwise</returns>
    Task<bool> VerifyAsync(string token, string? remoteIp = null, CancellationToken cancellationToken = default);
}

