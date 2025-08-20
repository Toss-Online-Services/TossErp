using Identity.Application.DTOs;
using Identity.Application.Services;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

public record CreateUserCommand(CreateUserDto Dto) : IRequest<UserDto>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuditService _auditService;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IAuditService auditService,
        ILogger<CreateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Check if user already exists
        var existingUser = await _userRepository.GetByEmailAsync(dto.Email, dto.TenantId, cancellationToken);
        if (existingUser != null)
        {
            _logger.LogWarning("User with email {Email} already exists in tenant {TenantId}", 
                dto.Email, dto.TenantId);
            throw new InvalidOperationException($"User with email {dto.Email} already exists");
        }

        // Create new user
        var user = new User(
            dto.Email,
            dto.Password,
            dto.FirstName,
            dto.LastName,
            dto.TenantId,
            dto.Role);

        await _userRepository.AddAsync(user, cancellationToken);

        // Log audit trail
        await _auditService.LogUserActionAsync(
            user.Id,
            "UserCreated",
            dto.IpAddress,
            dto.UserAgent,
            dto.TenantId,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Created user {UserId} with email {Email} in tenant {TenantId}", 
            user.Id, dto.Email, dto.TenantId);

        return MapToDto(user);
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.TenantId,
            user.Role,
            user.CreatedAt,
            user.LastLoginAt);
    }
}
