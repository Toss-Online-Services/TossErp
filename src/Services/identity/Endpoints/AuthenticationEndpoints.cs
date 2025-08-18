using Microsoft.AspNetCore.Mvc;
using TossErp.Identity.Services;
using System.ComponentModel.DataAnnotations;

namespace TossErp.Identity.Endpoints;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    {
        var auth = app.MapGroup("/api/auth").WithTags("Authentication");

        auth.MapPost("/login", LoginAsync)
            .WithName("Login")
            .WithSummary("Authenticate user and return JWT tokens")
            .WithOpenApi();

        auth.MapPost("/refresh", RefreshTokenAsync)
            .WithName("RefreshToken")
            .WithSummary("Refresh access token using refresh token")
            .WithOpenApi();

        auth.MapPost("/logout", LogoutAsync)
            .WithName("Logout")
            .WithSummary("Revoke refresh token")
            .WithOpenApi();

        auth.MapGet("/validate", ValidateTokenAsync)
            .WithName("ValidateToken")
            .WithSummary("Validate JWT token")
            .WithOpenApi();
    }

    private static async Task<IResult> LoginAsync(
        [FromBody] LoginRequest request,
        [FromServices] IJwtTokenService tokenService,
        [FromServices] ILogger<Program> logger,
        CancellationToken cancellationToken)
    {
        try
        {
            // In a real implementation, this would validate credentials against a database
            if (request.Email == "admin@toss-erp.local" && request.Password == "admin123")
            {
                var user = new User(
                    Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    "dev-tenant",
                    request.Email,
                    "System",
                    "Administrator",
                    new[] { "admin" }
                );

                var tokens = await tokenService.GenerateTokensAsync(user, cancellationToken);

                logger.LogInformation("User {Email} logged in successfully", request.Email);

                return Results.Ok(new LoginResponse(
                    tokens.AccessToken,
                    tokens.RefreshToken,
                    tokens.ExpiresAt,
                    user.Id,
                    user.TenantId,
                    user.Email,
                    $"{user.FirstName} {user.LastName}",
                    user.Roles.ToArray()
                ));
            }

            logger.LogWarning("Failed login attempt for {Email}", request.Email);
            return Results.Unauthorized();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during login for {Email}", request.Email);
            return Results.Problem("An error occurred during authentication");
        }
    }

    private static async Task<IResult> RefreshTokenAsync(
        [FromBody] RefreshTokenRequest request,
        [FromServices] IJwtTokenService tokenService,
        [FromServices] ILogger<Program> logger,
        CancellationToken cancellationToken)
    {
        try
        {
            var tokens = await tokenService.RefreshTokenAsync(request.RefreshToken, cancellationToken);

            logger.LogInformation("Token refreshed successfully");

            return Results.Ok(new RefreshTokenResponse(
                tokens.AccessToken,
                tokens.RefreshToken,
                tokens.ExpiresAt
            ));
        }
        catch (UnauthorizedAccessException)
        {
            logger.LogWarning("Invalid refresh token provided");
            return Results.Unauthorized();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during token refresh");
            return Results.Problem("An error occurred during token refresh");
        }
    }

    private static async Task<IResult> LogoutAsync(
        [FromBody] LogoutRequest request,
        [FromServices] IJwtTokenService tokenService,
        [FromServices] ILogger<Program> logger,
        CancellationToken cancellationToken)
    {
        try
        {
            await tokenService.RevokeTokenAsync(request.RefreshToken, cancellationToken);

            logger.LogInformation("User logged out successfully");

            return Results.Ok(new { message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during logout");
            return Results.Problem("An error occurred during logout");
        }
    }

    private static async Task<IResult> ValidateTokenAsync(
        [FromHeader(Name = "Authorization")] string? authorization,
        [FromServices] IJwtTokenService tokenService,
        [FromServices] ILogger<Program> logger,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
            {
                return Results.BadRequest("Invalid authorization header");
            }

            var token = authorization["Bearer ".Length..];
            var isValid = await tokenService.ValidateTokenAsync(token, cancellationToken);

            if (isValid)
            {
                return Results.Ok(new { valid = true, message = "Token is valid" });
            }

            return Results.Unauthorized();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during token validation");
            return Results.Problem("An error occurred during token validation");
        }
    }
}

// Request/Response DTOs
public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password
);

public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    Guid UserId,
    string TenantId,
    string Email,
    string Name,
    string[] Roles
);

public record RefreshTokenRequest(
    [Required] string RefreshToken
);

public record RefreshTokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt
);

public record LogoutRequest(
    [Required] string RefreshToken
);
