using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Stores;
using Toss.Domain.Entities;
using Toss.Domain.ValueObjects;

namespace Toss.Application.Stores.Commands.CreateStore;

public record CreateStoreCommand : IRequest<int>
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string OwnerId { get; init; } = string.Empty;
    public string? Url { get; init; }
    public bool SslEnabled { get; init; }
    public string? Hosts { get; init; }
    public int DisplayOrder { get; init; }
    public string? CompanyName { get; init; }
    public string? CompanyAddress { get; init; }
    public string? CompanyPhoneNumber { get; init; }
    public string? CompanyVat { get; init; }
    public string? ContactPhone { get; init; }
    public string? Email { get; init; }
    public string Currency { get; init; } = "ZAR";
    public decimal TaxRate { get; init; } = 15m;
    public string Language { get; init; } = "en";
    public string Timezone { get; init; } = "Africa/Johannesburg";
    public string? AreaGroup { get; init; }
    public TimeOnly? OpeningTime { get; init; }
    public TimeOnly? ClosingTime { get; init; }
    public bool WhatsAppAlertsEnabled { get; init; } = true;
    public bool GroupBuyingEnabled { get; init; } = true;
    public bool AIAssistantEnabled { get; init; } = true;
    
    // Location
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    
    // Address
    public string? AddressStreet { get; init; }
    public string? AddressCity { get; init; }
    public string? AddressProvince { get; init; }
    public string? AddressPostalCode { get; init; }
    public string? AddressCountry { get; init; }
}

public class CreateStoreCommandHandler : IRequestHandler<CreateStoreCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;

    public CreateStoreCommandHandler(IApplicationDbContext _context, IUser currentUser)
    {
        this._context = _context;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(CreateStoreCommand request, CancellationToken cancellationToken)
    {
        var store = new Store
        {
            Name = request.Name,
            Description = request.Description,
            OwnerId = request.OwnerId,
            Url = !string.IsNullOrWhiteSpace(request.Url) ? request.Url : string.Empty,
            Ssl_enabled = request.SslEnabled,
            Hosts = request.Hosts ?? string.Empty,
            DisplayOrder = request.DisplayOrder,
            CompanyName = request.CompanyName,
            CompanyAddress = request.CompanyAddress,
            CompanyPhoneNumber = request.CompanyPhoneNumber,
            CompanyVat = request.CompanyVat,
            ContactPhone = !string.IsNullOrWhiteSpace(request.ContactPhone) 
                ? new PhoneNumber(request.ContactPhone) 
                : null,
            Email = request.Email,
            Currency = request.Currency,
            TaxRate = request.TaxRate,
            Language = request.Language,
            Timezone = request.Timezone,
            AreaGroup = request.AreaGroup,
            OpeningTime = request.OpeningTime,
            ClosingTime = request.ClosingTime,
            WhatsAppAlertsEnabled = request.WhatsAppAlertsEnabled,
            GroupBuyingEnabled = request.GroupBuyingEnabled,
            AIAssistantEnabled = request.AIAssistantEnabled,
            IsActive = true
        };

        // Set location if provided
        if (request.Latitude.HasValue && request.Longitude.HasValue)
        {
            store.Location = new Location(request.Latitude.Value, request.Longitude.Value);
        }

        // Create address if provided
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
            
            store.AddressId = address.Id;
        }

        // Ensure URL ends with "/"
        if (!string.IsNullOrWhiteSpace(store.Url) && !store.Url.EndsWith("/"))
        {
            store.Url += "/";
        }

        _context.Stores.Add(store);
        await _context.SaveChangesAsync(cancellationToken);

        return store.Id;
    }
}

