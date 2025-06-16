using POS.Domain.Models;
using POS.Domain.Repositories;

namespace POS.Domain.Repositories;

public interface IPaymentMethodRepository : IRepository<PaymentMethod>
{
    Task<PaymentMethod?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<PaymentMethod>> GetByTypeAsync(string type, CancellationToken cancellationToken = default);
} 
