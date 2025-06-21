using TossErp.WebApp.DTOs;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace TossErp.WebApp.Services
{
    public interface IAuthService
    {
        Task<bool> IsAuthenticatedAsync();
        Task<string?> GetTokenAsync();
        Task<UserDto?> GetCurrentUserAsync();
        Task SetTokenAsync(string token);
        Task SetUserAsync(UserDto user);
        Task LogoutAsync();
        Task<bool> LoginAsync(string username, string password);
        Task<bool> RegisterAsync(string username, string email, string password, string fullName);
    }

    public class AuthService : IAuthService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorageService;

        public AuthService(IJSRuntime jsRuntime, HttpClient httpClient, ILocalStorageService localStorageService)
        {
            _jsRuntime = jsRuntime;
            _httpClient = httpClient;
            _localStorageService = localStorageService;
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _localStorageService.GetItemAsync<string>("authToken");
        }

        public async Task<UserDto?> GetCurrentUserAsync()
        {
            try
            {
                return await _localStorageService.GetItemAsync<UserDto>("user");
            }
            catch
            {
                return null;
            }
        }

        public async Task SetTokenAsync(string token)
        {
            await _localStorageService.SetItemAsync("authToken", token);
        }

        public async Task SetUserAsync(UserDto user)
        {
            await _localStorageService.SetItemAsync("user", user);
        }

        public async Task LogoutAsync()
        {
            await _localStorageService.RemoveItemAsync("authToken");
            await _localStorageService.RemoveItemAsync("user");
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                // Call the IdentityServer token endpoint
                var tokenRequest = new
                {
                    grant_type = "password",
                    username = username,
                    password = password,
                    client_id = "toss_erp_client", // This should match your IdentityServer client configuration
                    scope = "openid profile email toss_erp_api"
                };

                var response = await _httpClient.PostAsJsonAsync("https://localhost:5006/connect/token", tokenRequest);
                
                if (response.IsSuccessStatusCode)
                {
                    var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                    if (tokenResponse?.access_token != null)
                    {
                        await SetTokenAsync(tokenResponse.access_token);
                        
                        // Get user info
                        var userInfo = await GetUserInfoAsync(tokenResponse.access_token);
                        if (userInfo != null)
                        {
                            await SetUserAsync(userInfo);
                            return true;
                        }
                    }
                }
                
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RegisterAsync(string username, string email, string password, string fullName)
        {
            try
            {
                var registerRequest = new
                {
                    UserName = username,
                    Email = email,
                    Password = password,
                    FullName = fullName
                };

                var response = await _httpClient.PostAsJsonAsync("https://localhost:5006/api/auth/register", registerRequest);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private async Task<UserDto?> GetUserInfoAsync(string token)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:5006/connect/userinfo");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var userInfo = await response.Content.ReadFromJsonAsync<UserInfoResponse>();
                    if (userInfo != null)
                    {
                        return new UserDto
                        {
                            Id = Guid.Parse(userInfo.sub ?? throw new InvalidOperationException("User ID is required")),
                            UserName = userInfo.preferred_username ?? userInfo.name ?? "Unknown",
                            Email = userInfo.email ?? "",
                            FullName = userInfo.name ?? "Unknown User",
                            Role = userInfo.role ?? "User"
                        };
                    }
                }
                
                return null;
            }
            catch
            {
                return null;
            }
        }
    }

    public class TokenResponse
    {
        public string? access_token { get; set; }
        public string? token_type { get; set; }
        public int expires_in { get; set; }
        public string? refresh_token { get; set; }
    }

    public class UserInfoResponse
    {
        public string? sub { get; set; }
        public string? name { get; set; }
        public string? given_name { get; set; }
        public string? family_name { get; set; }
        public string? preferred_username { get; set; }
        public string? email { get; set; }
        public string? role { get; set; }
    }
} 
