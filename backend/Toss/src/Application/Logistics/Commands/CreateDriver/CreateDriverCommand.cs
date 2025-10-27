using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Logistics;
using Toss.Domain.ValueObjects;

namespace Toss.Application.Logistics.Commands.CreateDriver;

public record CreateDriverCommand : IRequest<int>
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? LicenseNumber { get; init; }
    public string? VehicleType { get; init; }
    public string? VehicleRegistration { get; init; }
    public bool IsActive { get; init; } = true;
    public bool IsAvailable { get; init; } = true;
}

public class CreateDriverCommandHandler : IRequestHandler<CreateDriverCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateDriverCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateDriverCommand request, CancellationToken cancellationToken)
    {
        var driver = new Driver
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = new PhoneNumber(request.PhoneNumber),
            Email = request.Email,
            LicenseNumber = request.LicenseNumber,
            VehicleType = request.VehicleType,
            VehicleRegistration = request.VehicleRegistration,
            IsActive = request.IsActive,
            IsAvailable = request.IsAvailable
        };

        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync(cancellationToken);

        return driver.Id;
    }
}

