using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities;
using Toss.Domain.Entities.Payments;

namespace Toss.Application.Payments.Commands.GeneratePayLink;

public record GeneratePayLinkCommand : IRequest<PayLinkResultDto>
{
    public int ShopId { get; init; }
    public decimal Amount { get; init; }
    public string Description { get; init; } = string.Empty;
    public string? SourceType { get; init; }
    public int? SourceId { get; init; }
    public int? CustomerId { get; init; }
    public string? CustomerName { get; init; }
    public string? CustomerPhone { get; init; }
    public string? CustomerEmail { get; init; }
    public int ExpiryHours { get; init; } = 24;
}

public class GeneratePayLinkCommandHandler : IRequestHandler<GeneratePayLinkCommand, PayLinkResultDto>
{
    private readonly IApplicationDbContext _context;

    public GeneratePayLinkCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PayLinkResultDto> Handle(GeneratePayLinkCommand request, CancellationToken cancellationToken)
    {
        // Validate shop exists
        var shop = await _context.Shops.FindAsync(new object[] { request.ShopId }, cancellationToken);
        if (shop == null)
            throw new NotFoundException(nameof(Shop), request.ShopId.ToString());

        var linkCode = GenerateLinkCode();
        var fullUrl = $"https://pay.toss.co.za/{linkCode}"; // TODO: Use actual configuration

        var payLink = new PayLink
        {
            LinkCode = linkCode,
            FullUrl = fullUrl,
            ShopId = request.ShopId,
            Amount = request.Amount,
            Currency = shop.Currency,
            Description = request.Description,
            SourceType = request.SourceType,
            SourceId = request.SourceId,
            CustomerId = request.CustomerId,
            CustomerName = request.CustomerName,
            CustomerPhone = request.CustomerPhone,
            CustomerEmail = request.CustomerEmail,
            IsActive = true,
            IsUsed = false,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(request.ExpiryHours)
        };

        _context.PayLinks.Add(payLink);
        await _context.SaveChangesAsync(cancellationToken);

        return new PayLinkResultDto
        {
            Id = payLink.Id,
            LinkCode = payLink.LinkCode,
            FullUrl = payLink.FullUrl,
            Amount = payLink.Amount,
            Currency = payLink.Currency,
            ExpiresAt = payLink.ExpiresAt
        };
    }

    private static string GenerateLinkCode()
    {
        // Generate a random 12-character alphanumeric code
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 12)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}

public class PayLinkResultDto
{
    public int Id { get; init; }
    public string LinkCode { get; init; } = string.Empty;
    public string FullUrl { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; init; }
}

