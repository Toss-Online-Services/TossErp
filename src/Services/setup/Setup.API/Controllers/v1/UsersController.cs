using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Setup.Application.Common.Interfaces;
using Setup.Application.Features.Users.Commands.CreateUser;
using Setup.Application.Features.Users.Commands.UpdateUser;
using Setup.Application.Features.Users.Queries.GetUser;
using Setup.Application.Features.Users.Queries.GetUsers;
using MediatR;

namespace Setup.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IMediator mediator, IUserService userService, ILogger<UsersController> logger)
    {
        _mediator = mediator;
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get all users with optional filtering
    /// </summary>
    /// <param name="tenantId">Filter by tenant ID</param>
    /// <param name="role">Filter by role</param>
    /// <param name="isActive">Filter by active status</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of users</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetUsersResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GetUsersResponse>> GetUsers(
        [FromQuery] string? tenantId = null,
        [FromQuery] string? role = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            pageSize = Math.Min(pageSize, 100); // Limit page size

            var query = new GetUsersQuery
            {
                TenantId = tenantId,
                Role = role,
                IsActive = isActive,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return BadRequest(new { Message = "Error retrieving users", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get a specific user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GetUserResponse>> GetUser(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetUserQuery { Id = id };
            var result = await _mediator.Send(query, cancellationToken);

            if (result == null)
                return NotFound(new { Message = $"User with ID {id} not found" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {UserId}", id);
            return BadRequest(new { Message = "Error retrieving user", Details = ex.Message });
        }
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="command">User creation details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created user details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CreateUserResponse>> CreateUser(
        CreateUserCommand command, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetUser), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return BadRequest(new { Message = "Error creating user", Details = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="command">User update details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated user details</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UpdateUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UpdateUserResponse>> UpdateUser(
        string id, 
        UpdateUserCommand command, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (id != command.Id)
                return BadRequest(new { Message = "User ID in URL does not match command" });

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            return BadRequest(new { Message = "Error updating user", Details = ex.Message });
        }
    }

    /// <summary>
    /// Activate a user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost("{id}/activate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ActivateUser(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _userService.ActivateUserAsync(id, cancellationToken);
            
            if (!success)
                return NotFound(new { Message = $"User with ID {id} not found" });

            return Ok(new { Message = "User activated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating user {UserId}", id);
            return BadRequest(new { Message = "Error activating user", Details = ex.Message });
        }
    }

    /// <summary>
    /// Deactivate a user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="reason">Deactivation reason</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost("{id}/deactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeactivateUser(
        string id, 
        [FromBody] string reason,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _userService.DeactivateUserAsync(id, reason, cancellationToken);
            
            if (!success)
                return NotFound(new { Message = $"User with ID {id} not found" });

            return Ok(new { Message = "User deactivated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating user {UserId}", id);
            return BadRequest(new { Message = "Error deactivating user", Details = ex.Message });
        }
    }

    /// <summary>
    /// Lock a user account
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="reason">Lock reason</param>
    /// <param name="lockUntil">Lock until date (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost("{id}/lock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LockUser(
        string id,
        [FromBody] LockUserRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _userService.LockUserAsync(id, request.Reason, request.LockUntil, cancellationToken);
            
            if (!success)
                return NotFound(new { Message = $"User with ID {id} not found" });

            return Ok(new { Message = "User locked successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error locking user {UserId}", id);
            return BadRequest(new { Message = "Error locking user", Details = ex.Message });
        }
    }

    /// <summary>
    /// Unlock a user account
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost("{id}/unlock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UnlockUser(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _userService.UnlockUserAsync(id, cancellationToken);
            
            if (!success)
                return NotFound(new { Message = $"User with ID {id} not found" });

            return Ok(new { Message = "User unlocked successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlocking user {UserId}", id);
            return BadRequest(new { Message = "Error unlocking user", Details = ex.Message });
        }
    }

    /// <summary>
    /// Assign a role to a user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="role">Role name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost("{id}/roles/{role}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AssignRole(string id, string role, CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _userService.AssignRoleAsync(id, role, cancellationToken);
            
            if (!success)
                return NotFound(new { Message = $"User with ID {id} not found or role assignment failed" });

            return Ok(new { Message = $"Role '{role}' assigned successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role {Role} to user {UserId}", role, id);
            return BadRequest(new { Message = "Error assigning role", Details = ex.Message });
        }
    }

    /// <summary>
    /// Remove a role from a user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="role">Role name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}/roles/{role}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RemoveRole(string id, string role, CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _userService.RemoveRoleAsync(id, role, cancellationToken);
            
            if (!success)
                return NotFound(new { Message = $"User with ID {id} not found or role removal failed" });

            return Ok(new { Message = $"Role '{role}' removed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role {Role} from user {UserId}", role, id);
            return BadRequest(new { Message = "Error removing role", Details = ex.Message });
        }
    }

    /// <summary>
    /// Get user metrics
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="fromDate">Start date for metrics</param>
    /// <param name="toDate">End date for metrics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User metrics</returns>
    [HttpGet("{id}/metrics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserMetrics(
        string id, 
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var metrics = await _userService.GetUserMetricsAsync(id, fromDate, toDate, cancellationToken);
            return Ok(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user metrics {UserId}", id);
            return BadRequest(new { Message = "Error retrieving user metrics", Details = ex.Message });
        }
    }
}

// Request models
public class LockUserRequest
{
    public string Reason { get; set; } = string.Empty;
    public DateTime? LockUntil { get; set; }
}
