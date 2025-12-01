using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Toss.Application.Common.Interfaces.Authentication;
using Toss.Application.Common.Interfaces.Analytics;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Enums;
using Toss.Infrastructure.Identity;

namespace Toss.Web.Endpoints;

public class Auth : EndpointGroupBase
{
 public override void Map(RouteGroupBuilder groupBuilder)
 {
        groupBuilder.MapPost("login", Login)
            .AllowAnonymous()
            .RequireRateLimiting("AuthLimiter");

        groupBuilder.MapPost("otp/verify", VerifyOtp)
            .AllowAnonymous()
            .RequireRateLimiting("AuthLimiter");

        groupBuilder.MapPost("refresh", Refresh)
            .AllowAnonymous()
            .RequireRateLimiting("AuthLimiter");

        groupBuilder.MapPost("logout", Logout)
 .RequireAuthorization();

 groupBuilder.MapGet("verify", (Delegate)Verify)
 .RequireAuthorization();
 }

    private static async Task<IResult> Login(
        LoginRequest request,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        ITwoFactorSessionStore sessionStore,
        IOtpSender otpSender,
        IBusinessEventService? eventService,
        IBusinessContext? businessContext,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return Results.BadRequest(new { message = "Password is required." });
        }

        var user = await FindUserAsync(request, userManager, cancellationToken);
        if (user is null)
        {
            await Task.Delay(Random.Shared.Next(100, 250), cancellationToken);
            return Results.Unauthorized();
        }

        var signInResult = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

        if (signInResult.IsLockedOut)
        {
            return Results.Problem("Account is locked. Please try again later.", statusCode: StatusCodes.Status423Locked);
        }

        if (!signInResult.Succeeded && !signInResult.RequiresTwoFactor)
        {
            return Results.Unauthorized();
        }

        var provider = DetermineProvider(request, user);

        if (signInResult.RequiresTwoFactor || request.ForceOtp || !string.IsNullOrWhiteSpace(request.TwoFactorCode))
        {
            if (!string.IsNullOrWhiteSpace(request.TwoFactorCode))
            {
                var isValid = await userManager.VerifyTwoFactorTokenAsync(user, provider, request.TwoFactorCode);
                if (!isValid)
                {
                    return Results.Unauthorized();
                }

                var tokens = await tokenService.CreateAsync(user.Id, cancellationToken);
                return Results.Json(tokens);
            }

            if (provider == TokenOptions.DefaultPhoneProvider && !string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                var code = await userManager.GenerateTwoFactorTokenAsync(user, provider);
                await otpSender.SendAsync(user.PhoneNumber!, code, cancellationToken);
            }

            var sessionId = sessionStore.CreateSession(user.Id, provider);
            var response = new OtpChallengeResponse(
                sessionId,
                provider,
                provider == TokenOptions.DefaultPhoneProvider ? "OTP sent via SMS." : "Enter your authenticator app code.");

            return Results.Json(response, statusCode: StatusCodes.Status202Accepted);
        }

        var authTokens = await tokenService.CreateAsync(user.Id, cancellationToken);
        
        // Track login event for analytics
        if (eventService != null && businessContext != null && businessContext.HasBusiness)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await eventService.EmitEventAsync(
                        BusinessEventType.Login,
                        module: "Auth",
                        userId: user.Id);
                }
                catch
                {
                    // Silently fail - don't break login
                }
            });
        }
        
        return Results.Json(authTokens);
    }

    private static async Task<IResult> VerifyOtp(
        OtpVerifyRequest request,
        ITwoFactorSessionStore sessionStore,
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        CancellationToken cancellationToken)
    {
        if (!sessionStore.TryGetSession(request.SessionId, out var session) || session.ExpiresAt < DateTimeOffset.UtcNow)
        {
            return Results.Unauthorized();
        }

        var user = await userManager.FindByIdAsync(session.UserId);
        if (user is null)
        {
            sessionStore.RemoveSession(request.SessionId);
            return Results.Unauthorized();
        }

        var isValid = await userManager.VerifyTwoFactorTokenAsync(user, session.Provider, request.Code);
        if (!isValid)
        {
            return Results.Unauthorized();
        }

        sessionStore.RemoveSession(request.SessionId);

        var tokens = await tokenService.CreateAsync(user.Id, cancellationToken);
        return Results.Json(tokens);
    }

    private static async Task<IResult> Refresh(
        RefreshRequest request,
        ITokenService tokenService,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return Results.BadRequest(new { message = "Refresh token is required." });
        }

        var tokens = await tokenService.RefreshAsync(request.RefreshToken, cancellationToken);
        return tokens is null ? Results.Unauthorized() : Results.Json(tokens);
    }

    private static async Task<IResult> Logout(
        RefreshRequest request,
        ITokenService tokenService,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            await tokenService.RevokeAsync(request.RefreshToken, cancellationToken);
        }

        return Results.Ok(new { message = "Logged out" });
    }

    private static IResult Verify(ClaimsPrincipal user)
    {
        return user.Identity is { IsAuthenticated: true }
            ? Results.Ok(new { message = "Token is valid" })
            : Results.Unauthorized();
    }

    private static async Task<ApplicationUser?> FindUserAsync(
        LoginRequest request,
        UserManager<ApplicationUser> userManager,
        CancellationToken cancellationToken)
    {
        var identifier = request.Identifier ?? request.Email ?? request.PhoneNumber;
        if (string.IsNullOrWhiteSpace(identifier))
        {
 return null;
 }

        if (identifier.Contains('@', StringComparison.Ordinal))
        {
            return await userManager.FindByEmailAsync(identifier);
        }

        var normalized = NormalizePhone(identifier);
        return await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == normalized, cancellationToken);
    }

    private static string DetermineProvider(LoginRequest request, ApplicationUser user)
    {
        if (!string.IsNullOrWhiteSpace(request.TwoFactorProvider))
        {
            return request.TwoFactorProvider;
        }

        return !string.IsNullOrWhiteSpace(user.PhoneNumber) && user.PhoneNumberConfirmed
            ? TokenOptions.DefaultPhoneProvider
            : TokenOptions.DefaultAuthenticatorProvider;
    }

    private static string NormalizePhone(string phone)
    {
        return phone.Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .Replace("(", string.Empty, StringComparison.Ordinal)
            .Replace(")", string.Empty, StringComparison.Ordinal);
    }

    private sealed record LoginRequest
    {
        public string? Identifier { get; init; }

        public string? Email { get; init; }

        public string? PhoneNumber { get; init; }

        [Required]
        public string Password { get; init; } = string.Empty;

        public string? TwoFactorCode { get; init; }

        public string? TwoFactorProvider { get; init; }

        public bool ForceOtp { get; init; }
    }

    private sealed record OtpVerifyRequest(
        [property: Required] string SessionId,
        [property: Required] string Code);

    private sealed record OtpChallengeResponse(string SessionId, string Provider, string Message);

    private sealed record RefreshRequest([property: Required] string RefreshToken);
}
