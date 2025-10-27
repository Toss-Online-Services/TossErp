using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Exceptions;
using Toss.Domain.Entities.Stores;
using Toss.Domain.Entities;
using Toss.Domain.ValueObjects;

namespace Toss.Application.Registration.Commands.RegisterStoreOwner;

public record RegisterStoreOwnerCommand : IRequest<RegisterStoreOwnerResult>
{
    // Shop Information
    public string ShopName { get; init; } = string.Empty;
    public string Area { get; init; } = string.Empty;
    public string? Zone { get; init; }
    public string Address { get; init; } = string.Empty;
    
    // Owner Information
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string? Email { get; init; }
    
    // Security
    public string Password { get; init; } = string.Empty;
    public bool WhatsappAlerts { get; init; } = true;
    
    // Optional Location
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
}

public record RegisterStoreOwnerResult
{
    public string UserId { get; init; } = string.Empty;
    public int StoreId { get; init; }
    public string Token { get; init; } = string.Empty;
    public UserDto User { get; init; } = null!;
    public StoreDto Store { get; init; } = null!;
}

public record UserDto
{
    public string Id { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public bool WhatsappAlerts { get; init; }
}

public record StoreDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Area { get; init; } = string.Empty;
    public string? Zone { get; init; }
    public string Address { get; init; } = string.Empty;
}

public class RegisterStoreOwnerCommandHandler : IRequestHandler<RegisterStoreOwnerCommand, RegisterStoreOwnerResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public RegisterStoreOwnerCommandHandler(
        IApplicationDbContext context,
        IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<RegisterStoreOwnerResult> Handle(RegisterStoreOwnerCommand request, CancellationToken cancellationToken)
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

        // Assign Store Owner role
        await _identityService.AddToRoleAsync(userId!, "StoreOwner");

        // Create store
        var store = new Store
        {
            Name = request.ShopName,
            Description = $"{request.ShopName} in {request.Area}",
            OwnerId = userId!,
            Url = string.Empty,
            Ssl_enabled = false,
            Hosts = string.Empty,
            DisplayOrder = 0,
            CompanyName = request.ShopName,
            CompanyAddress = request.Address,
            CompanyPhoneNumber = request.Phone,
            ContactPhone = new PhoneNumber(request.Phone),
            Email = request.Email,
            Currency = "ZAR",
            TaxRate = 15m,
            Language = "en",
            Timezone = "Africa/Johannesburg",
            AreaGroup = request.Area,
            WhatsAppAlertsEnabled = request.WhatsappAlerts,
            GroupBuyingEnabled = true,
            AIAssistantEnabled = true,
            IsActive = true
        };

        // Set location if provided
        if (request.Latitude.HasValue && request.Longitude.HasValue)
        {
            store.Location = new Location(request.Latitude.Value, request.Longitude.Value);
        }

        // Create address
        var address = new Address
        {
            Street = request.Address,
            City = request.Area,
            StateProvince = request.Zone ?? string.Empty,
            PostalCode = string.Empty,
            Country = "South Africa"
        };

        _context.Addresses.Add(address);
        await _context.SaveChangesAsync(cancellationToken);
        
        store.AddressId = address.Id;
        
        _context.Stores.Add(store);
        await _context.SaveChangesAsync(cancellationToken);

        // Generate token
        var token = await _identityService.GenerateTokenAsync(userId!);

        return new RegisterStoreOwnerResult
        {
            UserId = userId!,
            StoreId = store.Id,
            Token = token,
            User = new UserDto
            {
                Id = userId!,
                Email = request.Email ?? string.Empty,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                Role = "StoreOwner",
                WhatsappAlerts = request.WhatsappAlerts
            },
            Store = new StoreDto
            {
                Id = store.Id,
                Name = request.ShopName,
                Area = request.Area,
                Zone = request.Zone,
                Address = request.Address
            }
        };
    }
}

