namespace eShop.POS.API.Services;

public interface IStaffService
{
    Task RecordTip(string staffId, decimal amount, int saleId, CancellationToken cancellationToken = default);
} 
