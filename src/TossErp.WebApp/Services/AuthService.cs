using TossErp.Shared.DTOs;
using Microsoft.JSInterop;

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
    }

    public class AuthService : IAuthService
    {
        private readonly IJSRuntime _jsRuntime;

        public AuthService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
        }

        public async Task<UserDto?> GetCurrentUserAsync()
        {
            try
            {
                return await _jsRuntime.InvokeAsync<UserDto>("localStorage.getItem", "user");
            }
            catch
            {
                return null;
            }
        }

        public async Task SetTokenAsync(string token)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
        }

        public async Task SetUserAsync(UserDto user)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "user", user);
        }

        public async Task LogoutAsync()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "user");
        }
    }
} 
