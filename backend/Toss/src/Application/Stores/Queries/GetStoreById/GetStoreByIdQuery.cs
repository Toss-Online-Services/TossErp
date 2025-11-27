using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Stores;

namespace Toss.Application.Stores.Queries.GetStoreById;

public record StoreDetailDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string OwnerId { get; init; } = string.Empty;
    public int BusinessId { get; init; }
    public string BusinessName { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public bool SslEnabled { get; init; }
    public string Hosts { get; init; } = string.Empty;
    public int DisplayOrder { get; init; }
    public string? CompanyName { get; init; }
    public string? CompanyAddress { get; init; }
    public string? CompanyPhoneNumber { get; init; }
    public string? CompanyVat { get; init; }
    public string? ContactPhone { get; init; }
    public string? Email { get; init; }
    public string Currency { get; init; } = string.Empty;
    public decimal TaxRate { get; init; }
    public string Language { get; init; } = string.Empty;
    public string Timezone { get; init; } = string.Empty;
    public string? AreaGroup { get; init; }
    public TimeOnly? OpeningTime { get; init; }
    public TimeOnly? ClosingTime { get; init; }
    public bool WhatsAppAlertsEnabled { get; init; }
    public bool GroupBuyingEnabled { get; init; }
    public bool AIAssistantEnabled { get; init; }
    public bool IsActive { get; init; }
    
    // Location
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    
    // Address
    public int? AddressId { get; init; }
    public string? AddressStreet { get; init; }
    public string? AddressCity { get; init; }
    public string? AddressProvince { get; init; }
    public string? AddressPostalCode { get; init; }
    public string? AddressCountry { get; init; }
    
    // Stats
    public int CustomerCount { get; init; }
    public int ProductCount { get; init; }
    public int SalesCount { get; init; }
    public decimal TotalRevenue { get; init; }
}

public record GetStoreByIdQuery : IRequest<StoreDetailDto>
{
    public int Id { get; init; }
}

public class GetStoreByIdQueryHandler : IRequestHandler<GetStoreByIdQuery, StoreDetailDto>
{
    private readonly IApplicationDbContext _context;

    public GetStoreByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<StoreDetailDto> Handle(GetStoreByIdQuery request, CancellationToken cancellationToken)
    {
        var store = await _context.Stores
            .Include(s => s.Address)
            .Include(s => s.Business)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (store == null)
            throw new NotFoundException(nameof(Store), request.Id.ToString());

        // Get stats
        var customerCount = await _context.Customers
            .CountAsync(c => c.ShopId == request.Id, cancellationToken);
        
        var productCount = await _context.StockLevels
            .CountAsync(sl => sl.ShopId == request.Id, cancellationToken);
        
        var salesCount = await _context.Sales
            .CountAsync(s => s.ShopId == request.Id, cancellationToken);
        
        var totalRevenue = await _context.Sales
            .Where(s => s.ShopId == request.Id && s.Status == Domain.Enums.SaleStatus.Completed)
            .SumAsync(s => s.TotalAmount, cancellationToken);

        return new StoreDetailDto
        {
            Id = store.Id,
            Name = store.Name,
            Description = store.Description,
            OwnerId = store.OwnerId,
            BusinessId = store.BusinessId,
            BusinessName = store.Business?.Name ?? string.Empty,
            Url = store.Url,
            SslEnabled = store.Ssl_enabled,
            Hosts = store.Hosts,
            DisplayOrder = store.DisplayOrder,
            CompanyName = store.CompanyName,
            CompanyAddress = store.CompanyAddress,
            CompanyPhoneNumber = store.CompanyPhoneNumber,
            CompanyVat = store.CompanyVat,
            ContactPhone = store.ContactPhone?.ToString(),
            Email = store.Email,
            Currency = store.Currency,
            TaxRate = store.TaxRate,
            Language = store.Language,
            Timezone = store.Timezone,
            AreaGroup = store.AreaGroup,
            OpeningTime = store.OpeningTime,
            ClosingTime = store.ClosingTime,
            WhatsAppAlertsEnabled = store.WhatsAppAlertsEnabled,
            GroupBuyingEnabled = store.GroupBuyingEnabled,
            AIAssistantEnabled = store.AIAssistantEnabled,
            IsActive = store.IsActive,
            Latitude = store.Location?.Latitude,
            Longitude = store.Location?.Longitude,
            AddressId = store.AddressId,
            AddressStreet = store.Address?.Street,
            AddressCity = store.Address?.City,
            AddressProvince = store.Address?.StateProvince,
            AddressPostalCode = store.Address?.PostalCode,
            AddressCountry = store.Address?.Country,
            CustomerCount = customerCount,
            ProductCount = productCount,
            SalesCount = salesCount,
            TotalRevenue = totalRevenue
        };
    }
}


