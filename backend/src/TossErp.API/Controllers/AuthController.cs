using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TossErp.Domain.Entities.Auth;
using TossErp.Infrastructure.Data;
using TossErp.Infrastructure.Services;

namespace TossErp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        ApplicationDbContext context,
        IAuthService authService,
        ILogger<AuthController> logger)
    {
        _context = context;
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest(new { error = "Email already registered" });

            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return BadRequest(new { error = "Username already taken" });

            // Create new user
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = _authService.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            
            // Assign default role
            var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (defaultRole != null)
            {
                user.UserRoles.Add(new UserRole
                {
                    RoleId = defaultRole.Id,
                    AssignedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("New user registered: {Email}", user.Email);

            // Generate tokens
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            var token = _authService.GenerateJwtToken(user, roles, new List<string>());
            var refreshToken = _authService.GenerateRefreshToken(GetIpAddress());
            
            refreshToken.UserId = user.Id;
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Register), new AuthResponse
            {
                Token = token,
                RefreshToken = refreshToken.Token,
                User = new UserDto
                {
                    Id = user.Id,
                    Name = user.FullName,
                    Email = user.Email,
                    Roles = roles,
                    Permissions = new List<string>()
                },
                ExpiresIn = 3600
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return BadRequest(new { error = "Registration failed", message = ex.Message });
        }
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Email == request.Email || u.Username == request.Email);

            if (user == null)
                return Unauthorized(new { error = "Invalid credentials" });

            if (!user.IsActive)
                return Unauthorized(new { error = "Account is deactivated" });

            if (user.IsLockedOut())
                return Unauthorized(new { error = $"Account is locked until {user.LockedOutUntil:yyyy-MM-dd HH:mm}" });

            // Verify password
            if (!_authService.VerifyPassword(request.Password, user.PasswordHash))
            {
                user.RecordFailedLogin();
                await _context.SaveChangesAsync();
                return Unauthorized(new { error = "Invalid credentials" });
            }

            // Record successful login
            user.RecordSuccessfulLogin(GetIpAddress());

            // Get roles and permissions
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            var permissions = user.UserRoles
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission.Name)
                .Distinct()
                .ToList();

            // Generate tokens
            var token = _authService.GenerateJwtToken(user, roles, permissions);
            var refreshToken = _authService.GenerateRefreshToken(GetIpAddress());
            
            refreshToken.UserId = user.Id;
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User logged in: {Email}", user.Email);

            return Ok(new AuthResponse
            {
                Token = token,
                RefreshToken = refreshToken.Token,
                User = new UserDto
                {
                    Id = user.Id,
                    Name = user.FullName,
                    Email = user.Email,
                    Roles = roles,
                    Permissions = permissions
                },
                ExpiresIn = 3600
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return BadRequest(new { error = "Login failed", message = ex.Message });
        }
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
                .ThenInclude(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

        if (refreshToken == null || !refreshToken.IsActive)
            return Unauthorized(new { error = "Invalid refresh token" });

        var user = refreshToken.User;

        if (!user.IsActive)
            return Unauthorized(new { error = "Account is deactivated" });

        // Revoke old refresh token
        refreshToken.IsRevoked = true;
        refreshToken.RevokedAt = DateTime.UtcNow;
        refreshToken.RevokedByIp = GetIpAddress();

        // Generate new tokens
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToList();

        var newJwtToken = _authService.GenerateJwtToken(user, roles, permissions);
        var newRefreshToken = _authService.GenerateRefreshToken(GetIpAddress());
        
        newRefreshToken.UserId = user.Id;
        refreshToken.ReplacedByToken = newRefreshToken.Token;
        
        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync();

        return Ok(new AuthResponse
        {
            Token = newJwtToken,
            RefreshToken = newRefreshToken.Token,
            User = new UserDto
            {
                Id = user.Id,
                Name = user.FullName,
                Email = user.Email,
                Roles = roles,
                Permissions = permissions
            },
            ExpiresIn = 3600
        });
    }

    /// <summary>
    /// Revoke refresh token
    /// </summary>
    [HttpPost("revoke")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequest request)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

        if (refreshToken == null || !refreshToken.IsActive)
            return BadRequest(new { error = "Invalid token" });

        refreshToken.IsRevoked = true;
        refreshToken.RevokedAt = DateTime.UtcNow;
        refreshToken.RevokedByIp = GetIpAddress();

        await _context.SaveChangesAsync();

        return Ok(new { message = "Token revoked" });
    }

    private string GetIpAddress()
    {
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
            return Request.Headers["X-Forwarded-For"].ToString();

        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }
}

// Request/Response DTOs
public record LoginRequest(string Email, string Password);

public record RegisterRequest(
    string Username,
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? Phone
);

public record RefreshTokenRequest(string RefreshToken);

public record AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
    public int ExpiresIn { get; set; }
}

public record UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
}

