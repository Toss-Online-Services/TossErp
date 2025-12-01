using System.Text.Json;
using Toss.Application.Common.Interfaces.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Toss.Infrastructure.Services.Security;

/// <summary>
/// Cloudflare Turnstile captcha verifier
/// </summary>
public class TurnstileCaptchaVerifier : ICaptchaVerifier
{
    private readonly HttpClient _httpClient;
    private readonly string _secretKey;
    private readonly ILogger<TurnstileCaptchaVerifier> _logger;

    public TurnstileCaptchaVerifier(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<TurnstileCaptchaVerifier> logger)
    {
        _httpClient = httpClient;
        _secretKey = configuration["Captcha:Turnstile:SecretKey"] ?? string.Empty;
        _logger = logger;
    }

    public async Task<bool> VerifyAsync(string token, string? remoteIp = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(_secretKey))
        {
            _logger.LogWarning("Turnstile secret key not configured. Skipping captcha verification.");
            return true; // Fail open in development if not configured
        }

        try
        {
            var formData = new List<KeyValuePair<string, string>>
            {
                new("secret", _secretKey),
                new("response", token)
            };

            if (!string.IsNullOrWhiteSpace(remoteIp))
            {
                formData.Add(new KeyValuePair<string, string>("remoteip", remoteIp));
            }

            var content = new FormUrlEncodedContent(formData);
            var response = await _httpClient.PostAsync("https://challenges.cloudflare.com/turnstile/v0/siteverify", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Turnstile verification request failed with status {StatusCode}", response.StatusCode);
                return false;
            }

            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<TurnstileResponse>(responseBody);

            if (result == null)
            {
                _logger.LogWarning("Failed to deserialize Turnstile response");
                return false;
            }

            if (!result.Success)
            {
                _logger.LogWarning("Turnstile verification failed: {Errors}", string.Join(", ", result.ErrorCodes ?? Array.Empty<string>()));
            }

            return result.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying Turnstile captcha");
            return false;
        }
    }

    private record TurnstileResponse
    {
        public bool Success { get; init; }
        public string[]? ErrorCodes { get; init; }
    }
}

