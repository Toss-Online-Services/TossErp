using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Exceptions;
using Toss.Domain.Entities.Logistics;
using Toss.Domain.ValueObjects;

namespace Toss.Application.Registration.Commands.RegisterDriver;

public record RegisterDriverCommand : IRequest<RegisterDriverResult>
{
    // Driver Information
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string? Email { get; init; }
    
    // License & Vehicle
    public string? LicenseNumber { get; init; }
    public string? VehicleType { get; init; }
    public string? VehicleRegistration { get; init; }
    
    // Security
    public string Password { get; init; } = string.Empty;
}

public record RegisterDriverResult
{
    public string UserId { get; init; } = string.Empty;
    public int DriverId { get; init; }
    public string Token { get; init; } = string.Empty;
    public DriverUserDto User { get; init; } = null!;
    public DriverProfileDto Driver { get; init; } = null!;
}

public record DriverUserDto
{
    public string Id { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}

public record DriverProfileDto
{
    public int Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? LicenseNumber { get; init; }
    public string? VehicleType { get; init; }
    public string? VehicleRegistration { get; init; }
    public bool IsActive { get; init; }
    public bool IsAvailable { get; init; }
}

public class RegisterDriverCommandHandler : IRequestHandler<RegisterDriverCommand, RegisterDriverResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public RegisterDriverCommandHandler(
        IApplicationDbContext context,
        IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<RegisterDriverResult> Handle(RegisterDriverCommand request, CancellationToken cancellationToken)
    {
        // Validate phone number doesn't already exist
        var existingUser = await _identityService.GetUserByPhoneAsync(request.Phone);
        if (existingUser != null)
        {
            throw new BadRequestException("An account with this phone number already exists");
        }

        // Validate email if provided
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            existingUser = await _identityService.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new BadRequestException("An account with this email already exists");
            }
        }

        // Create user account
        var (result, userId) = await _identityService.CreateUserAsync(
            userName: request.Email ?? request.Phone,
            email: request.Email,
            password: request.Password,
            phoneNumber: request.Phone,
            firstName: request.FirstName,
            lastName: request.LastName);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors);
            throw new BadRequestException($"User creation failed: {errors}");
        }

        // Assign Driver role
        await _identityService.AddToRoleAsync(userId!, "Driver");

        // Create driver profile
        var driver = new Driver
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = new PhoneNumber(request.Phone),
            Email = request.Email,
            LicenseNumber = request.LicenseNumber,
            VehicleType = request.VehicleType,
            VehicleRegistration = request.VehicleRegistration,
            IsActive = true,
            IsAvailable = true
        };

        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync(cancellationToken);

        // Generate token
        var token = await _identityService.GenerateTokenAsync(userId!);

        return new RegisterDriverResult
        {
            UserId = userId!,
            DriverId = driver.Id,
            Token = token,
            User = new DriverUserDto
            {
                Id = userId!,
                Email = request.Email ?? string.Empty,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                Role = "Driver"
            },
            Driver = new DriverProfileDto
            {
                Id = driver.Id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                LicenseNumber = request.LicenseNumber,
                VehicleType = request.VehicleType,
                VehicleRegistration = request.VehicleRegistration,
                IsActive = true,
                IsAvailable = true
            }
        };
    }
}

