using TossErp.Identity.API.DTOs;

namespace TossErp.Identity.API.Services
{
    public interface IUserService
    {
        Task<UserDto?> ValidateUserAsync(string userName, string password);
        Task<UserDto> CreateUserAsync(CreateUserDto request);
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(Guid id);
        Task<UserDto?> UpdateUserAsync(Guid id, CreateUserDto request);
        Task<bool> DeleteUserAsync(Guid id);
        Task<UserDto?> ActivateUserAsync(Guid id);
    }
} 
