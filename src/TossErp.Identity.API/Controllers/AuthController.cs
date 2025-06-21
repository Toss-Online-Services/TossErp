using Microsoft.AspNetCore.Mvc;
using TossErp.Identity.API.DTOs;
using TossErp.Identity.API.Services;
using TossErp.Identity.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace TossErp.Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(JwtService jwtService, IUserService userService, UserManager<ApplicationUser> userManager)
        {
            _jwtService = jwtService;
            _userService = userService;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var userDto = await _userService.ValidateUserAsync(request.UserName, request.Password);
                if (userDto == null)
                {
                    return Unauthorized(new { message = "Invalid username or password" });
                }

                // Get the ApplicationUser for JWT token generation
                var applicationUser = await _userManager.FindByNameAsync(request.UserName);
                if (applicationUser == null)
                {
                    return Unauthorized(new { message = "User not found" });
                }

                var token = _jwtService.GenerateToken(applicationUser);
                return Ok(new { token, user = userDto });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login", error = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDto request)
        {
            try
            {
                var user = await _userService.CreateUserAsync(request);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to create user", error = ex.Message });
            }
        }

        [HttpGet("users")]
        [Authorize]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve users", error = ex.Message });
            }
        }

        [HttpGet("users/{id}")]
        [Authorize]
        public async Task<IActionResult> GetUser(Guid id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve user", error = ex.Message });
            }
        }

        [HttpPut("users/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] CreateUserDto request)
        {
            try
            {
                var user = await _userService.UpdateUserAsync(id, request);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to update user", error = ex.Message });
            }
        }

        [HttpDelete("users/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                var success = await _userService.DeleteUserAsync(id);
                if (!success)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to delete user", error = ex.Message });
            }
        }

        [HttpPost("users/{id}/activate")]
        [Authorize]
        public async Task<IActionResult> ActivateUser(Guid id)
        {
            try
            {
                var user = await _userService.ActivateUserAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to activate user", error = ex.Message });
            }
        }
    }
} 
