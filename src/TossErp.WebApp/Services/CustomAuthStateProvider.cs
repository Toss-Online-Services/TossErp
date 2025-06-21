using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;
using TossErp.WebApp.DTOs;

namespace TossErp.WebApp.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IAuthService _authService;
        private readonly ILocalStorageService _localStorageService;
        private readonly HttpClient _httpClient;

        public CustomAuthStateProvider(
            IAuthService authService,
            ILocalStorageService localStorageService,
            HttpClient httpClient)
        {
            _authService = authService;
            _localStorageService = localStorageService;
            _httpClient = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _authService.GetTokenAsync();
                
                if (string.IsNullOrEmpty(token))
                {
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                // Validate token and get user info
                var user = await _authService.GetCurrentUserAsync();
                if (user == null)
                {
                    // Token exists but no user, clear token
                    await _authService.LogoutAsync();
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                // Create claims from user data
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName ?? user.Email ?? ""),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("UserName", user.UserName ?? ""),
                    new Claim("Email", user.Email ?? ""),
                    new Claim("FullName", user.FullName ?? ""),
                    new Claim("Role", user.Role ?? "User")
                };

                // Add role claims
                if (!string.IsNullOrEmpty(user.Role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, user.Role));
                }

                var identity = new ClaimsIdentity(claims, "jwt");
                var principal = new ClaimsPrincipal(identity);

                return new AuthenticationState(principal);
            }
            catch
            {
                // If any error occurs, return unauthenticated state
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public async Task MarkUserAsAuthenticatedAsync(string token, UserDto user)
        {
            await _authService.SetTokenAsync(token);
            await _authService.SetUserAsync(user);
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? user.Email ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim("UserId", user.Id.ToString()),
                new Claim("UserName", user.UserName ?? ""),
                new Claim("Email", user.Email ?? ""),
                new Claim("FullName", user.FullName ?? ""),
                new Claim("Role", user.Role ?? "User")
            };

            if (!string.IsNullOrEmpty(user.Role))
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role));
            }

            var identity = new ClaimsIdentity(claims, "jwt");
            var principal = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        }

        public async Task MarkUserAsLoggedOutAsync()
        {
            await _authService.LogoutAsync();
            
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        }
    }
} 
