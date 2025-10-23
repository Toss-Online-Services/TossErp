using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Toss.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Security.Claims;
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
    private static async Task<IResult> Login(HttpContext httpContext)
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
        return Results.Content(respContent, "application/json", (int)response.StatusCode);
    }

    // Proxy refresh to Identity endpoint and transform response
    private static async Task<IResult> Refresh(HttpContext httpContext)
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
        return Results.Content(respContent, "application/json", (int)response.StatusCode);
    }

    // Decode JWT to AuthUser
    private static object DecodeUserFromJwt(string token)
    {
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        var user = new Dictionary<string, object?>();
        user["id"] = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        user["name"] = jwt.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
        user["email"] = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
        user["roles"] = jwt.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToArray();
        user["permissions"] = jwt.Claims.Where(c => c.Type == "permission").Select(c => c.Value).ToArray();
        user["avatar"] = jwt.Claims.FirstOrDefault(c => c.Type == "avatar")?.Value;
        user["lastLogin"] = DateTime.UtcNow;
        return user;
    }

    // Proxy refresh to Identity endpoint
    private static async Task<IResult> Refresh(HttpContext httpContext)
    {
        var request = httpContext.Request;
        var client = httpContext.RequestServices.GetRequiredService<IHttpClientFactory>().CreateClient();
        var backendUrl = "/api/Users/refresh";
        request.EnableBuffering();
        var body = await new System.IO.StreamReader(request.Body).ReadToEndAsync();
        request.Body.Position = 0;
        var response = await client.PostAsync(backendUrl, new StringContent(body, System.Text.Encoding.UTF8, "application/json"));
        var respContent = await response.Content.ReadAsStringAsync();
        return Results.Content(respContent, "application/json", (int)response.StatusCode);
    }

    // Simple in-memory store for invalidated refresh tokens (for demo; use persistent store in production)
    private static readonly HashSet<string> InvalidatedRefreshTokens = new();

    private static async Task<IResult> Logout(HttpContext httpContext)
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
    private static IResult Verify(ClaimsPrincipal user)
    {
        if (user.Identity != null && user.Identity.IsAuthenticated)
        {
            return Results.Ok(new { message = "Token is valid" });
        }
        return Results.Unauthorized();
    }
}