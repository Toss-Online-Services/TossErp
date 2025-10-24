using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Toss.Application.Buying.Commands.CreatePurchaseOrder;
using Toss.Infrastructure.Identity;
// using Microsoft.AspNetCore.Authentication.JwtBearer; // TODO: Add JWT authentication

namespace Toss.Web.Endpoints;

public class Auth : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        // POST /api/auth/login (alias for /api/Users/login)
        groupBuilder.MapPost("login", Login)
            .AllowAnonymous();

        // POST /api/auth/refresh (alias for /api/Users/refresh)
        groupBuilder.MapPost("refresh", Refresh)
            .AllowAnonymous();

        // POST /api/auth/logout
        groupBuilder.MapPost("logout", Logout)
            .RequireAuthorization();

        // GET /api/auth/verify
        groupBuilder.MapGet("verify", Verify)
            .RequireAuthorization();
    }

    // Proxy login to Identity endpoint and transform response
    private static async Task<IResult> Login(ISender sender,HttpContext httpContext)
    {
        var request = httpContext.Request;
        var client = httpContext.RequestServices.GetRequiredService<IHttpClientFactory>().CreateClient();
        var backendUrl = "/api/Users/login";
        request.EnableBuffering();
        var body = await new System.IO.StreamReader(request.Body).ReadToEndAsync();
        request.Body.Position = 0;
        var response = await client.PostAsync(backendUrl, new StringContent(body, System.Text.Encoding.UTF8, "application/json"));
        var respContent = await response.Content.ReadAsStringAsync();

        // Transform backend response to frontend AuthResponse
        if (response.IsSuccessStatusCode)
        {
            var backendObj = System.Text.Json.JsonDocument.Parse(respContent).RootElement;
            var authResponse = new System.Dynamic.ExpandoObject() as IDictionary<string, object?>;
            authResponse["token"] = backendObj.GetProperty("accessToken").GetString();
            authResponse["refreshToken"] = backendObj.GetProperty("refreshToken").GetString();
            authResponse["expiresIn"] = backendObj.GetProperty("expiresIn").GetInt64();
            // Decode JWT to get user info
            var token = backendObj.GetProperty("accessToken").GetString();
            if (!string.IsNullOrEmpty(token))
            {
                var user = DecodeUserFromJwt(token);
                authResponse["user"] = user;
            }
            return Results.Json(authResponse);
        }
        return Results.Json(System.Text.Json.JsonDocument.Parse(respContent), statusCode: (int)response.StatusCode);
    }

    // Proxy refresh to Identity endpoint and transform response
    private static async Task<IResult> Refresh(ISender sender, HttpContext httpContext)
    {
        var request = httpContext.Request;
        var client = httpContext.RequestServices.GetRequiredService<IHttpClientFactory>().CreateClient();
        var backendUrl = "/api/Users/refresh";
        request.EnableBuffering();
        var body = await new System.IO.StreamReader(request.Body).ReadToEndAsync();
        request.Body.Position = 0;
        var response = await client.PostAsync(backendUrl, new StringContent(body, System.Text.Encoding.UTF8, "application/json"));
        var respContent = await response.Content.ReadAsStringAsync();

        // Transform backend response to frontend RefreshTokenResponse
        if (response.IsSuccessStatusCode)
        {
            var backendObj = System.Text.Json.JsonDocument.Parse(respContent).RootElement;
            var refreshResponse = new System.Dynamic.ExpandoObject() as IDictionary<string, object?>;
            refreshResponse["token"] = backendObj.GetProperty("accessToken").GetString();
            refreshResponse["expiresIn"] = backendObj.GetProperty("expiresIn").GetInt64();
            return Results.Json(refreshResponse);
        }
        return Results.Json(System.Text.Json.JsonDocument.Parse(respContent), statusCode: (int)response.StatusCode);
    }

    // Decode JWT to AuthUser (simplified - TODO: Add proper JWT decoding library)
    private static object DecodeUserFromJwt(string token)
    {
        // TODO: Install System.IdentityModel.Tokens.Jwt package and implement proper JWT decoding
        var user = new Dictionary<string, object?>();
        user["id"] = "demo-user";
        user["name"] = "Demo User";
        user["email"] = "demo@toss.co.za";
        user["roles"] = new[] { "User" };
        user["permissions"] = Array.Empty<string>();
        user["avatar"] = null;
        user["lastLogin"] = DateTime.UtcNow;
        return user;
    }

    // Simple in-memory store for invalidated refresh tokens (for demo; use persistent store in production)
    private static readonly HashSet<string> InvalidatedRefreshTokens = new();

    private static async Task<IResult> Logout(ISender sender, HttpContext httpContext)
    {
        // Read refresh token from body
        httpContext.Request.EnableBuffering();
        var body = await new System.IO.StreamReader(httpContext.Request.Body).ReadToEndAsync();
        httpContext.Request.Body.Position = 0;
        var refreshToken = ExtractRefreshTokenFromBody(body);
        if (!string.IsNullOrEmpty(refreshToken))
        {
            InvalidatedRefreshTokens.Add(refreshToken);
            return Results.Ok(new { message = "Logged out", refreshToken });
        }
        return Results.BadRequest(new { message = "No refresh token provided" });
    }

    private static string? ExtractRefreshTokenFromBody(string body)
    {
        // Expecting JSON: { "refreshToken": "..." }
        try
        {
            var json = System.Text.Json.JsonDocument.Parse(body);
            if (json.RootElement.TryGetProperty("refreshToken", out var tokenProp))
            {
                return tokenProp.GetString();
            }
        }
        catch { }
        return null;
    }

    // Verify: Validate JWT
    private static IResult Verify(ISender sender, ClaimsPrincipal user)
    {
        if (user.Identity != null && user.Identity.IsAuthenticated)
        {
            return Results.Ok(new { message = "Token is valid" });
        }
        return Results.Unauthorized();
    }
}
