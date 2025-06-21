using TossErp.Shared.DTOs;
using TossErp.Identity.API.Models;
using Microsoft.AspNetCore.Identity;

namespace TossErp.Identity.API.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserService> _logger;

        public UserService(UserManager<ApplicationUser> userManager, ILogger<UserService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<UserDto?> ValidateUserAsync(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            {
                return null;
            }

            return MapToDto(user);
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto request)
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = request.UserName,
                Email = request.Email,
                Name = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            _logger.LogInformation("Created new user: {UserName}", user.UserName);
            return MapToDto(user);
        }

        public Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            return Task.FromResult(users.Select(MapToDto).ToList());
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            return user != null ? MapToDto(user) : null;
        }

        public async Task<UserDto?> UpdateUserAsync(Guid id, CreateUserDto request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return null;
            }

            user.Name = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to update user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            _logger.LogInformation("Updated user: {UserName}", user.UserName);
            return MapToDto(user);
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to delete user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            _logger.LogInformation("Deleted user: {UserName}", user.UserName);
            return true;
        }

        public async Task<UserDto?> ActivateUserAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return null;
            }

            user.LockoutEnabled = false;
            user.LockoutEnd = null;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to activate user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            _logger.LogInformation("Activated user: {UserName}", user.UserName);
            return MapToDto(user);
        }

        private static UserDto MapToDto(ApplicationUser user)
        {
            return new UserDto
            {
                Id = Guid.Parse(user.Id),
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FirstName = user.Name,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                IsActive = !user.LockoutEnabled,
                Roles = new string[0], // TODO: Get roles from UserManager
                CreatedAt = DateTime.UtcNow, // TODO: Add CreatedAt to ApplicationUser if needed
                LastLoginAt = null // TODO: Add LastLoginAt to ApplicationUser if needed
            };
        }
    }
} 
