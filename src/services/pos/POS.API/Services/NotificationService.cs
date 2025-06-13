using Microsoft.Extensions.Logging;
using eShop.POS.Infrastructure.Repositories;

namespace eShop.POS.API.Services;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly IStoreRepository _storeRepository;
    private readonly IEmailService _emailService;
    private readonly IPushNotificationService _pushNotificationService;

    public NotificationService(
        ILogger<NotificationService> logger,
        IStoreRepository storeRepository,
        IEmailService emailService,
        IPushNotificationService pushNotificationService)
    {
        _logger = logger;
        _storeRepository = storeRepository;
        _emailService = emailService;
        _pushNotificationService = pushNotificationService;
    }

    public async Task NotifyHighValueSale(string storeId, int saleId, decimal total, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending high-value sale notification for store {StoreId}, sale {SaleId}", storeId, saleId);

        try
        {
            var store = await _storeRepository.GetAsync(storeId);
            if (store == null)
                throw new InvalidOperationException($"Store {storeId} not found");

            // Send email notification to store owner
            if (!string.IsNullOrEmpty(store.OwnerEmail))
            {
                await _emailService.SendHighValueSaleNotification(
                    store.OwnerEmail,
                    store.Name,
                    saleId,
                    total,
                    cancellationToken);
            }

            // Send push notification to store staff
            if (store.NotificationTokens.Any())
            {
                await _pushNotificationService.SendHighValueSaleNotification(
                    store.NotificationTokens,
                    store.Name,
                    saleId,
                    total,
                    cancellationToken);
            }

            _logger.LogInformation("Successfully sent high-value sale notification for store {StoreId}, sale {SaleId}", storeId, saleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending high-value sale notification for store {StoreId}, sale {SaleId}", storeId, saleId);
            throw;
        }
    }
} 
