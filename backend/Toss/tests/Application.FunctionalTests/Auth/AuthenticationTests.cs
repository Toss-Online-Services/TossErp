using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Toss.Infrastructure.Identity;
using static Toss.Application.FunctionalTests.Testing;

namespace Toss.Application.FunctionalTests.Auth;

public class AuthenticationTests : BaseTestFixture
{
    [Test]
    public async Task Login_WithEmail_ReturnsTokens()
    {
        // Arrange
        var email = "authuser@local";
        var password = "Testing1234!";
        await RunAsUserAsync(email, password, Array.Empty<string>());

        var client = CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Password = password
        });

        // Assert
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.That(payload.GetProperty("token").GetString(), Is.Not.Empty);
        Assert.That(payload.GetProperty("refreshToken").GetString(), Is.Not.Empty);
    }

    [Test]
    public async Task Login_WithPhone_SendsOtp_And_Verifies()
    {
        var email = "otpuser@local";
        var password = "Testing1234!";
        var userId = await RunAsUserAsync(email, password, Array.Empty<string>());

        using (var scope = GetScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByIdAsync(userId);
            user!.PhoneNumber = "+27821234567";
            user.PhoneNumberConfirmed = true;
            user.TwoFactorEnabled = true;
            await userManager.UpdateAsync(user);
        }

        var client = CreateClient();

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new
        {
            PhoneNumber = "+27821234567",
            Password = password
        });

        Assert.That(loginResponse.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.Accepted));
        var challenge = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        var sessionId = challenge.GetProperty("sessionId").GetString();
        Assert.That(sessionId, Is.Not.Empty);

        var otpSender = GetRequiredService<TestOtpSender>();
        var code = otpSender.GetLastCode("+27821234567");
        Assert.That(code, Is.Not.Empty);

        var verifyResponse = await client.PostAsJsonAsync("/api/auth/otp/verify", new
        {
            SessionId = sessionId,
            Code = code
        });

        verifyResponse.EnsureSuccessStatusCode();
        var tokens = await verifyResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.That(tokens.GetProperty("token").GetString(), Is.Not.Empty);
    }

    [Test]
    public async Task RefreshToken_IssuesNewTokenPair()
    {
        var email = "refreshuser@local";
        var password = "Testing1234!";
        await RunAsUserAsync(email, password, Array.Empty<string>());

        var client = CreateClient();
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Password = password
        });

        loginResponse.EnsureSuccessStatusCode();
        var payload = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        var refreshToken = payload.GetProperty("refreshToken").GetString();
        Assert.That(refreshToken, Is.Not.Empty);

        var refreshResponse = await client.PostAsJsonAsync("/api/auth/refresh", new
        {
            RefreshToken = refreshToken
        });

        refreshResponse.EnsureSuccessStatusCode();
        var refreshed = await refreshResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.That(refreshed.GetProperty("token").GetString(), Is.Not.Empty);
        Assert.That(refreshed.GetProperty("refreshToken").GetString(), Is.Not.EqualTo(refreshToken));
    }

    private static IServiceScope GetScope() => GetRequiredService<IServiceScopeFactory>().CreateScope();
}

