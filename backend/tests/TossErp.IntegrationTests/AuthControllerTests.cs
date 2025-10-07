using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TossErp.Domain.Entities.Auth;
using TossErp.Infrastructure.Data;
using Xunit;

namespace TossErp.IntegrationTests;

public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AuthControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"AuthTestDb_{Guid.NewGuid()}");
                });

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated();
                
                // Seed default role
                db.Roles.Add(new Role
                {
                    Name = "User",
                    Description = "Default user role",
                    IsSystem = true,
                    CreatedAt = DateTime.UtcNow
                });
                db.SaveChanges();
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Register_ValidUser_ReturnsToken()
    {
        // Arrange
        var registerRequest = new
        {
            username = "testuser",
            email = "test@example.com",
            password = "Test@123456",
            firstName = "Test",
            lastName = "User",
            phone = "0821234567"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var authResponse = await response.Content.ReadFromJsonAsync<dynamic>();
        authResponse.Should().NotBeNull();
        
        string token = authResponse?.token?.ToString() ?? "";
        token.Should().NotBeEmpty();
        
        string refreshToken = authResponse?.refreshToken?.ToString() ?? "";
        refreshToken.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        // Arrange - First register a user
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var authService = scope.ServiceProvider.GetRequiredService<TossErp.Infrastructure.Services.IAuthService>();
        
        var user = new User
        {
            Username = "logintest",
            Email = "login@example.com",
            PasswordHash = authService.HashPassword("Password123"),
            FirstName = "Login",
            LastName = "Test",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        
        var role = await context.Roles.FirstAsync();
        user.UserRoles.Add(new UserRole
        {
            RoleId = role.Id,
            AssignedAt = DateTime.UtcNow
        });
        
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var loginRequest = new
        {
            email = "login@example.com",
            password = "Password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var authResponse = await response.Content.ReadFromJsonAsync<dynamic>();
        authResponse.Should().NotBeNull();
        
        string token = authResponse?.token?.ToString() ?? "";
        token.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new
        {
            email = "nonexistent@example.com",
            password = "WrongPassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var existingUser = new User
        {
            Username = "existing",
            Email = "duplicate@example.com",
            PasswordHash = "hash",
            FirstName = "Existing",
            LastName = "User",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        context.Users.Add(existingUser);
        await context.SaveChangesAsync();

        var registerRequest = new
        {
            username = "newuser",
            email = "duplicate@example.com",
            password = "Password123",
            firstName = "New",
            lastName = "User"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

