using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;

namespace TossErp.WebApp.Services
{
    public class AuthenticatedHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private readonly AuthenticationStateProvider _authStateProvider;

        public AuthenticatedHttpClient(
            HttpClient httpClient,
            IAuthService authService,
            AuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _authService = authService;
            _authStateProvider = authStateProvider;
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            await SetAuthorizationHeader();
            return await _httpClient.GetAsync(requestUri);
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            await SetAuthorizationHeader();
            return await _httpClient.PostAsync(requestUri, content);
        }

        public async Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content)
        {
            await SetAuthorizationHeader();
            return await _httpClient.PutAsync(requestUri, content);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string requestUri)
        {
            await SetAuthorizationHeader();
            return await _httpClient.DeleteAsync(requestUri);
        }

        public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string requestUri, T data)
        {
            await SetAuthorizationHeader();
            return await _httpClient.PostAsJsonAsync(requestUri, data);
        }

        public async Task<HttpResponseMessage> PutAsJsonAsync<T>(string requestUri, T data)
        {
            await SetAuthorizationHeader();
            return await _httpClient.PutAsJsonAsync(requestUri, data);
        }

        private async Task SetAuthorizationHeader()
        {
            var token = await _authService.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }
    }
} 
