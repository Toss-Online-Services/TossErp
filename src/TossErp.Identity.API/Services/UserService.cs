using TossErp.Shared.DTOs;
using TossErp.Identity.API.Models;

namespace TossErp.Identity.API.Services
{
    public class UserService : IUserService
    {
        private readonly List<ApplicationUser> _users = new();
        private readonly ILogger<UserService> _logger;

        public UserService(ILogger<UserService> logger)
        {
            _logger = logger;
            InitializeDefaultUsers();
        }

        private void InitializeDefaultUsers()
        {
            _users.AddRange(new[]
            {
                new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "admin",
                    Email = "admin@toss.com",
                    FirstName = "Admin",
                    LastName = "User",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    IsActive = true,
                    Roles = new[] { "Admin" },
                    CreatedAt = DateTime.UtcNow
                },
                new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "manager",
                    Email = "manager@toss.com",
                    FirstName = "John",
                    LastName = "Manager",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("manager123"),
                    IsActive = true,
                    Roles = new[] { "Manager" },
                    CreatedAt = DateTime.UtcNow
                },
                new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "cashier1",
                    Email = "cashier1@toss.com",
                    FirstName = "Jane",
                    LastName = "Cashier",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("cashier123"),
                    IsActive = true,
                    Roles = new[] { "Cashier" },
                    CreatedAt = DateTime.UtcNow
                }
            });
        }

        public async Task<UserDto?> ValidateUserAsync(string userName, string password)
        {
            var user = _users.FirstOrDefault(u => u.UserName == userName && u.IsActive);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null;
            }

            user.LastLoginAt = DateTime.UtcNow;
            return MapToDto(user);
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto request)
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = request.UserName,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Roles = request.Roles,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _users.Add(user);
            _logger.LogInformation("Created new user: {UserName}", user.UserName);
            return MapToDto(user);
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return _users.Select(MapToDto).ToList();
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            return user != null ? MapToDto(user) : null;
        }

        public async Task<UserDto?> UpdateUserAsync(Guid id, CreateUserDto request)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return null;
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            user.Roles = request.Roles;
            user.LastModifiedAt = DateTime.UtcNow;

            _logger.LogInformation("Updated user: {UserName}", user.UserName);
            return MapToDto(user);
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return false;
            }

            user.IsActive = false;
            user.LastModifiedAt = DateTime.UtcNow;
            _logger.LogInformation("Deactivated user: {UserName}", user.UserName);
            return true;
        }

        public async Task<UserDto?> ActivateUserAsync(Guid id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return null;
            }

            user.IsActive = true;
            user.LastModifiedAt = DateTime.UtcNow;
            _logger.LogInformation("Activated user: {UserName}", user.UserName);
            return MapToDto(user);
        }

        private static UserDto MapToDto(ApplicationUser user)
        {
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                Roles = user.Roles,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };
        }
    }
} 
