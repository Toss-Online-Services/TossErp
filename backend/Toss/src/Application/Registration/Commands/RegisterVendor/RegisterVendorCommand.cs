using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Exceptions;
using Toss.Domain.Entities.Vendors;
using Toss.Domain.Entities;
using Toss.Domain.ValueObjects;

namespace Toss.Application.Registration.Commands.RegisterVendor;

public record RegisterVendorCommand : IRequest<RegisterVendorResult>
{
    // Company Information
    public string CompanyName { get; init; } = string.Empty;
    public string? CompanyRegNumber { get; init; }
    public string? VATNumber { get; init; }
    public string? Description { get; init; }
    public string? Website { get; init; }
    
    // Contact Person
    public string ContactPerson { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    
    // Security
    public string Password { get; init; } = string.Empty;
    
    // Address
    public string? AddressStreet { get; init; }
    public string? AddressCity { get; init; }
    public string? AddressProvince { get; init; }
    public string? AddressPostalCode { get; init; }
    public string? AddressCountry { get; init; }
    
    // Business Terms
    public int PaymentTermsDays { get; init; } = 30;
    public decimal? CreditLimit { get; init; }
}

public record RegisterVendorResult
{
    public string UserId { get; init; } = string.Empty;
    public int VendorId { get; init; }
    public string Token { get; init; } = string.Empty;
    public VendorUserDto User { get; init; } = null!;
    public VendorCompanyDto Vendor { get; init; } = null!;
}

public record VendorUserDto
{
    public string Id { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string ContactPerson { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}

public record VendorCompanyDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? CompanyRegNumber { get; init; }
    public string? VATNumber { get; init; }
    public string? Website { get; init; }
}

public class RegisterVendorCommandHandler : IRequestHandler<RegisterVendorCommand, RegisterVendorResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public RegisterVendorCommandHandler(
        IApplicationDbContext context,
        IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<RegisterVendorResult> Handle(RegisterVendorCommand request, CancellationToken cancellationToken)
    {
        // Validate phone number doesn't already exist
        var existingUser = await _identityService.GetUserByPhoneAsync(request.Phone);
        if (existingUser != null)
        {
            throw new BadRequestException("An account with this phone number already exists");
        }

        // Validate email
        existingUser = await _identityService.GetUserByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new BadRequestException("An account with this email already exists");
        }

        // Create user account
        var (result, userId) = await _identityService.CreateUserAsync(
            userName: request.Email,
            email: request.Email,
            password: request.Password,
            phoneNumber: request.Phone,
            firstName: request.ContactPerson.Split(' ')[0],
            lastName: request.ContactPerson.Contains(' ') ? string.Join(" ", request.ContactPerson.Split(' ').Skip(1)) : string.Empty);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors);
            throw new BadRequestException($"User creation failed: {errors}");
        }

        // Assign Vendor role
        await _identityService.AddToRoleAsync(userId!, "Vendor");

        // Create address if provided
        int? addressId = null;
        if (!string.IsNullOrWhiteSpace(request.AddressStreet))
        {
            var address = new Address
            {
                Street = request.AddressStreet,
                City = request.AddressCity ?? string.Empty,
                StateProvince = request.AddressProvince ?? string.Empty,
                PostalCode = request.AddressPostalCode ?? string.Empty,
                Country = request.AddressCountry ?? "South Africa"
            };

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync(cancellationToken);
            addressId = address.Id;
        }

        // Create vendor
        var vendor = new Vendor
        {
            Name = request.CompanyName,
            Email = request.Email,
            Description = request.Description,
            CompanyRegNumber = request.CompanyRegNumber,
            VATNumber = request.VATNumber,
            ContactPerson = request.ContactPerson,
            ContactPhone = new PhoneNumber(request.Phone),
            Website = request.Website,
            Active = true,
            CreditLimit = request.CreditLimit,
            PaymentTermsDays = request.PaymentTermsDays,
            DisplayOrder = 0,
            Deleted = false,
            AddressId = addressId
        };

        _context.Vendors.Add(vendor);
        await _context.SaveChangesAsync(cancellationToken);

        // Generate token
        var token = await _identityService.GenerateTokenAsync(userId!);

        return new RegisterVendorResult
        {
            UserId = userId!,
            VendorId = vendor.Id,
            Token = token,
            User = new VendorUserDto
            {
                Id = userId!,
                Email = request.Email,
                ContactPerson = request.ContactPerson,
                Phone = request.Phone,
                Role = "Vendor"
            },
            Vendor = new VendorCompanyDto
            {
                Id = vendor.Id,
                Name = request.CompanyName,
                CompanyRegNumber = request.CompanyRegNumber,
                VATNumber = request.VATNumber,
                Website = request.Website
            }
        };
    }
}



