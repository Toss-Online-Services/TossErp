using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TossErp.POS.Domain.Common;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate;

public interface ISaleRepository : IRepository<Sale>
{
    new Task<Sale?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetByStoreIdAsync(int storeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetByStaffIdAsync(int staffId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetOfflineSalesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetByCustomerAsync(int customerId, CancellationToken cancellationToken = default);
} 
