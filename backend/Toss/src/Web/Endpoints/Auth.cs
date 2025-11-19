using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Toss.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;

namespace Toss.Web.Endpoints;

public class Auth : EndpointGroupBase
{
 public override void Map(RouteGroupBuilder groupBuilder)
 {
 // POST /api/auth/login (alias for /api/Users/login)
 groupBuilder.MapPost("login", (Delegate)Login)
 .AllowAnonymous();

 // POST /api/auth/refresh (alias for /api/Users/refresh)
 groupBuilder.MapPost("refresh", (Delegate)Refresh)
 .AllowAnonymous();

 // POST /api/auth/logout
 groupBuilder.MapPost("logout", (Delegate)Logout)
 .RequireAuthorization();

 // GET /api/auth/verify
 groupBuilder.MapGet("verify", (Delegate)Verify)
 .RequireAuthorization();
 }

 // Proxy login to Identity endpoint and transform response
 private static async Task<IResult> Login(HttpContext httpContext)
 {
 var request = httpContext.Request;
 var client = httpContext.RequestServices.GetRequiredService<IHttpClientFactory>().CreateClient();                                                              
 var configuration = httpContext.RequestServices.GetRequiredService<IConfiguration>();
 var backendUrl = "/api/Users/login";
 request.EnableBuffering();
 var body = await new System.IO.StreamReader(request.Body).ReadToEndAsync();    
 request.Body.Position =0;
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
 var user = DecodeUserFromJwt(token, configuration);
 authResponse["user"] = user;
 }
 return Results.Json(authResponse);
 }
 return Results.Json(System.Text.Json.JsonDocument.Parse(respContent), statusCode: (int)response.StatusCode);                                                   
 }

 // Proxy refresh to Identity endpoint and transform response
 private static async Task<IResult> Refresh(HttpContext httpContext)
 {
 var request = httpContext.Request;
 var client = httpContext.RequestServices.GetRequiredService<IHttpClientFactory>().CreateClient();
 var backendUrl = "/api/Users/refresh";
 request.EnableBuffering();
 var body = await new System.IO.StreamReader(request.Body).ReadToEndAsync();
 request.Body.Position =0;
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

 // Decode JWT to AuthUser using System.IdentityModel.Tokens.Jwt
 private static object DecodeUserFromJwt(string token, IConfiguration configuration)
 {
     try
     {
         var handler = new JwtSecurityTokenHandler();
         var jwtSettings = configuration.GetSection("JwtSettings");
         var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
         
         var validationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
         {
             ValidateIssuer = true,
             ValidateAudience = true,
             ValidateLifetime = false, // Don't validate expiry for decoding
             ValidateIssuerSigningKey = true,
             ValidIssuer = jwtSettings["Issuer"] ?? "TossErp",
             ValidAudience = jwtSettings["Audience"] ?? "TossErp",
             IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                 System.Text.Encoding.UTF8.GetBytes(secretKey))
         };
         
         var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);
         var jwtToken = validatedToken as JwtSecurityToken;
         
         if (jwtToken == null)
         {
             throw new InvalidOperationException("Invalid JWT token");
         }
         
         var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
         var userName = principal.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
         var email = principal.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
         var phone = principal.FindFirst("phone")?.Value;
         var firstName = principal.FindFirst("firstName")?.Value;
         var lastName = principal.FindFirst("lastName")?.Value;
         var roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
         
         var user = new Dictionary<string, object?>();
         user["id"] = userId;
         user["name"] = !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName) 
             ? $"{firstName} {lastName}" 
             : userName;
         user["email"] = email;
         user["phone"] = phone;
         user["firstName"] = firstName;
         user["lastName"] = lastName;
         user["roles"] = roles;
         user["permissions"] = Array.Empty<string>();
         user["avatar"] = null;
         user["lastLogin"] = DateTime.UtcNow;
         
         return user;
     }
     catch (Exception ex)
     {
         // Log error and return minimal user info
         Console.WriteLine($"Error decoding JWT: {ex.Message}");
         var user = new Dictionary<string, object?>();
         user["id"] = "unknown";
         user["name"] = "Unknown User";
         user["email"] = "";
         user["roles"] = Array.Empty<string>();
         user["permissions"] = Array.Empty<string>();
         user["avatar"] = null;
         user["lastLogin"] = DateTime.UtcNow;
         return user;
     }
 }

 // Simple in-memory store for invalidated refresh tokens (for demo; use persistent store in production)
 private static readonly HashSet<string> InvalidatedRefreshTokens = new();

 private static async Task<IResult> Logout(HttpContext httpContext)
 {
 // Read refresh token from body
 httpContext.Request.EnableBuffering();
 var body = await new System.IO.StreamReader(httpContext.Request.Body).ReadToEndAsync();
 httpContext.Request.Body.Position =0;
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
