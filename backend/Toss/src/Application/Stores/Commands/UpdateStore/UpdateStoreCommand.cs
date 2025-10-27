using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Stores;
using Toss.Domain.Entities;
using Toss.Domain.ValueObjects;

namespace Toss.Application.Stores.Commands.UpdateStore;

public record UpdateStoreCommand : IRequest<bool>
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
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
    public bool IsActive { get; init; } = true;
    
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

public class UpdateStoreCommandHandler : IRequestHandler<UpdateStoreCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateStoreCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateStoreCommand request, CancellationToken cancellationToken)
    {
        var store = await _context.Stores
            .Include(s => s.Address)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (store == null)
            throw new NotFoundException(nameof(Store), request.Id.ToString());

        // Update store properties
        store.Name = request.Name;
        store.Description = request.Description;
        store.Url = !string.IsNullOrWhiteSpace(request.Url) ? request.Url : string.Empty;
        store.Ssl_enabled = request.SslEnabled;
        store.Hosts = request.Hosts ?? string.Empty;
        store.DisplayOrder = request.DisplayOrder;
        store.CompanyName = request.CompanyName;
        store.CompanyAddress = request.CompanyAddress;
        store.CompanyPhoneNumber = request.CompanyPhoneNumber;
        store.CompanyVat = request.CompanyVat;
        store.ContactPhone = !string.IsNullOrWhiteSpace(request.ContactPhone) 
            ? new PhoneNumber(request.ContactPhone) 
            : null;
        store.Email = request.Email;
        store.Currency = request.Currency;
        store.TaxRate = request.TaxRate;
        store.Language = request.Language;
        store.Timezone = request.Timezone;
        store.AreaGroup = request.AreaGroup;
        store.OpeningTime = request.OpeningTime;
        store.ClosingTime = request.ClosingTime;
        store.WhatsAppAlertsEnabled = request.WhatsAppAlertsEnabled;
        store.GroupBuyingEnabled = request.GroupBuyingEnabled;
        store.AIAssistantEnabled = request.AIAssistantEnabled;
        store.IsActive = request.IsActive;

        // Update location if provided
        if (request.Latitude.HasValue && request.Longitude.HasValue)
        {
            store.Location = new Location(request.Latitude.Value, request.Longitude.Value);
        }

        // Update or create address if provided
        if (!string.IsNullOrWhiteSpace(request.AddressStreet))
        {
            if (store.Address != null)
            {
                store.Address.Street = request.AddressStreet;
                store.Address.City = request.AddressCity ?? string.Empty;
                store.Address.StateProvince = request.AddressProvince ?? string.Empty;
                store.Address.PostalCode = request.AddressPostalCode ?? string.Empty;
                store.Address.Country = request.AddressCountry ?? "South Africa";
            }
            else
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
        }

        // Ensure URL ends with "/"
        if (!string.IsNullOrWhiteSpace(store.Url) && !store.Url.EndsWith("/"))
        {
            store.Url += "/";
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

